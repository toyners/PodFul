
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
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
    private IDownloadFunctionality downloadManager;
    private IFeedCollection feedCollection;
    private Queue<Int32> feedIndexes;
    private IFileDeliveryLogger fileDeliveryLogger;
    private IImageResolver imageResolver;
    private ILogController logController;
    #endregion

    #region Construction
    public FeedScanner(
      IFeedCollection feedCollection,
      Queue<Int32> feedIndexes,
      IImageResolver imageResolver,
      IFileDeliveryLogger fileDeliveryLogger,
      ILogController logController,
      IDownloadFunctionality downloadManager)
    {
      this.feedCollection = feedCollection;
      this.feedIndexes = feedIndexes;
      this.imageResolver = imageResolver;
      this.fileDeliveryLogger = fileDeliveryLogger;
      this.logController = logController;
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
    public static FeedScanner Create(FeedCollection feedCollection, Queue<Int32> feedIndexes, IImageResolver imageResolver, IFileDeliveryLogger fileDeliveryLogger, ILogController logController, IDownloadManager downloadManager)
    {
      fileDeliveryLogger.Clear();
      var feedScanner = new FeedScanner(feedCollection, feedIndexes, imageResolver, fileDeliveryLogger, logController, downloadManager);
      return feedScanner;
    }

    public void CancelAll()
    {
      this.cancellationTokenSource.Cancel();
      this.downloadManager.CancelAllJobs();
    }

    public void CancelDownload(DownloadJob job)
    {
      this.downloadManager.CancelJob(job);
    }

    public void Process()
    {
      var cancelToken = this.cancellationTokenSource.Token;

      // Set this outside of the scanning task because there is no guarantee that the scanning task
      // will start before the downloading task.
      var isScanning = true;
      var scanReport = new MessageBuilder();
      IPodcastDownloadConfirmer podcastDownloadConfirmer = new PodcastDownloadConfirmer();

      Task scanningTask = Task.Factory.StartNew(
      () =>
      {
        try
        {
          while (this.feedIndexes.Count > 0)
          {
            cancelToken.ThrowIfCancellationRequested();

            this.FeedStartedEvent?.Invoke();
            Int32 feedIndex = this.feedIndexes.Dequeue();

            var feed = this.feedCollection[feedIndex];
            var message = "";
            if (!feed.DoScan)
            {
              message = "SKIPPING \"" + feed.Title + "\".\r\n";
              this.logController.Message(LoggerKeys.UiKey, message + "\r\n").Message(LoggerKeys.InfoKey, message);
              continue;
            }

            message = "Scanning \"" + feed.Title + "\".";
            this.logController.Message(LoggerKeys.UiKey, message + "\r\n").Message(LoggerKeys.InfoKey, message);

            Feed newFeed = null;
            try
            {
              var feedFilePath = Path.Combine(feed.Directory, "download.rss");
              newFeed = FeedFunctions.UpdateFeed(feed, feedFilePath, cancelToken);

              if (this.imageResolver != null)
              {
                newFeed = this.imageResolver.ResolveFeedImage(newFeed);
              }

              // Creating the new feed takes a while - check for cancellation before furthering processing.
              cancelToken.ThrowIfCancellationRequested();

              message = "Searching for new podcasts ... ";
              this.logController.Message(LoggerKeys.UiKey, message).Message(LoggerKeys.InfoKey, message);

              var podcastIndexes = this.BuildPodcastList(feed, newFeed);
              var feedHasNewPodcasts = (podcastIndexes.Count > 0);

              var downloadConfirmation = (!newFeed.CompleteDownloadsOnScan ? DownloadConfirmationStatus.SkipDownloading : DownloadConfirmationStatus.ContinueDownloading);
              if (feedHasNewPodcasts && downloadConfirmation != DownloadConfirmationStatus.SkipDownloading)
              {
                this.LogNewPodcastsFromFeed(this.logController.GetLogger<FileLogger>(LoggerKeys.InfoKey), newFeed, podcastIndexes);

                podcastDownloadConfirmer.ConfirmDownloadThreshold = feed.ConfirmDownloadThreshold;
                downloadConfirmation = podcastDownloadConfirmer.ConfirmPodcastsForDownload(feed, newFeed, podcastIndexes);
                if (downloadConfirmation == DownloadConfirmationStatus.CancelScanning)
                {
                  var feedMessage = podcastIndexes.Count + " podcasts found";
                  var logMessage = feedMessage + " (Scan cancelled).";
                  this.logController.Message(LoggerKeys.InfoKey, logMessage).Message(LoggerKeys.UiKey, feedMessage + "\r\n");
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
              this.logController.Message(LoggerKeys.UiKey, exceptionMessage + "\r\n").Message(LoggerKeys.ExceptionKey, exceptionMessage);
            }
          }
        }
        catch (OperationCanceledException ocException)
        {
          this.logController.Message(LoggerKeys.ExceptionKey, ocException.Message);
          throw; // Rethrow so that the task state is correctly set (i.e. IsCanceled = true)
        }
        catch (Exception exception)
        {
          this.logController.Message(LoggerKeys.ExceptionKey, exception.Message);
        }
        finally
        {
          isScanning = false;
        }

      }, cancelToken);

      Task downloadingTask = Task.Factory.StartNew(
      () =>
      {
        while (isScanning || this.downloadManager.GotIncompleteJobs)
        {
          this.downloadManager.StartWaitingJobs();
          Thread.Sleep(50);

          if (!cancelToken.IsCancellationRequested)
          { 
            cancelToken.ThrowIfCancellationRequested();
          }
        }

      }, cancelToken);

      var completionTask = Task.Factory.ContinueWhenAll(new[] { scanningTask, downloadingTask },
      (tasks) =>
      {
        if (this.fileDeliveryLogger != null)
        {
          // Display the file delivery status messages
          foreach (var deliveryLine in this.fileDeliveryLogger)
          {
            scanReport.Message(deliveryLine + "\r\n");
          }
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
          this.logController.Message(LoggerKeys.UiKey, "Nothing to report.");
        }
        else
        {
          this.logController.Message(LoggerKeys.UiKey, "Scan Report\r\n" + text);
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
      if (this.imageResolver == null)
      {
        return;
      }

      this.imageResolver.ResolvePodcastImages(podcasts, cancellationToken);
    }

    private void FeedScanCompleted(Int32 feedIndex, Feed feed)
    {
      var message = String.Format("Updating \"{0}\" ... ", feed.Title);
      this.logController.Message(LoggerKeys.UiKey, message).Message(LoggerKeys.InfoKey, message);

      feed = Feed.SetUpdatedDate(DateTime.Now, feed);

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.feedCollection[feedIndex] = feed;
      });

      // Now update the feed to storage for real.
      this.feedCollection.UpdateFeedContent(feed);

      message = "Completed.";
      this.logController.Message(LoggerKeys.UiKey, message + "\r\n\r\n").Message(LoggerKeys.InfoKey, message);
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

      this.logController.Message(LoggerKeys.UiKey, message + "\r\n").Message(LoggerKeys.InfoKey, message);
    }
    #endregion
  }
}