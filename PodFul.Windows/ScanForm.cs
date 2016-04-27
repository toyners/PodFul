
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class ScanForm : Form
  {
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private TaskScheduler mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

    public ScanForm(IList<Feed> feeds)
    {
      InitializeComponent();
      this.Text = "Scanning " + feeds.Count + " feed" + (feeds.Count != 1 ? "s" : String.Empty);

      CancellationToken cancellationToken = this.cancellationTokenSource.Token;

      Task task = Task.Factory.StartNew(() =>
      {
        Dictionary<Feed, List<Podcast>> updatedFeeds = new Dictionary<Feed, List<Podcast>>();
        String scanReport = null;
        foreach (var feed in feeds)
        {
          if (this.cancellationTokenSource.IsCancellationRequested)
          {
            this.PostMessage("\r\nCANCELLED");
            return;
          }

          this.PostMessage("Scanning \"" + feed.Title + "\".");

          var newFeed = FeedFunctions.CreateFeed(feed.URL, String.Empty);

          this.PostMessage("Podcast list read.");

          Int32 index = 0;
          List<Podcast> newPodcasts = new List<Podcast>();
          while (index < newFeed.Podcasts.Length && index < feed.Podcasts.Length && !newFeed.Podcasts[index].Equals(feed.Podcasts[index]))
          {
            newPodcasts.Add(newFeed.Podcasts[index]);
            index++;
          }

          String message = "Scan completed. ";
          if (index == 0)
          {
            message += "No new podcasts found.";
          }
          else
          {
            scanReport += String.Format("\"{0}\": {1} podcast{2} found.\r\n", feed.Title, index, (index != 1 ? "s" : String.Empty));
            message += index + " new podcast" + (index != 1 ? "s" : String.Empty) + " found.";
            updatedFeeds.Add(newFeed, newPodcasts);
          }

          this.PostMessage(message + "\r\n");
        }

        if (scanReport != null)
        {
          this.PostMessage("Scan Report\r\n" + scanReport);
        }
      }, cancellationToken);
    }

    private void PostMessage(String message)
    {
      new Task(() =>
      {
        this.feedback.Text += message + "\r\n";
      }).Start(this.mainTaskScheduler);
    }

    private void cancelButton_Click(Object sender, EventArgs e)
    {
      this.cancellationTokenSource.Cancel();
    }
  }
}
