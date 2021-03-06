﻿
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

    public MainForm()
    {
      InitializeComponent();

      this.DisplayTitle();

      var feedDirectory = ConfigurationManager.AppSettings["FeedDirectory"];
      this.feedStorage = new JSONFileStorage(feedDirectory);
      this.feedStorage.Open();
      foreach (var feed in this.feedStorage.Feeds)
      {
        AddFeedToList(feed);
      }

      this.removeFeed.Enabled = (this.feedList.SelectedIndex != -1);
      this.scanFeeds.Enabled = (this.feedList.Items.Count > 0);
    }

    private void DisplayTitle()
    {
      var guiVersion = Assembly.GetExecutingAssembly().GetName().Version;
      var libraryVersion = Assembly.GetAssembly(typeof(IFeedStorage)).GetName().Version;
      this.Text = String.Format("PodFul - v{0}.{1}.{2} (v{3}.{4}.{5})",
        guiVersion.Major,
        guiVersion.Minor,
        guiVersion.Build,
        libraryVersion.Major,
        libraryVersion.Minor,
        libraryVersion.Build);
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
        var feedDirectory = addFeedForm.FeedDirectory.Text;
        var feedFilePath = Path.Combine(feedDirectory, "download.rss");
        feed = FeedFunctions.CreateFeed(addFeedForm.FeedURL.Text, feedFilePath, feedDirectory, "", System.Threading.CancellationToken.None);
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

        podcast.SetFileDetails(fileInfo.Length, fileInfo.CreationTime);
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
      var selectedList = this.GetSelectedFeedsForScanning();
      if (selectedList == null)
      {
        return;
      }

      var form = new ProcessingForm(this.feedStorage, selectedList, this.addToWinamp.Checked);
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
        Miscellaneous.GetReadableFileSize(podcast.FileDetails.FileSize) + " MB",
        podcast.FileDetails.DownloadDate != DateTime.MinValue ? podcast.FileDetails.DownloadDate.ToString("ddd, dd-MMM-yyyy HH:mm:ss") : @"n\a");

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
      var form = new SelectionForm(
        feed.Title + String.Format(" [{0} podcast{1}]", feed.Podcasts.Length, (feed.Podcasts.Length != 1 ? "s" : String.Empty)),
        feed.Podcasts);

      if (form.ShowDialog() == DialogResult.Cancel)
      {
        return null;
      }

      return new Queue<Int32>(form.SelectedRowIndexes);
    }

    private Queue<Int32> GetSelectedFeedsForScanning()
    {
      var form = new SelectionForm(
        String.Format("{0} feed{1}", this.feedStorage.Feeds.Length, (this.feedStorage.Feeds.Length != 1 ? "s" : String.Empty)),
        this.feedStorage);

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
