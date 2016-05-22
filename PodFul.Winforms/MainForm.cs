
namespace PodFul.Winforms
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Reflection;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class MainForm : Form
  {
    private IFeedStorage feedStorage;
    private Feed currentFeed;
    private String feedDirectory;

    public MainForm()
    {
      InitializeComponent();

      this.Text = String.Format("PodFul - v{0} (v{1})",
        Assembly.GetExecutingAssembly().GetName().Version,
        Assembly.GetAssembly(typeof(IFeedStorage)).GetName().Version);
      
      this.feedDirectory = ConfigurationManager.AppSettings["FeedDirectory"];
      this.feedStorage = new FeedFileStorage(this.feedDirectory);
      this.feedStorage.Open();
      foreach (var feed in this.feedStorage.Feeds)
      {
        AddFeedToList(feed);
      }

      this.removeFeed.Enabled = (this.feedList.SelectedIndex != -1);
      this.scanFeeds.Enabled = (this.feedList.Items.Count > 0);
    }

    private void addFeed_Click(Object sender, EventArgs e)
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

      var fileCount = this.GetCountOfExistingMediaFilesForFeed(feed);
      if (fileCount > 0 &&
        MessageBox.Show(String.Format("{0} mp3 file(s) found in '{1}'.\r\n\r\n Attempt to sync the feed against these files?", fileCount, feed.Directory), "Existing files found", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
      {
        this.SyncWithExistingFiles(feed);
      }

      this.feedStorage.Add(feed);
      this.AddFeedToList(feed);

      this.DownloadPodcasts(feed);
    }

    private Int32 GetCountOfExistingMediaFilesForFeed(Feed feed)
    {
      return Directory.GetFiles(feed.Directory, "*.mp3").Length;
    }

    private Int32 SyncWithExistingFiles(Feed feed)
    {
      var syncCount = 0;
      for (Int32 podcastIndex = 0; podcastIndex < feed.Podcasts.Length; podcastIndex++)
      {
        var podcast = feed.Podcasts[podcastIndex];
        var fileName = podcast.URL.Substring(podcast.URL.LastIndexOf('/') + 1);
        var fileInfo = new FileInfo(feed.Directory + fileName);

        if (!fileInfo.Exists)
        {
          continue;
        }

        podcast = Podcast.SetDownloadDate(podcast, fileInfo.CreationTime);
        podcast = Podcast.SetFileSize(podcast, fileInfo.Length);
        feed.Podcasts[podcastIndex] = podcast;
        syncCount++;
      }

      return syncCount;
    }

    private void AddFeedToList(Feed feed)
    {
      var title = feed.Title + " [" + feed.Podcasts.Length + " podcast" + (feed.Podcasts.Length != 1 ? "s" : String.Empty) + "]";
      this.feedList.Items.Add(title);
      this.feedList.SelectedIndex = this.feedList.Items.Count - 1;
    }

    private void removeFeed_Click(Object sender, EventArgs e)
    {
      var index = this.feedList.SelectedIndex;

      this.feedStorage.Remove(this.currentFeed);
      this.feedList.Items.RemoveAt(index);

      if (this.feedStorage.Feeds.Length == 0)
      {
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

      this.feedList.SelectedIndex = index;
    }

    private void feedList_SelectedIndexChanged(Object sender, EventArgs e)
    {
      this.removeFeed.Enabled = (this.feedList.SelectedIndex != -1);
      this.downloadPodcast.Enabled = (this.feedList.SelectedIndex != -1);

      if (this.feedList.SelectedIndex == -1)
      {
        this.podcastList.Items.Clear();
        this.feedDescription.Text = String.Empty;
        return;
      }

      this.currentFeed = this.feedStorage.Feeds[this.feedList.SelectedIndex];
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
      var form = new ProcessingForm(this.feedStorage, this.addToWinamp.Checked);
      form.ShowDialog();
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
        Miscellaneous.GetReadableFileSize(podcast.FileSize) + " MB",
        podcast.DownloadDate != DateTime.MinValue ? podcast.DownloadDate.ToString("ddd, dd-MMM-yyyy HH:mm:ss") : @"n\a");

      this.podcastDescription.Text = text;
    }

    private void downloadPodcast_Click(Object sender, EventArgs e)
    {
      DownloadPodcasts(this.currentFeed);
    }

    private void DownloadPodcasts(Feed feed)
    {
      var selectedList = this.GetSelectedPodcastsFromDownloadForm(feed);
      if (selectedList == null)
      {
        return;
      }

      var form = new ProcessingForm(this.feedStorage, feed, selectedList, this.addToWinamp.Checked);
      form.ShowDialog();
    }

    private Queue<Int32> GetSelectedPodcastsFromDownloadForm(Feed feed)
    {
      var form = new DownloadForm(
        feed.Title + String.Format(" [{0} podcast{1}]", feed.Podcasts.Length, (feed.Podcasts.Length != 1 ? "s" : String.Empty)),
        feed.Podcasts);

      if (form.ShowDialog() == DialogResult.Cancel)
      {
        return null;
      }

      return new Queue<Int32>(form.SelectedRowIndexes);
    }

    private void syncPodcasts_Click(Object sender, EventArgs e)
    {
      var fileCount = this.GetCountOfExistingMediaFilesForFeed(this.currentFeed);
      if (fileCount == 0)
      {
        MessageBox.Show(String.Format("No mp3 files found in '{0}'.", this.currentFeed.Directory), "No existing files found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      var syncCount = this.SyncWithExistingFiles(this.currentFeed);

      MessageBox.Show(String.Format("{0} podcast(s) synced.", syncCount), "Files Synced");

      if (syncCount > 0)
      {
        this.feedStorage.Update(this.currentFeed);
      }
    }
  }
}
