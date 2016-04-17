
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
      var form = new AddFeedForm();
      if (form.ShowDialog() == DialogResult.Cancel)
      {
        return;
      }

      var feed = FeedFunctions.CreateFeed(form.FeedURL.Text, form.FeedDirectory.Text);
      FeedFunctions.WriteFeedToFile(feed, feedDirectory + this.feeds.Count + feedFileExtension);
      this.AddFeedToList(feed);
    }

    private void AddFeedToList(Feed feed)
    {
      this.feeds.Add(feed);
      var title = feed.Title + " [" + feed.Podcasts.Length + " podcast" + (feed.Podcasts.Length != 1 ? "s" : String.Empty) + "]";
      this.feedList.Items.Add(title);
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
    }

    private void scanFeeds_Click(Object sender, EventArgs e)
    {
      foreach (var feed in this.feeds)
      {
        FeedFunctions.DownloadDocument(feed.URL);
      }
    }
  }
}
