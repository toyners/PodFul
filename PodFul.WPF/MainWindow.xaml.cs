
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Controls;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private IFeedStorage feedStorage;
    private IImageResolver imageResolver;
    private Feed currentFeed;

    public MainWindow()
    {
      InitializeComponent();

      this.DisplayTitle();

      var feedDirectory = ConfigurationManager.AppSettings["FeedDirectory"];
      this.feedStorage = new FeedFileStorage(feedDirectory);
      this.feedStorage.Open();

      var imageDirectory = Path.Combine(feedDirectory, "Images");
      this.imageResolver = new ImageResolver(imageDirectory); 

      FeedList.ItemsSource = this.feedStorage.Feeds;
      FeedList.SelectedIndex = 0;
      if (this.feedStorage.Feeds.Length > 0)
      {
        this.currentFeed = this.feedStorage.Feeds[0];

        PodcastList.ItemsSource = this.currentFeed.Podcasts;
      }
    }

    private void DisplayTitle()
    {
      var guiVersion = Assembly.GetExecutingAssembly().GetName().Version;
      var libraryVersion = Assembly.GetAssembly(typeof(IFeedStorage)).GetName().Version;
      this.Title = String.Format("PodFul - v{0}.{1}.{2} (v{3}.{4}.{5})",
        guiVersion.Major,
        guiVersion.Minor,
        guiVersion.Build,
        libraryVersion.Major,
        libraryVersion.Minor,
        libraryVersion.Build);
    }

    private void addButton_Click(Object sender, RoutedEventArgs e)
    {
      var addFeedWindow = new AddFeedWindow();
      var dialogResult = addFeedWindow.ShowDialog();
      if (!dialogResult.HasValue || !dialogResult.Value)
      {
        return;
      }

      // Create the feed and add to storage.
      Feed feed = null;
      try
      {
        feed = FeedFunctions.CreateFeed(addFeedWindow.FeedURL, addFeedWindow.FeedDirectory);
      }
      catch (Exception exception)
      {
        MessageBox.Show("Exception occurred when adding feed:\r\n\r\n" + exception.Message, "Exception occurred.");
        return;
      }

      var fileCount = GetCountOfExistingMediaFilesForFeed(feed);
      if (fileCount > 0 &&
        MessageBox.Show(String.Format("{0} mp3 file(s) found in '{1}'.\r\n\r\n Attempt to sync the feed against these files?", fileCount, feed.Directory), "Existing files found", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
      {
        this.SyncWithExistingFiles(feed);
      }

      this.feedStorage.Add(feed);
      this.FeedList.SelectedItem = feed;
      //this.currentFeed = feed;
      //this.AddFeedToList(feed);

      this.DownloadPodcasts();
    }

    private static Int32 GetCountOfExistingMediaFilesForFeed(Feed feed)
    {
      return Directory.GetFiles(feed.Directory, "*.mp3").Length;
    }

    private void removeButton_Click(Object sender, RoutedEventArgs e)
    {
      var index = this.FeedList.SelectedIndex;

      this.feedStorage.Remove(this.currentFeed);
      this.FeedList.Items.RemoveAt(index);

      if (this.feedStorage.Feeds.Length == 0)
      {
        return;
      }

      if (index == 0)
      {
        this.FeedList.SelectedIndex = 0;
        return;
      }

      if (index == this.FeedList.Items.Count)
      {
        this.FeedList.SelectedIndex = this.FeedList.Items.Count - 1;
        return;
      }

      this.FeedList.SelectedIndex = index;
    }

    private void syncButton_Click(Object sender, RoutedEventArgs e)
    {

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

    private void scanButton_Click(Object sender, RoutedEventArgs e)
    {
      var title = String.Format("{0} feed{1}", this.feedStorage.Feeds.Length, (this.feedStorage.Feeds.Length != 1 ? "s" : String.Empty));
      var selectionWindow = new SelectionWindow(title, this.feedStorage.Feeds);
      var startScanning = selectionWindow.ShowDialog();

      if (startScanning == null || !startScanning.Value)
      {
        return;
      }

      var feedIndexes = new Queue<Int32>(selectionWindow.SelectedIndexes);
      var addToWinAmp = (this.AddToWinAmp.IsChecked.HasValue && this.AddToWinAmp.IsChecked.Value);
      var processingWindow = new ProcessingWindow(this.feedStorage, feedIndexes, addToWinAmp);
      processingWindow.ShowDialog();
    }

    private void downloadButton_Click(Object sender, RoutedEventArgs e)
    {
      this.DownloadPodcasts();
    }

    private void DownloadPodcasts()
    {
      var title = String.Format("{0} podcast{1}", this.currentFeed.Podcasts.Length, (this.currentFeed.Podcasts.Length != 1 ? "s" : String.Empty));
      var selectionWindow = new SelectionWindow(title, this.currentFeed.Podcasts);
      var startDownloading = selectionWindow.ShowDialog();

      if (startDownloading == null || !startDownloading.Value)
      {
        return;
      }

      var selectedIndexes = selectionWindow.SelectedIndexes;

      // Sort the indexes into descending order. Podcasts will be downloaded
      // in Chronological order.
      selectedIndexes.Sort((x, y) => { return y - x; });
      var podcastIndexes = new Queue<Int32>(selectedIndexes);
      var addToWinAmp = (this.AddToWinAmp.IsChecked.HasValue && this.AddToWinAmp.IsChecked.Value);
      var processingWindow = new ProcessingWindow(this.feedStorage, this.currentFeed, podcastIndexes, addToWinAmp);
      processingWindow.ShowDialog();
    }

    private void FeedList_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      var index = (sender as ListBox).SelectedIndex;
      if (index == -1)
      {
        this.FeedDescription.Text = String.Empty;
        return;
      }

      var feed = this.feedStorage.Feeds[index];
      if (feed == this.currentFeed)
      {
        return;
      }

      this.currentFeed = feed;
      this.FeedDescription.Text = feed.Description;
      this.PodcastList.ItemsSource = feed.Podcasts;
    }

    private void PodcastList_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      var index = (sender as ListBox).SelectedIndex;
      if (index == -1)
      {
        this.PodcastDescription.Text = String.Empty;
        return;
      }

      var podcast = this.currentFeed.Podcasts[index];
      var text = String.Format("{0}\r\nPUB DATE: {1}\r\nFILE SIZE: {2}\r\nDOWNLOAD DATE: {3}",
        podcast.Description,
        podcast.PubDate.ToString("ddd, dd-MMM-yyyy"),
        Miscellaneous.GetReadableFileSize(podcast.FileSize) + " MB",
        podcast.DownloadDate != DateTime.MinValue ? podcast.DownloadDate.ToString("ddd, dd-MMM-yyyy HH:mm:ss") : @"n\a");

      this.PodcastDescription.Text = text;
    }
  }
}
