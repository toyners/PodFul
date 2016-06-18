
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for ProcessingWindow.xaml
  /// </summary>
  public partial class ProcessingWindow : Window
  {
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private TaskScheduler mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private Int64 fileSize;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;
    private Boolean fileSizeNotKnown;
    private String progressSizeLabel;

    public ProcessingWindow(IFeedStorage feedStorage, Queue<Int32> feedIndexes, IImageResolver imageResolver, IFileDeliverer fileDeliverer)
    {
      InitializeComponent();

      Feed[] feeds = feedStorage.Feeds;

      var feedTotal = feedIndexes.Count;
      this.Title = "Scanning " + feedTotal + " feed" + (feedTotal != 1 ? "s" : String.Empty);

      var cancelToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(true, cancelToken, fileDeliverer);

      Task task = Task.Factory.StartNew(() =>
      {
        this.SetStateOfCancelButton(true);
        var podcastIndexes = new Queue<Int32>();
        String scanReport = null;

        while (feedIndexes.Count > 0)
        {
          Int32 feedIndex = feedIndexes.Dequeue();

          DisplayTitleForScanning(feedTotal - feedIndexes.Count, feedTotal);

          if (this.cancellationTokenSource.IsCancellationRequested)
          {
            this.PostMessage("\r\nCANCELLED");
            return;
          }

          var feed = feeds[feedIndex];

          this.PostMessage("Scanning \"" + feed.Title + "\".");

          Feed newFeed = null;
          try
          {
            newFeed = FeedFunctions.CreateFeed(feed.URL, feed.Directory);
          }
          catch (Exception exception)
          {
            var exceptionReport = String.Format("EXCEPTION thrown for \"{0}\": {1}\r\n", feed.Title, exception.Message);
            scanReport += exceptionReport;
            this.PostMessage(exceptionReport);
            continue;
          }

          // Resolve the feed image.
          var imageFileName = imageResolver.GetName(newFeed.ImageFileName);
          newFeed = Feed.SetImageFileName(newFeed, imageFileName);

          this.PostMessage("Comparing podcasts ... ", false);

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
              this.PostMessage(feedReport + " (Scan cancelled).\r\n");
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

          newFeed = this.SynchroniseFeed(newFeed, podcastIndex, feed);
          this.PostMessage(message);

          this.PostMessage(String.Format("Updating \"{0}\" ... ", feed.Title), false);
          feedStorage.Update(newFeed);
          this.PostMessage("Completed.");

          feeds[feedIndex] = newFeed;

          if (downloadPodcasts && !podcastDownload.Download(feed.Directory, newFeed.Podcasts, podcastIndexes))
          {
            this.PostMessage("\r\nCANCELLED");
            return;
          }

          this.PostMessage(String.Empty);
        }

        // Display the final scan report.
        if (scanReport == null)
        {
          this.PostMessage("Nothing to report.");
        }
        else
        {
          this.PostMessage("Scan Report\r\n" + scanReport);
        }

        this.SetStateOfCancelButton(false);

      }, cancelToken);
    }

    public ProcessingWindow(IFeedStorage feedStorage, Feed feed, Queue<Int32> podcastIndexes, IFileDeliverer fileDeliverer)
    {
      InitializeComponent();

      this.Title = "Downloading " + podcastIndexes.Count + " podcast" + (podcastIndexes.Count != 1 ? "s" : String.Empty);

      var cancelToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(false, cancelToken, fileDeliverer);

      Task task = Task.Factory.StartNew(() =>
      {
        this.SetStateOfCancelButton(true);
        if (podcastDownload.Download(feed.Directory, feed.Podcasts, podcastIndexes))
        {
          feedStorage.Update(feed);
        }

        this.SetStateOfCancelButton(false);
      }, cancelToken);
    }

    private void Cancel_Click(Object sender, RoutedEventArgs e)
    {
      this.cancellationTokenSource.Cancel();
    }

    private void DisplayTitleForScanning(Int32 number, Int32 count)
    {
      new Task(() =>
      {
        this.Title = "Scanning " + number + " of " + count + " feed" + (count != 1 ? "s" : String.Empty);

      }).Start(this.mainTaskScheduler);
    }


    private PodcastDownload InitialisePodcastDownload(Boolean isScanning, CancellationToken cancelToken, IFileDeliverer fileDeliverer)
    {
      var podcastDownload = new PodcastDownload(cancelToken, this.UpdateProgessEventHandler);

      podcastDownload.OnBeforeDownload += (podcast) =>
      {
        this.fileSize = podcast.FileSize;
        this.percentageStepSize = this.fileSize / 100;
        this.downloadedSize = 0;
        this.ResetProgressBar(podcast.FileSize);
        this.PostMessage(String.Format("Downloading \"{0}\" ... ", podcast.Title), false);
      };

      if (isScanning)
      {
        podcastDownload.OnSuccessfulDownload += (podcast, filePath) =>
        {
          this.PostMessage("Completed.");
          fileDeliverer.Deliver(podcast, filePath);
        };
      }
      else
      {
        podcastDownload.OnSuccessfulDownload += (podcast, filePath) =>
        {
          this.PostMessage("Completed.");
          this.PostMessage(String.Empty); //Blank line to break up text flow
        };
      }

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

        this.PostMessage(e.Message);
      };

      podcastDownload.OnFinish += () => this.ResetProgressBar(-1);

      return podcastDownload;
    }

    private void PostMessage(String message)
    {
      this.PostMessage(message, true);
    }

    private void PostMessage(String message, Boolean includeLineBreak)
    {
      new Task(() =>
      {
        if (includeLineBreak)
        {
          message += "\r\n";
        }

        this.Feedback.Text += message;
        this.FeedbackScroller.ScrollToBottom();
      }).Start(this.mainTaskScheduler);
    }

    private void ResetProgressBar(Int64 expectedFileSize = -1)
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

      new Task(() =>
      {
        this.Progress.Value = 0;
        this.ProgressSize.Text = progressSize;
        this.Progress.IsIndeterminate = this.fileSizeNotKnown;
      }).Start(this.mainTaskScheduler);
    }

    private void SetStateOfCancelButton(Boolean state)
    {
      new Task(() =>
      {
        this.Cancel.IsEnabled = state;
      }).Start(this.mainTaskScheduler);
    }

    private Feed SynchroniseFeed(Feed newFeed, Int32 podcastIndex, Feed oldFeed)
    {
      Int32 i = 0;
      while (podcastIndex < newFeed.Podcasts.Length && i < oldFeed.Podcasts.Length)
      {
        if (!oldFeed.Podcasts[i].Equals(newFeed.Podcasts[podcastIndex]))
        {
          break;
        }

        newFeed.Podcasts[podcastIndex] = oldFeed.Podcasts[i];
        i++;
        podcastIndex++;
      }

      return newFeed;
    }

    private void UpdateProgessEventHandler(Int32 bytesWrittenToFile)
    {
      this.downloadedSize += bytesWrittenToFile;
      var downloadedSizeInMb = this.downloadedSize / 1048576.0;

      if (this.fileSizeNotKnown)
      {
        new Task(() =>
        {
          this.ProgressSize.Text = downloadedSizeInMb.ToString("0.00") + this.progressSizeLabel;
        }).Start(this.mainTaskScheduler);

        return;
      }

      new Task(() =>
      {
        if (this.downloadedSize > this.fileSize)
        {
          this.Progress.Value = 100;
          return;
        }

        Int64 steps = this.downloadedSize / this.percentageStepSize;
        if (steps > this.Progress.Value)
        {
          this.Progress.Value = (Int32)steps;
        }

        this.ProgressSize.Text = downloadedSizeInMb.ToString("0.00") + this.progressSizeLabel;

      }).Start(this.mainTaskScheduler);
    }
  }
}
