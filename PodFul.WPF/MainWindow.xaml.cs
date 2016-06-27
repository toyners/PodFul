
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Controls;
  using Jabberwocky.Toolkit.IO;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private FeedCollection feedCollection;
    private IImageResolver imageResolver;
    private IFileDeliverer fileDeliverer;
    private Feed currentFeed;
    private ILogger logger;

    public MainWindow()
    {
      this.logger = new FileLogger();

      InitializeComponent();

      this.DisplayTitle();

      var feedDirectory = ConfigurationManager.AppSettings["FeedDirectory"];
      this.feedCollection = new FeedCollection(feedDirectory);

      var imageDirectory = Path.Combine(feedDirectory, "Images");
      this.imageResolver = new ImageResolver(imageDirectory);
      DirectoryOperations.EnsureDirectoryExists(imageDirectory);

      this.FeedList.ItemsSource = feedCollection.Feeds;
      this.FeedList.SelectedIndex = 0;
      if (this.feedCollection.Feeds.Count > 0)
      {
        this.currentFeed = this.feedCollection.Feeds[0];
      }

      this.FeedList.Focus();

      var settings = new Settings();
      this.fileDeliverer = new FileDeliverer(settings.CreateDeliveryPoints(logger));

      this.logger.Message("Main Window instantiated.");
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
        this.logger.Message("'" + feed.Title + "' added. Podcasts stored in '" + feed.Directory + "'");
      }
      catch (Exception exception)
      {
        MessageBox.Show("Exception occurred when adding feed:\r\n\r\n" + exception.Message, "Exception occurred.");
        this.logger.Exception("Trying to add new feed: " + exception.Message);
        return;
      }

      var fileCount = GetCountOfExistingMediaFilesForFeed(feed);
      if (fileCount > 0 &&
        MessageBox.Show(String.Format("{0} mp3 file(s) found in '{1}'.\r\n\r\n Attempt to sync the feed against these files?", fileCount, feed.Directory), "Existing files found", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
      {
        this.SyncWithExistingFiles(feed);
      }

      var resolvedName = this.imageResolver.GetName(feed.ImageFileName);
      feed = Feed.SetImageFileName(feed, resolvedName);

      this.feedCollection.AddFeed(feed);
      this.FeedList.SelectedItem = feed;
      this.currentFeed = feed;

      this.DownloadPodcasts();
    }

    private static Int32 GetCountOfExistingMediaFilesForFeed(Feed feed)
    {
      return Directory.GetFiles(feed.Directory, "*.mp3").Length;
    }

    private void removeButton_Click(Object sender, RoutedEventArgs e)
    {
      var index = this.FeedList.SelectedIndex;

      this.feedCollection.RemoveFeed(this.currentFeed);

      if (this.feedCollection.Feeds.Count == 0)
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

        podcast = Podcast.SetDownloadDate(fileInfo.CreationTime, podcast);
        podcast = Podcast.SetFileSize(fileInfo.Length, podcast);
        feed.Podcasts[podcastIndex] = podcast;
        syncCount++;
      }

      return syncCount;
    }

    private void SettingsButton_Click(Object sender, RoutedEventArgs e)
    {
      var settingsWindow = new SettingsWindow();

      settingsWindow.ShowDialog();  
    }

    private void scanButton_Click(Object sender, RoutedEventArgs e)
    {
      var selectionWindow = new SelectionWindow(this.feedCollection.Feeds);
      var startScanning = selectionWindow.ShowDialog();

      if (startScanning == null || !startScanning.Value)
      {
        return;
      }

      var feedIndexes = new Queue<Int32>(selectionWindow.SelectedIndexes);
      var guiLogger = new GUILogger(this.logger);
      var feedScanner = new FeedScanner(this.feedCollection, feedIndexes, this.imageResolver, this.fileDeliverer, guiLogger);
      var processingWindow = new ProcessingWindow(feedScanner);

      guiLogger.PostMessage = processingWindow.PostMessage;
      feedScanner.SetWindowTitleEvent = processingWindow.SetWindowTitleEventHandler;
      feedScanner.InitialiseProgressEvent = processingWindow.InitialiseProgressEventHandler;
      feedScanner.SetCancelButtonStateEvent = processingWindow.SetCancelButtonStateEventHandler;
      feedScanner.SetProgressEvent = processingWindow.SetProgressEventHandler;
      feedScanner.ResetProgressEvent = processingWindow.ResetProgressEventHandler;

      processingWindow.ShowDialog();
    }

    private void downloadButton_Click(Object sender, RoutedEventArgs e)
    {
      this.DownloadPodcasts();
    }

    private void DownloadPodcasts()
    {
      var selectionWindow = new SelectionWindow(this.currentFeed.Podcasts);
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
      var guiLogger = new GUILogger(this.logger);
      var feedDownload = new FeedDownload(this.feedCollection, this.currentFeed, podcastIndexes, this.fileDeliverer, guiLogger);
      var processingWindow = new ProcessingWindow(feedDownload);

      guiLogger.PostMessage = processingWindow.PostMessage;
      feedDownload.SetWindowTitleEvent = processingWindow.SetWindowTitleEventHandler;
      feedDownload.InitialiseProgressEvent = processingWindow.InitialiseProgressEventHandler;
      feedDownload.SetCancelButtonStateEvent = processingWindow.SetCancelButtonStateEventHandler;
      feedDownload.SetProgressEvent = processingWindow.SetProgressEventHandler;
      feedDownload.ResetProgressEvent = processingWindow.ResetProgressEventHandler;

      processingWindow.ShowDialog();
    }

    private void FeedList_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      var index = (sender as ListBox).SelectedIndex;
      if (index == -1)
      {
        return;
      }

      var feed = this.feedCollection.Feeds[index];
      if (feed == this.currentFeed)
      {
        return;
      }

      this.currentFeed = feed;
    }
  }
}
