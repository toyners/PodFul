
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class ScanForm : Form
  {
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private TaskScheduler mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private Int64 fileSize;
    private Int64 downloadedSize;
    
    public ScanForm(IList<Feed> feeds, IList<String> feedFilePaths)
    {
      InitializeComponent();
      this.Text = "Scanning " + feeds.Count + " feed" + (feeds.Count != 1 ? "s" : String.Empty);

      var cancellationToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(cancellationToken);

      Task task = Task.Factory.StartNew(() =>
      {
        var podcastIndexes = new Queue<Int32>();
        String scanReport = null;

        for (Int32 feedIndex = 0; feedIndex < feeds.Count; feedIndex++)
        {
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
            newFeed = FeedFunctions.CreateFeed(feed.URL, String.Empty);
          }
          catch (Exception exception)
          {
            MessageBox.Show(exception.Message);
            continue;
          }

          this.PostMessage("Comparing podcasts feeds.");

          Int32 podcastIndex = 0;
          podcastIndexes.Clear();
          while (podcastIndex < newFeed.Podcasts.Length && podcastIndex < feed.Podcasts.Length && !newFeed.Podcasts[podcastIndex].Equals(feed.Podcasts[podcastIndex]))
          {
            podcastIndexes.Enqueue(podcastIndex);
            podcastIndex++;
          }

          String message = "Comparison completed. ";
          if (podcastIndex == 0)
          {
            message += "No new podcasts found.";
          }
          else
          {  
            var feedReport = podcastIndex + " podcast" + (podcastIndex != 1 ? "s" : String.Empty) + " found";
            message += feedReport + ".";
            scanReport += feedReport + " for \"" + feed.Title + "\".\r\n";
          }

          this.PostMessage(message);

          if (!podcastDownload.Download(feed.Directory, newFeed.Podcasts, podcastIndexes))
          {
            this.PostMessage("\r\nCANCELLED");
            continue;
          }

          this.PostMessage("Updating feed on file.");
          this.UpdateFeed(feedFilePaths[feedIndex], newFeed);
          this.PostMessage("Feed updated.");

          feeds[feedIndex] = newFeed;
        }

        // Display the final scan report.
        this.PostMessage("Scan Report\r\n" + scanReport);

      }, cancellationToken);
    }

    public ScanForm(Feed feed, String feedFilePath, Queue<Int32> queue)
    {
      var cancellationToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(cancellationToken);

      Task task = Task.Factory.StartNew(() =>
      {
        if (podcastDownload.Download(feed.Directory, feed.Podcasts, queue))
        {
          FeedFunctions.WriteFeedToFile(feed, feedFilePath);
        }

      }, cancellationToken);
    }

    private PodcastDownload InitialisePodcastDownload(CancellationToken cancellationToken)
    {
      var podcastDownload = new PodcastDownload(cancellationToken, this.UpdateProgessEventHandler);
      podcastDownload.OnBeforeDownload += (podcast) =>
      {
        this.fileSize = podcast.FileSize;
        this.downloadedSize = 0;
        this.ResetProgressBar(podcast.FileSize);
        this.PostMessage(String.Format("Downloading \"{0}\" ...", podcast.Title), false);
      };

      podcastDownload.OnSuccessfulDownload += (podcast) =>
      {
        this.PostMessage("Completed");
      };

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
          this.progressBar.Style = ProgressBarStyle.Marquee;
          return;
        }

        var value = (Int32)(this.downloadedSize / this.fileSize) * 100;

        this.progressBar.Value = value;
      }).Start(this.mainTaskScheduler);
    }

    private Boolean UpdateFeed(String feedFilePath, Feed feed)
    {
      this.PostMessage(String.Format("Updating \"{0}\" ...", feed.Title), false);
      Boolean updateSuccessful = false;
      try
      {
        File.Copy(feedFilePath, feedFilePath + ".bak", true);
        FeedFunctions.WriteFeedToFile(feed, feedFilePath);
        updateSuccessful = true;
        this.PostMessage(" Complete");
      }
      catch (Exception exception)
      {        
        this.PostMessage(String.Format(" FAILED!\r\nEXCEPTION: {0}.", exception.Message), false);
      }

      if (!updateSuccessful)
      {
        try
        {
          File.Copy(feedFilePath + ".bak", feedFilePath, true);
          this.PostMessage("\r\nReverted to the original feed.");  
        }
        catch
        {
          this.PostMessage("\r\nFAILED to revert to the original feed.");
        }
      }

      if (File.Exists(feedFilePath + ".bak"))
      {
        try
        {
          File.Delete(feedFilePath + ".bak");
        }
        catch
        {
          // Failing to delete the backup is not a problem. Ignore and carry on.
        }
      }

      return updateSuccessful;
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
