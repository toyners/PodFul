
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

      var feed = FeedFunctions.CreateFeed(addFeedForm.FeedURL.Text, addFeedForm.FeedDirectory.Text);
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

      if (this.feedList.SelectedIndex == -1)
      {
        return;
      }

      this.DisplayPodcasts(this.feeds[this.feedList.SelectedIndex].Podcasts);
    }

    private void DisplayPodcasts(IEnumerable<Podcast> podcasts)
    {
      this.podcastList.Items.Clear();
      foreach (var podcast in podcasts)
      {
        var shortDescription = podcast.Description.Substring(0, 47) + "...";
        this.podcastList.Items.Add(podcast.Title + " - " + shortDescription);
      }
    }

    private void scanFeeds_Click(Object sender, EventArgs e)
    {
      List<Podcast> newPodcasts = new List<Podcast>();
      foreach (var feed in this.feeds)
      {
        var podcasts = FeedFunctions.CreatePodcastList(feed.URL);

        Int32 index = 0;
        while (podcasts[index] != feed.Podcasts[index])
        {
          newPodcasts.Add(podcasts[index]);
          index++;
        }
      }

      var form = new ScanResultsForm(newPodcasts);
      if (form.ShowDialog() == DialogResult.Cancel)
      {
        return;
      }
    }
  }
}
