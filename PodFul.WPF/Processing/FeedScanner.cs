
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using Jabberwocky.Toolkit.String;
  using Library;
  using Processing;

  /// <summary>
  /// Scans a list of feeds and collects a list of podcasts to be downloaded. Feeds with new podcasts are updated.
  /// </summary>
  public class FeedScanner : FeedProcessor
  {
    private DownloadManager downloadManager;

    #region Construction
    public FeedScanner(
      FeedCollection feedCollection,
      Queue<Int32> feedIndexes,
      IImageResolver imageResolver,
      IFileDeliverer fileDeliverer,
      ILogger log) : base(feedCollection, feedIndexes, imageResolver, fileDeliverer, log)
    {
    }
    #endregion

    #region Properties
    #endregion

    #region Methods
    public override void Process()
    {
      var feedTotal = (UInt32)this.indexes.Count;
      var title = "Scanning " + feedTotal + " feed".Pluralize(feedTotal);
      this.SetWindowTitleEvent?.Invoke(title);

      var cancelToken = this.cancellationTokenSource.Token;

      Queue<PodcastMonitor> jobs = new Queue<PodcastMonitor>();
      var isScanning = true;

      Task scanningTask = Task.Factory.StartNew(() =>
      {
        this.SetCancelButtonStateEvent?.Invoke(true);
        var podcastIndexes = new List<Int32>();
        String scanReport = null;

        while (indexes.Count > 0)
        {
          Int32 feedIndex = indexes.Dequeue();

          title = "Scanning " + (feedTotal - indexes.Count) + " of " + feedTotal + " feed" + (feedTotal != 1 ? "s" : String.Empty);
          this.SetWindowTitleEvent?.Invoke(title);

          if (this.cancellationTokenSource.IsCancellationRequested)
          {
            this.log.Message("\r\nCANCELLED");
            this.SetCancelButtonStateEvent?.Invoke(false);
            return;
          }

          var feed = this.feedCollection.Feeds[feedIndex];

          this.log.Message("Scanning \"" + feed.Title + "\".");

          Feed newFeed = null;
          try
          {
            newFeed = FeedFunctions.UpdateFeed(feed, this.imageResolver, cancelToken);

            if (this.cancellationTokenSource.IsCancellationRequested)
            {
              this.log.Message("CANCELLED");
              this.SetCancelButtonStateEvent?.Invoke(false);
              return;
            }

            this.log.Message("Comparing podcasts ... ", false);

            podcastIndexes.Clear();
            if (feed.Podcasts.Length == 0)
            {
              for (Int32 i = 0; i < newFeed.Podcasts.Length; i++)
              {
                podcastIndexes.Add(i);
              }
            }
            else
            {
              Int32 podcastIndex = 0;
              var firstPodcast = feed.Podcasts[0];
              while (podcastIndex < newFeed.Podcasts.Length && !newFeed.Podcasts[podcastIndex].Equals(firstPodcast))
              {
                podcastIndexes.Add(podcastIndex);
                podcastIndex++;
              }
            }

            Boolean downloadPodcasts = (podcastIndexes.Count > 0);
            if (podcastIndexes.Count > 5)
            {
              var text = String.Format("{0} new podcasts found during feed scan.\r\n\r\nYes to continue with downloading.\r\nNo to skip downloading (feed will still be updated).\r\nCancel to stop scanning (feed will not be updated).", podcastIndexes.Count);
              var continuingDownloading = MessageBox.Show(text, "Multiple podcasts found.", MessageBoxButton.YesNoCancel);
              if (continuingDownloading == MessageBoxResult.Cancel)
              {
                var feedReport = podcastIndexes.Count + " podcasts found";
                this.log.Message(feedReport + " (Scan cancelled).\r\n");
                scanReport += feedReport + " for \"" + feed.Title + "\" (Scan cancelled).";
                break;
              }

              if (continuingDownloading == MessageBoxResult.No)
              {
                downloadPodcasts = false;
              }
            }

            String message = "Complete - ";
            if (podcastIndexes.Count == 0)
            {
              message += "No new podcasts found.";
            }
            else
            {
              newFeed = Feed.SetUpdatedDate(DateTime.Now, newFeed);
              var feedReport = podcastIndexes.Count + " podcast" +
                (podcastIndexes.Count != 1 ? "s" : String.Empty) + " found";
              var downloadingReport = (downloadPodcasts ? String.Empty : " (Downloading skipped)");
              message += feedReport + downloadingReport + ".";
              scanReport += feedReport + " for \"" + feed.Title + "\"" + downloadingReport + ".\r\n";
            }

            this.log.Message(message);

            this.log.Message(String.Format("Updating \"{0}\" ... ", feed.Title), false);

            Application.Current.Dispatcher.Invoke(() =>
            {
              this.feedCollection.UpdateFeed(feedIndex, newFeed);
              this.log.Message("Completed.");
            });

            if (downloadPodcasts)
            {
              podcastIndexes.Reverse();

              foreach (var index in podcastIndexes)
              {
                var podcast = newFeed.Podcasts[index];
                var podcastMonitor = new PodcastMonitor(podcast, podcast.FileDetails.FileSize, newFeed.Directory);
                jobs.Enqueue(podcastMonitor);
              }
            }

            this.log.Message(String.Empty);

          }
          catch (Exception exception)
          {
            var exceptionReport = String.Format("EXCEPTION thrown for \"{0}\": {1}\r\n", feed.Title, exception.Message);
            scanReport += exceptionReport;
            this.log.Message(exceptionReport);
          }
        }

        // Display the final scan report.
        if (scanReport == null)
        {
          this.log.Message("Nothing to report.");
        }
        else
        {
          this.log.Message("Scan Report\r\n" + scanReport);
        }

        isScanning = false;
        this.SetCancelButtonStateEvent?.Invoke(false);

      }, cancelToken);

      Task downloadingTask = Task.Factory.StartNew(() =>
      {
        while (isScanning || jobs.Count > 0 || downloadManager.IsProcessingJob)
        {
          Application.Current.Dispatcher.Invoke(() =>
          {
            // Add to podcast list control
            //this.feedCollection.UpdateFeed(feedIndex, newFeed);
            //this.log.Message("Completed.");
          });

          Thread.Sleep(30); // Sleep while the UI thread completes adding the new job to the list.

          var job = jobs.Dequeue();
          downloadManager.AddJob(job);
        }
      }, cancelToken);
    }
    #endregion
  }
}
