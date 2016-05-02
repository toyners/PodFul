
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
    private CancellationToken cancellationToken;
    private TaskScheduler mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private Int64 fileSize;
    private Int64 downloadedSize;

    public ScanForm(IList<Feed> feeds, IList<String> feedFilePaths)
    {
      InitializeComponent();
      this.Text = "Scanning " + feeds.Count + " feed" + (feeds.Count != 1 ? "s" : String.Empty);

      this.cancellationToken = this.cancellationTokenSource.Token;

      Task task = Task.Factory.StartNew(() =>
      {
        Dictionary<Feed, List<Podcast>> updatedFeeds = new Dictionary<Feed, List<Podcast>>();
        Dictionary<String, Feed> updatedFeedFilePaths = new Dictionary<String, Feed>();
        var newPodcasts = new Queue<Podcast>();
        BigFileDownloader downloader = new BigFileDownloader();
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

          var newFeed = FeedFunctions.CreateFeed(feed.URL, String.Empty);

          this.PostMessage("Comparing podcasts feeds.");

          Int32 podcastIndex = 0;
          newPodcasts.Clear();
          while (podcastIndex < newFeed.Podcasts.Length && podcastIndex < feed.Podcasts.Length && !newFeed.Podcasts[podcastIndex].Equals(feed.Podcasts[podcastIndex]))
          {
            newPodcasts.Enqueue(newFeed.Podcasts[podcastIndex]);
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

          if (!DownloadPodcasts(downloader, feed.Directory, newPodcasts))
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

    private Boolean DownloadPodcasts(BigFileDownloader downloader, String directoryPath, Queue<Podcast> podcasts)
    {
      while (podcasts.Count > 0)
      {
        var podcast = podcasts.Dequeue();
        this.fileSize = podcast.FileSize;
        this.downloadedSize = 0;

        this.ResetProgressBar(podcast.FileSize);

        this.PostMessage(String.Format("Downloading \"{0}\" ...", podcast.Title), false);

        var filePath = Path.Combine(directoryPath, podcast.URL.Substring(podcast.URL.LastIndexOf('/') + 1));
        Task downloadTask = downloader.DownloadAsync(podcast.URL, filePath, this.cancellationToken, this.UpdateProgessEventHandler);
        downloadTask.Wait();

        if (downloadTask.IsFaulted)
        {
          var exception = downloadTask.Exception.Flatten();
          this.PostMessage(String.Format(" FAILED!\r\nEXCEPTION: {0}.", exception.Message), false);
        }
        else if (downloadTask.IsCanceled)
        {
          // Downloading cancelled.
          return false;
        }
        else
        {
          this.PostMessage("Complete");
        }
      }

      this.ResetProgressBar();

      return true;
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

        this.downloadedSize += bytesWrittenToFile;
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
