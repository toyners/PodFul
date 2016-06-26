
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using Library;

  public class FeedScanner : IFeedProcessor
  {
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private IFeedStorage feedStorage;
    private Queue<Int32> feedIndexes;
    private IImageResolver imageResolver;
    private IFileDeliverer fileDeliverer;
    private ILogger log;

    private Int64 fileSize;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;
    private Boolean fileSizeNotKnown;
    private String progressSizeLabel;

    public Action<String> SetWindowTitleEvent;

    public Action<Boolean> SetCancelButtonStateEvent;

    public Action<String, Boolean> InitialiseProgressEvent;

    public Action ResetProgressEvent;

    public Action<String, Int32> SetProgressEvent;

    public FeedScanner(
      IFeedStorage feedStorage,
      Queue<Int32> feedIndexes,
      IImageResolver imageResolver,
      IFileDeliverer fileDeliverer,
      ILogger log)
    {
      this.feedStorage = feedStorage;
      this.feedIndexes = feedIndexes;
      this.fileDeliverer = fileDeliverer;
      this.imageResolver = imageResolver;
      this.log = log;
    }

    public void Cancel()
    {
      this.cancellationTokenSource.Cancel();
    }

    public void Process()
    {
      Feed[] feeds = this.feedStorage.Feeds;

      var feedTotal = this.feedIndexes.Count;
      var title = "Scanning " + feedTotal + " feed" + (feedTotal != 1 ? "s" : String.Empty);
      this.SetWindowTitleEvent?.Invoke(title);

      var cancelToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(cancelToken);

      Task task = Task.Factory.StartNew(() =>
      {
        this.SetCancelButtonStateEvent?.Invoke(true);
        var podcastIndexes = new Queue<Int32>();
        String scanReport = null;

        while (feedIndexes.Count > 0)
        {
          Int32 feedIndex = feedIndexes.Dequeue();

          title = "Scanning " + (feedTotal - feedIndexes.Count) + " of " + feedTotal + " feed" + (feedTotal != 1 ? "s" : String.Empty);
          this.SetWindowTitleEvent?.Invoke(title);

          if (this.cancellationTokenSource.IsCancellationRequested)
          {
            this.log.Message("\r\nCANCELLED");
            return;
          }

          var feed = feeds[feedIndex];

          this.log.Message("Scanning \"" + feed.Title + "\".");

          Feed newFeed = null;
          try
          {
            newFeed = FeedFunctions.UpdateFeed(feed);
          }
          catch (Exception exception)
          {
            var exceptionReport = String.Format("EXCEPTION thrown for \"{0}\": {1}\r\n", feed.Title, exception.Message);
            scanReport += exceptionReport;
            this.log.Message(exceptionReport);
            continue;
          }

          // Resolve the feed image.
          var imageFileName = imageResolver.GetName(newFeed.ImageFileName);
          newFeed = Feed.SetImageFileName(newFeed, imageFileName);

          this.log.Message("Comparing podcasts ... ", false);

          Int32 podcastIndex = 0;
          podcastIndexes.Clear();
          var firstPodcast = feed.Podcasts[0];
          while (podcastIndex < newFeed.Podcasts.Length && !newFeed.Podcasts[podcastIndex].Equals(firstPodcast))
          {
            podcastIndexes.Enqueue(podcastIndex);
            podcastIndex++;
          }

          Boolean downloadPodcasts = true;
          if (podcastIndexes.Count > 5)
          {
            var text = String.Format("{0} new podcasts found during feed scan.\r\n\r\nYes to continue with downloading.\r\nNo to skip downloading (feed will still be updated).\r\nCancel to stop scanning.", podcastIndexes.Count);
            var continuingDownloading = MessageBox.Show(text, "Multiple podcasts found.", MessageBoxButton.YesNoCancel);
            if (continuingDownloading == MessageBoxResult.Cancel)
            {
              var feedReport = podcastIndex + " podcasts found";
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
          if (podcastIndex == 0)
          {
            message += "No new podcasts found.";
          }
          else
          {
            var feedReport = podcastIndex + " podcast" +
              (podcastIndex != 1 ? "s" : String.Empty) + " found";
            var downloadingReport = (downloadPodcasts ? String.Empty : " (Downloading skipped)");
            message += feedReport + downloadingReport + ".";
            scanReport += feedReport + " for \"" + feed.Title + "\"" + downloadingReport + ".\r\n";
          }

          this.log.Message(message);

          this.log.Message(String.Format("Updating \"{0}\" ... ", feed.Title), false);
          feedStorage.Update(newFeed);
          this.log.Message("Completed.");

          feeds[feedIndex] = newFeed;

          if (downloadPodcasts && !podcastDownload.Download(feed.Directory, newFeed.Podcasts, podcastIndexes))
          {
            this.log.Message("\r\nCANCELLED");
            return;
          }

          this.log.Message(String.Empty);
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

    private PodcastDownload InitialisePodcastDownload(CancellationToken cancelToken)
    {
      var podcastDownload = new PodcastDownload(cancelToken, this.UpdateProgessEventHandler);

      podcastDownload.OnBeforeDownload += (podcast) =>
      {
        this.fileSize = podcast.FileSize;
        this.percentageStepSize = this.fileSize / 100;
        this.downloadedSize = 0;
        this.InitialiseProgress(podcast.FileSize);
        this.log.Message(String.Format("Downloading \"{0}\" ... ", podcast.Title), false);
      };

      podcastDownload.OnSuccessfulDownload += (podcast, filePath) =>
      {
        this.log.Message("Completed.");
        this.fileDeliverer.Deliver(podcast, filePath);
      };

      podcastDownload.OnException += (podcast, exception) =>
      {
        Exception e = exception;
        if (exception is AggregateException)
        {
          e = ((AggregateException)exception).Flatten();
        }

        if (e.InnerException != null)
        {
          e = e.InnerException;
        }

        this.log.Exception(e.Message);
      };

      podcastDownload.OnFinish += () => this.ResetProgressEvent?.Invoke();

      return podcastDownload;
    }

    private void InitialiseProgress(Int64 expectedFileSize = -1)
    {
      this.fileSizeNotKnown = (expectedFileSize == 0);
      String progressSize;
      if (expectedFileSize > 0)
      {
        var total = expectedFileSize / 1048576.0;
        this.progressSizeLabel = " / " + total.ToString("0.00") + "Mb";
        progressSize = "0.00" + this.progressSizeLabel;
      }
      else
      {
        this.progressSizeLabel = "Mb";
        progressSize = "0.00" + this.progressSizeLabel;
      }

      this.InitialiseProgressEvent?.Invoke(progressSize, this.fileSizeNotKnown);
    }

    private void UpdateProgessEventHandler(Int32 bytesWrittenToFile)
    {
      this.downloadedSize += bytesWrittenToFile;
      var downloadedSizeInMb = this.downloadedSize / 1048576.0;

      Int64 value = 100;
      if (this.downloadedSize < this.fileSize)
      {
        value = this.downloadedSize / this.percentageStepSize;
      }

      var text = downloadedSizeInMb.ToString("0.00") + this.progressSizeLabel;

      this.SetProgressEvent?.Invoke(text, (Int32)value);
    }
  }
}
