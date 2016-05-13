
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows.Forms;
  using Jabberwocky.Toolkit.String;
  using PodFul.Library;

  public partial class MainForm : Form
  {
    private const String feedFileExtension = ".feed";
    private Dictionary<String, String> fileNameSubstitutions = new Dictionary<String, String>
    {
      { "\\", "_bs_" },
      { "/", "_fs_" },
      { ":", "_c_" },
      { "*", "_a_" },
      { "?", "_q_" },
      { "\"", "_qu_" },
      { "<", "_l_" },
      { ">", "_g_" },
      { "|", "_b_" },
    };

    private List<Feed> feeds;
    private List<String> feedFilePaths;
    private Feed currentFeed;
    private String feedDirectory;

    public MainForm()
    {
      InitializeComponent();

      this.feedDirectory = @"C:\Users\toyne\AppData\Local\PodFul\";
      this.feeds = new List<Feed>();
      this.feedFilePaths = new List<String>();

      this.CreateFeedList();
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

      var feedFilePath = feedDirectory + this.feeds.Count + "_" + feed.Title.Substitute(fileNameSubstitutions) + feedFileExtension;
      FeedFunctions.WriteFeedToFile(feed, feedFilePath);
      this.AddFeedToList(feed);
      this.feedFilePaths.Add(feedFilePath);

      this.DownloadPodcasts(feed, feedFilePath);
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
        this.feedFilePaths.Add(filePath);
      }
    }

    private void removeFeed_Click(Object sender, EventArgs e)
    {
      var index = this.feedList.SelectedIndex;

      try
      {
        File.Delete(this.feedFilePaths[index]);
      }
      catch (Exception exception)
      {
        MessageBox.Show("Cannot delete feed file. Exception message is\r\n\r\n" + exception.Message);
        return;
      }

      this.feeds.RemoveAt(index);
      this.feedList.Items.RemoveAt(index);
      this.feedFilePaths.RemoveAt(index);

      if (this.feeds.Count == 0)
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
      var form = new ScanForm(this.feeds, this.feedFilePaths, this.addToWinamp.Checked);
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
        Miscellaneous.GetReadableFileSize(podcast.FileSize) + " MB",
        podcast.DownloadDate != DateTime.MinValue ? podcast.DownloadDate.ToString("ddd, dd-MMM-yyyy HH:mm:ss") : @"n\a");

      this.podcastDescription.Text = text;
    }

    private void downloadPodcast_Click(Object sender, EventArgs e)
    {
      var feedFilePath = this.feedFilePaths[this.feeds.IndexOf(this.currentFeed)];
      DownloadPodcasts(this.currentFeed, feedFilePath);
    }

    private void DownloadPodcasts(Feed feed, String feedFilePath)
    {
      var selectedList = this.GetSelectedPodcastsFromDownloadForm(feed);
      if (selectedList == null)
      {
        return;
      }

      var scanForm = new ScanForm(feed, feedFilePath, selectedList, this.addToWinamp.Checked);
      scanForm.ShowDialog();
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
        var feedFilePath = this.feedFilePaths[this.feeds.IndexOf(this.currentFeed)];
        FeedFunctions.WriteFeedToFile(this.currentFeed, feedFilePath);
      }
    }
  }
}
