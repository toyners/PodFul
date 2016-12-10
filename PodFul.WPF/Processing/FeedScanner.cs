
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using Jabberwocky.Toolkit.Object;
  using Jabberwocky.Toolkit.String;
  using Library;
  using Logging;
  using Miscellaneous;
  using Processing;

  /// <summary>
  /// Scans a list of feeds and collects a list of podcasts to be downloaded. Feeds with new podcasts are updated.
  /// </summary>
  public class FeedScanner
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private DownloadManager downloadManager;

    private FeedCollection feedCollection;

    private IFileDeliverer fileDeliverer;

    private FileDeliveryLogger fileDeliveryLogger;

    private IImageResolver imageResolver;

    private Queue<Int32> indexes;

    private ILogger logger;

    private IPodcastDownloadConfirmer podcastDownloadConfirmer;
    #endregion

    #region Construction
    public FeedScanner(
      FeedCollection feedCollection,
      Queue<Int32> feedIndexes,
      IImageResolver imageResolver,
      IFileDeliverer fileDeliverer,
      FileDeliveryLogger fileDeliveryLogger,
      ILogger logger,
      IPodcastDownloadConfirmer podcastDownloadConfirmer,
      DownloadManager downloadManager)
    {
      this.feedCollection = feedCollection;
      this.indexes = feedIndexes;
      this.imageResolver = imageResolver;
      this.fileDeliverer = fileDeliverer;
      this.fileDeliveryLogger = fileDeliveryLogger;
      this.logger = logger;
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
      // will start before the downloading taks.
      var isScanning = true;
      String scanReport = null;
      
      Task scanningTask = Task.Factory.StartNew(
      () =>
      {
        while (indexes.Count > 0)
        {
          Int32 feedIndex = indexes.Dequeue();

          title = "Scanning " + (feedTotal - indexes.Count) + " of " + feedTotal + " feed".Pluralize(feedTotal);
          this.SetWindowTitleEvent?.Invoke(title);

          var feed = this.feedCollection.Feeds[feedIndex];

          this.logger.Message("Scanning \"" + feed.Title + "\".");

          Feed newFeed = null;
          try
          {
            newFeed = FeedFunctions.UpdateFeed(feed, this.imageResolver, cancelToken);

            this.logger.Message("Comparing podcasts ... ", false);

            var podcastIndexes = this.BuildPodcastList(feed, newFeed);
            var updateFeed = (podcastIndexes.Count > 0);

            var downloadConfirmation = this.podcastDownloadConfirmer.ConfirmPodcastsForDownload(feed, newFeed, podcastIndexes);
            if (downloadConfirmation == DownloadConfirmationStatus.CancelScanning)
            {
              var feedReport = podcastIndexes.Count + " podcasts found";
              this.logger.Message(feedReport + " (Scan cancelled).\r\n");
              scanReport += feedReport + " for \"" + feed.Title + "\".";
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
              var feedReport = podcastIndexes.Count + " podcast" +
                (podcastIndexes.Count != 1 ? "s" : String.Empty) + " found";
              var downloadingReport = (downloadConfirmation == DownloadConfirmationStatus.ContinueDownloading ? String.Empty : " (Downloading skipped)");
              message += feedReport + downloadingReport + ".";
              scanReport += feedReport + " for \"" + feed.Title + "\"" + downloadingReport + ".\r\n";
            }

            this.logger.Message(message);

            this.logger.Message(String.Format("Updating \"{0}\" ... ", feed.Title), false);

            this.FeedScanCompleted(feedIndex, newFeed);

            this.logger.Message(String.Empty);

            if (downloadConfirmation == DownloadConfirmationStatus.SkipDownloading)
            {
              continue;
            }

            podcastIndexes.Reverse();

            foreach (var index in podcastIndexes)
            {
              var podcast = newFeed.Podcasts[index];
              var job = new DownloadJob(podcast, newFeed, this.feedCollection, this.fileDeliverer, this.imageResolver);
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
            scanReport += exceptionReport;
            this.logger.Message(exceptionReport);
          }
        }

        isScanning = false;

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
          scanReport += deliveryLine + "\r\n";
        }

        if (tasks[0].IsCanceled || tasks[1].IsCanceled)
        {
          scanReport += "\r\nCANCELLED";
        }

        // Display the final scan report.
        if (scanReport == null)
        {
          this.logger.Message("Nothing to report.");
        }
        else
        {
          this.logger.Message("Scan Report\r\n" + scanReport);
        }

        this.ScanCompletedEvent?.Invoke();
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

    private void FeedScanCompleted(Int32 feedIndex, Feed feed)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.feedCollection.UpdateFeed(feedIndex, feed);
        this.logger.Message("Completed.");
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
