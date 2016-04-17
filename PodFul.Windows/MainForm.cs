
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
      this.feeds = this.CreateFeedList();
      this.removeFeed.Enabled = (this.feedList.SelectedIndex != -1);
      this.scanFeeds.Enabled = (this.feedList.Items.Count > 0);
    }

    private void AddFeed_Click(Object sender, EventArgs e)
    {
      var form = new AddFeedForm();
      if (form.ShowDialog() == DialogResult.Abort)
      {
        return;
      }

      var feed = FeedFunctions.CreateFeed(form.FeedURL.Text, form.FeedDirectory.Text);
      FeedFunctions.WriteFeedToFile(feed, feedDirectory + this.feeds.Count + feedFileExtension);
      this.feeds.Add(feed);
      this.feedList.Items.Add(feed.Title);
    }

    private List<Feed> CreateFeedList()
    {
      List<Feed> list = new List<Feed>();
      foreach (var filePath in Directory.GetFiles(this.feedDirectory, "*" + feedFileExtension, SearchOption.TopDirectoryOnly))
      {
        list.Add(FeedFunctions.ReadFeedFromFile(filePath));
      }

      return list;
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
