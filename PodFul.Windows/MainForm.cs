
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class MainForm : Form
  {
    private const String feedFileExtension = ".feed";

    private List<Feed> feeds;
    private Feed currentFeed;
    private String feedDirectory;

    public MainForm()
    {
      InitializeComponent();

      this.feedDirectory = @"C:\Projects\PodFul\Test\";
      this.feeds = new List<Feed>();
      this.CreateFeedList();
      this.removeFeed.Enabled = (this.feedList.SelectedIndex != -1);
      this.scanFeeds.Enabled = (this.feedList.Items.Count > 0);
    }

    private void AddFeed_Click(Object sender, EventArgs e)
    {
      var addFeedForm = new AddFeedForm();
      if (addFeedForm.ShowDialog() == DialogResult.Cancel)
      {
        return;
      }

      Feed feed = null;
      try
      {
        feed = FeedFunctions.CreateFeed(addFeedForm.FeedURL.Text, addFeedForm.FeedDirectory.Text);
      }
      catch (Exception exception)
      {
        MessageBox.Show("Exception occurred when adding feed:\r\n\r\n" + exception.Message, "Exception occurred.");
        return;
      }

      FeedFunctions.WriteFeedToFile(feed, feedDirectory + this.feeds.Count + feedFileExtension);
      this.AddFeedToList(feed);
    }

    private void AddFeedToList(Feed feed)
    {
      this.feeds.Add(feed);
      var title = feed.Title + " [" + feed.Podcasts.Length + " podcast" + (feed.Podcasts.Length != 1 ? "s" : String.Empty) + "]";
      this.feedList.Items.Add(title);
      this.feedList.SelectedIndex = this.feedList.Items.Count - 1;
    }

    private void CreateFeedList()
    {
      foreach (var filePath in Directory.GetFiles(this.feedDirectory, "*" + feedFileExtension, SearchOption.TopDirectoryOnly))
      {
        AddFeedToList(FeedFunctions.ReadFeedFromFile(filePath));
      }
    }

    private void removeFeed_Click(Object sender, EventArgs e)
    {
      var index = this.feedList.SelectedIndex;
      this.feeds.RemoveAt(this.feedList.SelectedIndex);

      if (this.feeds.Count == 0)
      {
        this.feedList.SelectedIndex = -1;
        return;
      }

      if (index == 0)
      {
        this.feedList.SelectedIndex = 0;
        return;
      }

      if (index == this.feedList.Items.Count)
      {
        this.feedList.SelectedIndex = this.feedList.Items.Count - 1;
        return;
      }

      this.feedList.SelectedIndex -= 1;
    }

    private void feedList_SelectedIndexChanged(Object sender, EventArgs e)
    {
      this.removeFeed.Enabled = (this.feedList.SelectedIndex != -1);
      this.downloadPodcast.Enabled = (this.feedList.SelectedIndex != -1);

      if (this.feedList.SelectedIndex == -1)
      {
        this.feedDescription.Text = String.Empty;
        return;
      }

      this.currentFeed = this.feeds[this.feedList.SelectedIndex];
      this.feedDescription.Text = this.currentFeed.Description;
      this.DisplayPodcasts();
    }

    private void DisplayPodcasts()
    {
      this.podcastList.Items.Clear();
      foreach (var podcast in this.currentFeed.Podcasts)
      {
        this.podcastList.Items.Add(podcast.Title);
      }
    }

    private void scanFeeds_Click(Object sender, EventArgs e)
    {
      List<Podcast> newPodcasts = new List<Podcast>();
      foreach (var feed in this.feeds)
      {
        var podcasts = FeedFunctions.CreatePodcastList(feed.URL);

        Int32 index = 0;
        while (index < podcasts.Length && index < feed.Podcasts.Length && !podcasts[index].Equals(feed.Podcasts[index]))
        {
          newPodcasts.Add(podcasts[index]);
          index++;
        }
      }

      if (newPodcasts.Count == 0)
      {
        MessageBox.Show("No new podcasts found", "Podful - Scan results", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      var form = new ScanResultsForm("Scan Results", newPodcasts);
      if (form.ShowDialog() == DialogResult.Cancel)
      {
        return;
      }
    }

    private void podcastList_SelectedIndexChanged(Object sender, EventArgs e)
    {
      this.downloadPodcast.Enabled = (this.podcastList.SelectedIndex != -1);

      if (this.podcastList.SelectedIndex == -1)
      {
        this.podcastDescription.Text = String.Empty;
        return;
      }

      var podcast = this.currentFeed.Podcasts[this.podcastList.SelectedIndex];
      var text = String.Format("{0}\r\nPUB DATE: {1}\r\nFILE SIZE: {2}\r\nDOWNLOAD DATE: {3}", 
        podcast.Description, 
        podcast.PubDate.ToString("ddd, dd-MMM-yyyy"), 
        podcast.FileSize,
        podcast.LatestDownloadDate != DateTime.MinValue ? podcast.LatestDownloadDate.ToString("ddd, dd-MMM-yyyy HH:mm::ss") : @"n\a");

      this.podcastDescription.Text = text;
    }

    private void downloadPodcast_Click(Object sender, EventArgs e)
    {
      var form = new ScanResultsForm(
        this.currentFeed.Title + String.Format("[{0} podcast{1}]", this.currentFeed.Podcasts.Length, (this.currentFeed.Podcasts.Length != 1 ? "s" : String.Empty)),
        this.currentFeed.Podcasts);

      if (form.ShowDialog() == DialogResult.Cancel)
      {
        return;
      }

    }
  }
}
