
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

    private FeedCollection feedCollection;

    private MessagePool fileDeliveryLogger;

    private IImageResolver imageResolver;

    private Queue<Int32> indexes;

    private ILogController logController;

    private IPodcastDownloadConfirmer podcastDownloadConfirmer;
    #endregion

    #region Construction
    public FeedScanner(
      FeedCollection feedCollection,
      Queue<Int32> feedIndexes,
      IImageResolver imageResolver,
      MessagePool fileDeliveryLogger,
      ILogController logController,
      IPodcastDownloadConfirmer podcastDownloadConfirmer,
      DownloadManager downloadManager)
    {
      this.feedCollection = feedCollection;
      this.indexes = feedIndexes;
      this.imageResolver = imageResolver;
      this.fileDeliveryLogger = fileDeliveryLogger;
      this.logController = logController;
      this.podcastDownloadConfirmer = podcastDownloadConfirmer;
      this.downloadManager = downloadManager;
      this.downloadManager.JobStartedEvent += this.UpdateCounts;
      this.downloadManager.JobStartedEvent += this.JobStartedEventHandler;
      this.downloadManager.JobFinishedEvent += this.UpdateCounts;
    }
    #endregion

    #region Properties
    public ObservableCollection<DownloadJob> Jobs { get { return this.downloadManager.Jobs; } }
    #endregion

    #region Events
    public Action<String> SetWindowTitleEvent;

    public event Action ScanCompletedEvent;

    public event Action<Int32, Int32, Int32, Int32, Int32> UpdateCountsEvent;

    public event Action<DownloadJob> JobStartedEvent;
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
      var podcast = (DownloadJob)dataContext;
      podcast.CancelDownload();
    }

    public void Process()
    {
      var feedTotal = (UInt32)this.indexes.Count;
      var title = "Scanning " + feedTotal + " feed".Pluralize(feedTotal);
      this.SetWindowTitleEvent?.Invoke(title);

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
          var combinedLogger = this.logController.GetLogger(MainWindow.CombinedKey);
          while (indexes.Count > 0)
          {
            Int32 feedIndex = indexes.Dequeue();

            title = "Scanning " + (feedTotal - indexes.Count) + " of " + feedTotal + " feed".Pluralize(feedTotal);
            this.SetWindowTitleEvent?.Invoke(title);

            var feed = this.feedCollection.Feeds[feedIndex];

            combinedLogger.Message("Scanning \"" + feed.Title + "\".");

            Feed newFeed = null;
            try
            {
              var feedFilePath = Path.Combine(feed.Directory, "download.rss");
              newFeed = FeedFunctions.UpdateFeedFromFile(feed, feedFilePath, this.imageResolver, cancelToken);

              combinedLogger.Message("Comparing podcasts ... ");

              var podcastIndexes = this.BuildPodcastList(feed, newFeed);
              var updateFeed = (podcastIndexes.Count > 0);

              if (updateFeed)
              {
                this.LogNewPodcastsFromFeed(this.logController.GetLogger<FileLogger>(MainWindow.InfoKey), newFeed, podcastIndexes);
              }

              var downloadConfirmation = this.podcastDownloadConfirmer.ConfirmPodcastsForDownload(feed, newFeed, podcastIndexes);
              if (downloadConfirmation == DownloadConfirmationStatus.CancelScanning)
              {
                var feedReport = podcastIndexes.Count + " podcasts found";
                combinedLogger.Message(feedReport + " (Scan cancelled).\r\n");
                scanReport.Message(feedReport + " for \"" + feed.Title + "\".");
                this.cancellationTokenSource.Cancel();

                break;
              }

              if (updateFeed)
              {
                newFeed = Feed.SetUpdatedDate(DateTime.Now, newFeed);
              }

              String message = "Complete - ";
              if (podcastIndexes.Count == 0)
              {
                message += "No new podcasts found.";
              }
              else
              {
                var feedReport = podcastIndexes.Count + " podcast".Pluralize((UInt32)podcastIndexes.Count) + " found";
                var downloadingReport = (downloadConfirmation == DownloadConfirmationStatus.ContinueDownloading ? String.Empty : " (Downloading skipped)");
                message += feedReport + downloadingReport + ".";
                scanReport.Message(feedReport + " for \"" + feed.Title + "\"" + downloadingReport + ".\r\n");
              }

              combinedLogger.Message(message);

              combinedLogger.Message(String.Format("Updating \"{0}\" ... ", feed.Title));

              this.FeedScanCompleted(feedIndex, newFeed);

              combinedLogger.Message(String.Empty);

              if (downloadConfirmation == DownloadConfirmationStatus.SkipDownloading)
              {
                continue;
              }

              podcastIndexes.Reverse();

              foreach (var index in podcastIndexes)
              {
                var podcast = newFeed.Podcasts[index];
                var job = new DownloadJob(podcast, newFeed, this.feedCollection, this.imageResolver);
                this.downloadManager.AddJob(job);
              }
            }
            catch (OperationCanceledException)
            {
              // Do not handle here - rethrow so that operation is canceled.
              throw;
            }
            catch (Exception exception)
            {
              var exceptionReport = String.Format("EXCEPTION thrown for \"{0}\": {1}\r\n", feed.Title, exception.Message);
              scanReport.Message(exceptionReport);
              combinedLogger.Message(exceptionReport);
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
          this.SetWindowTitleEvent?.Invoke(title + " - CANCELLED");
        }
        else
        {
          this.SetWindowTitleEvent?.Invoke(title + " - COMPLETED");
        }

        // Display the final scan report.
        var text = scanReport.Text;
        if (text == null)
        {
          this.logController.Message(MainWindow.CombinedKey, "Nothing to report.");
        }
        else
        {
          this.logController.Message(MainWindow.CombinedKey, "Scan Report\r\n" + text);
        }

        this.ScanCompletedEvent?.Invoke();
      });
      // No cancel token since we want this task to always run regardless
      // of what happens to the other tasks. Cancelling a task will stop any
      // continuation tasks from being scheduled (and hence started)
    }

    private void LogNewPodcastsFromFeed(ILogger logger, Feed feed, List<Int32> podcastIndexes)
    {
      logger.Message(podcastIndexes.Count + " podcast".Pluralize((UInt32)podcastIndexes.Count) + " found for feed '" + feed.Title + "'.\r\n");
      foreach (var podcastIndex in podcastIndexes)
      {
        var podcast = feed.Podcasts[podcastIndex];
        logger.Message("New podcast details. Title: '" + podcast.Title + "', Description: '" + podcast.Description + 
          "', Publishing Date: '" + podcast.PubDate + "', URL: '" + podcast.URL + 
          "', Image URL: '" + podcast.ImageURL + "'.\r\n");
      }
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

    private void FeedScanCompleted(Int32 feedIndex, Feed feed)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.feedCollection.UpdateFeed(feedIndex, feed);
        this.logController.Message(MainWindow.CombinedKey, "Completed.");
      });
    }

    private void JobStartedEventHandler(DownloadJob job)
    {
      this.JobStartedEvent?.Invoke(job);
    }

    private void UpdateCounts(DownloadJob job)
    {
      this.UpdateCountsEvent?.Invoke(this.downloadManager.WaitingJobsCount, 
        this.downloadManager.ProcessingJobsCount,
        this.downloadManager.CompletedJobsCount,
        this.downloadManager.CancelledJobsCount,
        this.downloadManager.FailedJobsCount);
    }
    #endregion
  }
}
