
namespace PodFul.Winforms
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class ProcessingForm : Form
  {
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private TaskScheduler mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private Int64 fileSize;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;

    public ProcessingForm(IFeedStorage feedStorage, Queue<Int32> feedIndexes, Boolean addToWinAmp)
    {
      InitializeComponent();

      Feed[] feeds = feedStorage.Feeds;

      var feedTotal = feedIndexes.Count;
      this.Text = "Scanning " + feedTotal + " feed" + (feedTotal != 1 ? "s" : String.Empty);

      var cancellationToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(cancellationToken, addToWinAmp, true);

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
            var feedFilePath = Path.Combine(feed.Directory, "download.rss");
            newFeed = FeedFunctions.CreateFeed(feed.URL, feedFilePath, feed.Directory, CancellationToken.None);
          }
          catch (Exception exception)
          {
            var exceptionReport = String.Format("EXCEPTION thrown for \"{0}\": {1}\r\n", feed.Title, exception.Message);
            scanReport += exceptionReport;
            MessageBox.Show(exception.Message);
            this.PostMessage(exceptionReport);
            continue;
          }

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
            var dialogResult = MessageBox.Show(text, "Multiple podcasts found.", MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Cancel)
            {
              var feedReport = podcastIndex + " podcasts found";
              this.PostMessage(feedReport + " (Scan cancelled).\r\n");
              scanReport += feedReport + " for \"" + feed.Title + "\" (Scan cancelled).";
              break;
            }

            if (dialogResult == DialogResult.No)
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

      }, cancellationToken);
    }

    public ProcessingForm(IFeedStorage feedStorage, Feed feed, Queue<Int32> queue, Boolean addToWinAmp)
    {
      InitializeComponent();
      this.Text = "Downloading " + queue.Count + " podcast" + (queue.Count != 1 ? "s" : String.Empty);

      var cancellationToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(cancellationToken, addToWinAmp, false);

      Task task = Task.Factory.StartNew(() =>
      {
        this.SetStateOfCancelButton(true);
        if (podcastDownload.Download(feed.Directory, feed.Podcasts, queue))
        {
          feedStorage.Update(feed);
        }

        this.SetStateOfCancelButton(false);
      }, cancellationToken);
    }

    private void DisplayTitleForScanning(Int32 number, Int32 count)
    {
      new Task(() =>
      {
        this.Text = "Scanning " + number + " of " + count + " feed" + (count != 1 ? "s" : String.Empty);

      }).Start(this.mainTaskScheduler);
    }

    private PodcastDownload InitialisePodcastDownload(CancellationToken cancellationToken, Boolean addToWinAmp, Boolean isScanning)
    {
      var podcastDownload = new PodcastDownload(cancellationToken, this.UpdateProgessEventHandler);
      podcastDownload.OnBeforeDownload += (podcast) =>
      {
        this.fileSize = podcast.FileDetails.FileSize;
        this.percentageStepSize = this.fileSize / 100;
        this.downloadedSize = 0;
        this.ResetProgressBar(podcast.FileDetails.FileSize);
        this.PostMessage(String.Format("Downloading \"{0}\" ... ", podcast.Title), false);
      };

      if (addToWinAmp)
      {
        if (isScanning)
        {
          podcastDownload.OnSuccessfulDownload += (podcast, filePath) =>
          {
            this.PostMessage("Completed.");
            Process.Start(@"C:\Program Files (x86)\Winamp\winamp.exe", String.Format("/ADD \"{0}\"", filePath));
            this.PostMessage(String.Format("Added \"{0}\" to WinAmp.", podcast.Title));
          };
        }
        else
        {
          podcastDownload.OnSuccessfulDownload += (podcast, filePath) =>
          {
            this.PostMessage("Completed.");
            Process.Start(@"C:\Program Files (x86)\Winamp\winamp.exe", String.Format("/ADD \"{0}\"", filePath));
            this.PostMessage(String.Format("\"{0}\" added to WinAmp", podcast.Title));
            this.PostMessage(String.Empty); //Blank line to break up text flow
          };
        }
      }
      else
      {
        podcastDownload.OnSuccessfulDownload += (podcast, filePath) =>
        {
          this.PostMessage("Completed.");
        };
      }

      podcastDownload.OnException += (exception, podcast) =>
      {
        Exception e = exception.Flatten();
        if (e.InnerException != null)
        {
          e = e.InnerException;
        }

        this.PostMessage(e.Message);
      };

      podcastDownload.OnFinish += () => this.ResetProgressBar(-1);

      return podcastDownload;
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
      if (this.progressBar.Style == ProgressBarStyle.Marquee)
      {
        return;
      }

      new Task(() =>
      {
        this.downloadedSize += bytesWrittenToFile;
        if (this.downloadedSize > this.fileSize)
        {
          this.progressBar.Value = 100;
          return;
        }

        Int64 steps = this.downloadedSize / this.percentageStepSize;
        if (steps > this.progressBar.Value)
        {
          this.progressBar.Value = (Int32)steps;
        }
      }).Start(this.mainTaskScheduler);
    }

    private void SetStateOfCancelButton(Boolean state)
    {
      new Task(() =>
      {
        this.cancelButton.Enabled = state;
      }).Start(this.mainTaskScheduler);
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

        this.feedback.Text += message;
        this.feedback.SelectionStart = this.feedback.Text.Length;
        this.feedback.ScrollToCaret();
      }).Start(this.mainTaskScheduler);
    }

    private void ResetProgressBar(Int64 expectedFileSize = -1)
    {
      new Task(() =>
      {
        this.progressBar.Value = 0;

        if (expectedFileSize == 0)
        {
          this.progressBar.Style = ProgressBarStyle.Marquee;
        }
        else
        {
          this.progressBar.Style = ProgressBarStyle.Continuous;
        }
      }).Start(this.mainTaskScheduler);
    }

    private void cancelButton_Click(Object sender, EventArgs e)
    {
      this.cancellationTokenSource.Cancel();
    }
  }
}
