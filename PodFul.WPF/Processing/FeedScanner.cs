
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using Jabberwocky.Toolkit.Object;
  using Jabberwocky.Toolkit.String;
  using Library;
  using Logging;
  using Miscellaneous;
  using Windows;

  /// <summary>
  /// Scans a list of feeds and collects a list of podcasts to be downloaded. Feeds with new podcasts are updated.
  /// </summary>
  public class FeedScanner
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private DownloadManager downloadManager;

    private IFeedCollection feedCollection;

    private Queue<Int32> feedIndexes;

    private MessagePool fileDeliveryLogger;

    private IImageResolver imageResolver;

    private ILogController logController;

    private IPodcastDownloadConfirmer podcastDownloadConfirmer;
    #endregion

    #region Construction
    public FeedScanner(
      IFeedCollection feedCollection,
      Queue<Int32> feedIndexes,
      IImageResolver imageResolver,
      MessagePool fileDeliveryLogger,
      ILogController logController,
      IPodcastDownloadConfirmer podcastDownloadConfirmer,
      DownloadManager downloadManager)
    {
      this.feedCollection = feedCollection;
      this.feedIndexes = feedIndexes;
      this.imageResolver = imageResolver;
      this.fileDeliveryLogger = fileDeliveryLogger;
      this.logController = logController;
      this.podcastDownloadConfirmer = podcastDownloadConfirmer;
      this.downloadManager = downloadManager;
    }
    #endregion

    #region Properties
    public ObservableCollection<DownloadJob> Jobs { get { return this.downloadManager.Jobs; } }
    #endregion

    #region Events
    public event Action FeedStartedEvent;
    public event Action ScanCompletedEvent;
    public event Action ScanCanceledEvent;
    #endregion

    #region Methods
    public void Cancel()
    {
      this.cancellationTokenSource.Cancel();
      this.downloadManager.CancelAllDownloads();
    }

    public void CancelDownload(Object dataContext)
    {
      dataContext.VerifyThatObjectIsNotNull("Parameter 'dataContext' is null.");
      var job = (DownloadJob)dataContext;
      job.CancelDownload();
    }

    public void Process()
    {
      var cancelToken = this.cancellationTokenSource.Token;

      // Set this outside of the scanning task because there is no guarantee that the scanning task
      // will start before the downloading task.
      var isScanning = true;
      var scanReport = new MessageBuilder();
      
      Task scanningTask = Task.Factory.StartNew(
      () =>
      {
        try
        {
          while (this.feedIndexes.Count > 0)
          {
            this.FeedStartedEvent?.Invoke();
            Int32 feedIndex = this.feedIndexes.Dequeue();

            var feed = this.feedCollection[feedIndex];
            var message = "";
            if (!feed.DoScan)
            {
              message = "SKIPPING \"" + feed.Title + "\".\r\n";
              this.logController.Message(MainWindow.UiKey, message + "\r\n").Message(MainWindow.InfoKey, message);
              continue;
            }

            message = "Scanning \"" + feed.Title + "\".";
            this.logController.Message(MainWindow.UiKey, message + "\r\n").Message(MainWindow.InfoKey, message);

            Feed newFeed = null;
            try
            {
              var feedFilePath = Path.Combine(feed.Directory, "download.rss");
              newFeed = FeedFunctions.UpdateFeed(feed, feedFilePath, cancelToken);

              message = "Searching for new podcasts ... ";
              this.logController.Message(MainWindow.UiKey, message).Message(MainWindow.InfoKey, message);

              var podcastIndexes = this.BuildPodcastList(feed, newFeed);
              var feedHasNewPodcasts = (podcastIndexes.Count > 0);

              var downloadConfirmation = (!newFeed.CompleteDownloadsOnScan ? DownloadConfirmationStatus.SkipDownloading : DownloadConfirmationStatus.ContinueDownloading);
              if (feedHasNewPodcasts && downloadConfirmation != DownloadConfirmationStatus.SkipDownloading)
              {
                this.LogNewPodcastsFromFeed(this.logController.GetLogger<FileLogger>(MainWindow.InfoKey), newFeed, podcastIndexes);

                downloadConfirmation = this.podcastDownloadConfirmer.ConfirmPodcastsForDownload(feed, newFeed, podcastIndexes);
                if (downloadConfirmation == DownloadConfirmationStatus.CancelScanning)
                {
                  var feedMessage = podcastIndexes.Count + " podcasts found";
                  var logMessage = feedMessage + " (Scan cancelled).";
                  this.logController.Message(MainWindow.InfoKey, logMessage).Message(MainWindow.UiKey, feedMessage + "\r\n");
                  scanReport.Message(feedMessage + " for \"" + feed.Title + "\".");
                  this.cancellationTokenSource.Cancel();
                  break;
                }
              }

              this.LogPodcastSearchResults(feed.Title, feedHasNewPodcasts, (UInt32)podcastIndexes.Count, downloadConfirmation, scanReport);

              var podcasts = this.BuildPodcastArray(podcastIndexes, newFeed);
              this.DownloadPodcastImages(podcasts, cancelToken);

              this.FeedScanCompleted(feedIndex, newFeed);
              
              if (downloadConfirmation == DownloadConfirmationStatus.SkipDownloading)
              {
                // Feed gets updated in storage everytime a download job completes - but if nothing
                // is being downloaded then I need to update the feed 'manually' 
                this.feedCollection.UpdateFeedContent(newFeed);
                continue;
              }

              this.CreateDownloadJobs(podcasts, newFeed);
            }
            catch (OperationCanceledException)
            {
              // Do not handle here - rethrow so that operation is canceled.
              throw;
            }
            catch (Exception exception)
            {
              var exceptionMessage = String.Format("EXCEPTION thrown for \"{0}\": {1}\r\n", feed.Title, exception.Message);
              scanReport.Message(exceptionMessage);
              this.logController.Message(MainWindow.UiKey, exceptionMessage + "\r\n").Message(MainWindow.ExceptionKey, exceptionMessage);
            }
          }

          isScanning = false;
        }
        catch (Exception exception)
        {
          this.logController.Message(MainWindow.ExceptionKey, exception.Message);
        }
      }, cancelToken);

      Task downloadingTask = Task.Factory.StartNew(
      () =>
      {
        while (isScanning || downloadManager.GotIncompleteJobs)
        {
          downloadManager.StartDownloads();
          Thread.Sleep(50);

          cancelToken.ThrowIfCancellationRequested();
        }

      }, cancelToken);

      var completionTask = Task.Factory.ContinueWhenAll(new[] { scanningTask, downloadingTask },
      (tasks) =>
      {
        // Collate the file delivery status
        foreach (var deliveryLine in this.fileDeliveryLogger)
        {
          scanReport.Message(deliveryLine + "\r\n");
        }

        if (tasks[0].IsCanceled || tasks[1].IsCanceled)
        {
          scanReport.Message("\r\nCANCELLED");
          this.ScanCanceledEvent?.Invoke();
        }
        else
        {
          this.ScanCompletedEvent?.Invoke();
        }

        // Display the final scan report.
        var text = scanReport.Text;
        if (String.IsNullOrEmpty(text))
        {
          this.logController.Message(MainWindow.UiKey, "Nothing to report.");
        }
        else
        {
          this.logController.Message(MainWindow.UiKey, "Scan Report\r\n" + text);
        }
      });
      // No cancel token since we want this task to always run regardless
      // of what happens to the other tasks. Cancelling a task will stop any
      // continuation tasks from being scheduled (and hence started)
    }

    private List<Int32> BuildPodcastList(Feed oldFeed, Feed newFeed)
    {
      var podcastIndexes = new List<Int32>();
      if (oldFeed.Podcasts.Length == 0)
      {
        for (Int32 i = 0; i < newFeed.Podcasts.Length; i++)
        {
          podcastIndexes.Add(i);
        }
      }
      else
      {
        Int32 podcastIndex = 0;
        var firstPodcast = oldFeed.Podcasts[0];
        while (podcastIndex < newFeed.Podcasts.Length && !newFeed.Podcasts[podcastIndex].Equals(firstPodcast))
        {
          podcastIndexes.Add(podcastIndex);
          podcastIndex++;
        }
      }

      return podcastIndexes;
    }

    private Podcast[] BuildPodcastArray(List<Int32> podcastIndexes, Feed feed)
    {
      var podcasts = new Podcast[podcastIndexes.Count];
      podcastIndexes.Reverse();

      var index = 0;
      foreach (var podcastIndex in podcastIndexes)
      {
        podcasts[index++] = feed.Podcasts[podcastIndex];
      }

      return podcasts;
    }

    private void CreateDownloadJobs(Podcast[] podcasts, Feed feed)
    {
      foreach (var podcast in podcasts)
      {
        var job = new DownloadJob(podcast, feed, this.feedCollection);
        this.downloadManager.AddJob(job);
      }
    }

    private void DownloadPodcastImages(Podcast[] podcasts, CancellationToken cancellationToken)
    {
      if (this.imageResolver != null)
      {
        return;
      }

      this.imageResolver.ResolvePodcastImages(podcasts, cancellationToken);
    }

    private void FeedScanCompleted(Int32 feedIndex, Feed feed)
    {
      var message = String.Format("Updating \"{0}\" ... ", feed.Title);
      this.logController.Message(MainWindow.UiKey, message).Message(MainWindow.InfoKey, message);

      feed = Feed.SetUpdatedDate(DateTime.Now, feed);

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.feedCollection[feedIndex] = feed;
      });

      message = "Completed.";
      this.logController.Message(MainWindow.UiKey, message + "\r\n\r\n").Message(MainWindow.InfoKey, message);
    }

    private void LogNewPodcastsFromFeed(ILogger logger, Feed feed, List<Int32> podcastIndexes)
    {
      foreach (var podcastIndex in podcastIndexes)
      {
        var podcast = feed.Podcasts[podcastIndex];
        var message = "New podcast details. Title: '" + podcast.Title +
          "', Description: '" + podcast.Description +
          "', Publishing Date: '" + podcast.PubDate +
          "', URL: '" + podcast.URL + "'";

        if (!String.IsNullOrEmpty(podcast.ImageURL))
        {
          message += ", Image URL: '" + podcast.ImageURL + "'";
        }

        logger.Message(message + ".");
      }
    }

    private void LogPodcastSearchResults(String feedTitle, Boolean feedHasNewPodcasts, UInt32 podcastCount, DownloadConfirmationStatus downloadConfirmation, MessageBuilder scanReport)
    {
      var message = "Completed - ";
      if (!feedHasNewPodcasts)
      {
        message += "No new podcasts found.";
      }
      else
      {
        var feedMessage = podcastCount + " podcast".Pluralize(podcastCount) + " found";
        var downloadingSkippedMessage = (downloadConfirmation == DownloadConfirmationStatus.SkipDownloading ? " (Downloading skipped)" : String.Empty);
        message += feedMessage + downloadingSkippedMessage + ".";
        scanReport.Message(feedMessage + " for \"" + feedTitle + "\"" + downloadingSkippedMessage + ".\r\n");
      }

      this.logController.Message(MainWindow.UiKey, message + "\r\n").Message(MainWindow.InfoKey, message);
    }
    #endregion
  }
}