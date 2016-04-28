
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

    public ScanForm(IList<Feed> feeds, IList<String> feedFilePaths)
    {
      InitializeComponent();
      this.Text = "Scanning " + feeds.Count + " feed" + (feeds.Count != 1 ? "s" : String.Empty);

      CancellationToken cancellationToken = this.cancellationTokenSource.Token;

      Task task = Task.Factory.StartNew(() =>
      {
        Dictionary<Feed, List<Podcast>> updatedFeeds = new Dictionary<Feed, List<Podcast>>();
        Dictionary<String, Feed> updatedFeedFilePaths = new Dictionary<String, Feed>();

        for (Int32 feedIndex = 0; feedIndex < feeds.Count; feedIndex++)
        {
          var feed = feeds[feedIndex];

          if (this.cancellationTokenSource.IsCancellationRequested)
          {
            this.PostMessage("\r\nCANCELLED");
            return;
          }

          this.PostMessage("Scanning \"" + feed.Title + "\".");

          var newFeed = FeedFunctions.CreateFeed(feed.URL, String.Empty);

          this.PostMessage("Podcast list read.");

          Int32 podcastIndex = 0;
          List<Podcast> newPodcasts = new List<Podcast>();
          while (podcastIndex < newFeed.Podcasts.Length && podcastIndex < feed.Podcasts.Length && !newFeed.Podcasts[podcastIndex].Equals(feed.Podcasts[podcastIndex]))
          {
            newPodcasts.Add(newFeed.Podcasts[podcastIndex]);
            podcastIndex++;
          }

          String message = "Scan completed. ";
          if (podcastIndex == 0)
          {
            message += "No new podcasts found.";
          }
          else
          {            
            message += podcastIndex + " new podcast" + (podcastIndex != 1 ? "s" : String.Empty) + " found.";

            updatedFeeds.Add(newFeed, newPodcasts);
            updatedFeedFilePaths.Add(feedFilePaths[feedIndex], newFeed);
          }

          this.PostMessage(message + "\r\n");
        }

        // Update all the feed files
        foreach (KeyValuePair<String, Feed> kv in updatedFeedFilePaths)
        {
          if (this.cancellationTokenSource.IsCancellationRequested)
          {
            this.PostMessage("\r\nCANCELLED");
            return;
          }

          if (!this.UpdateFeed(kv.Key, kv.Value))
          {
            // Updating the feed went wrong so scrap downloading the podcasts otherwise we may be 
            // out of sync in the future.
            updatedFeeds.Remove(kv.Value);
          }
        }

        // Now download the podcasts

        // Complete the final scan report.
        String scanReport = null;
        //scanReport += String.Format("\"{0}\": {1} podcast{2} found.\r\n", feed.Title, podcastIndex, (podcastIndex != 1 ? "s" : String.Empty));

        if (scanReport != null)
        {
          this.PostMessage("Scan Report\r\n" + scanReport);
        }

      }, cancellationToken);
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

    private void cancelButton_Click(Object sender, EventArgs e)
    {
      this.cancellationTokenSource.Cancel();
    }
  }
}
