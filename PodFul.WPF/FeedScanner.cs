
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using System.Windows;
  using Library;

  public class FeedScanner : FeedProcessor
  {
    public FeedScanner(
      FeedCollection feedCollection,
      Queue<Int32> feedIndexes,
      IImageResolver imageResolver,
      IFileDeliverer fileDeliverer,
      ILogger log) : base(feedCollection, feedIndexes, imageResolver, fileDeliverer, log)
    {
    }

    public override void Process()
    {
      var feedTotal = this.indexes.Count;
      var title = "Scanning " + feedTotal + " feed" + (feedTotal != 1 ? "s" : String.Empty);
      this.SetWindowTitleEvent?.Invoke(title);

      var cancelToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(cancelToken);

      Task task = Task.Factory.StartNew(() =>
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
            return;
          }

          var feed = this.feedCollection.Feeds[feedIndex];

          this.log.Message("Scanning \"" + feed.Title + "\".");

          Feed newFeed = null;
          try
          {
            newFeed = FeedFunctions.UpdateFeed(feed, this.imageResolver);

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
              var podcastQueue = new Queue<Int32>(podcastIndexes);
              var isCancelled = !podcastDownload.Download(feed.Directory, newFeed.Podcasts, podcastQueue);

              Application.Current.Dispatcher.Invoke(() =>
              {
                this.feedCollection.UpdateFeed(feedIndex, newFeed);
              });

              if (isCancelled)
              {
                this.log.Message("\r\nCANCELLED");
                return;
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

        this.SetCancelButtonStateEvent?.Invoke(false);

      }, cancelToken);
    }
  }
}
