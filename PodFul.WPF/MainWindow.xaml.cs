
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Controls;
  using Jabberwocky.Toolkit.Assembly;
  using Jabberwocky.Toolkit.IO;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private const String defaultImageName = "Question-Mark.jpg";
    private FeedCollection feedCollection;
    private IImageResolver imageResolver;
    private IFileDeliverer fileDeliverer;
    private Feed currentFeed;
    private FileLogger fileLogger;
    private GUILogger guiLogger;

    public MainWindow()
    {
      this.fileLogger = new FileLogger();
      this.guiLogger = new GUILogger(this.fileLogger);

      InitializeComponent();

      this.DisplayTitle();

      var feedDirectory = ConfigurationManager.AppSettings["FeedDirectory"];
      var feedStorage = new JSONFileStorage(feedDirectory);
      this.feedCollection = new FeedCollection(feedStorage);

      var imageDirectory = Path.Combine(feedDirectory, "Images");
      DirectoryOperations.EnsureDirectoryExists(imageDirectory);

      String defaultImagePath = Path.Combine(feedDirectory, defaultImageName);
      if (!File.Exists(defaultImagePath))
      {
        Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("PodFul.WPF." + defaultImageName, defaultImagePath);
      }

      this.imageResolver = new ImageResolver(imageDirectory, defaultImagePath);

      this.FeedList.ItemsSource = feedCollection.Feeds;
      if (this.feedCollection.Feeds.Count > 0)
      {
        this.FeedList.SelectedIndex = 0;
        this.currentFeed = this.feedCollection.Feeds[0];
        this.ScanButton.IsEnabled = true;
      }

      this.FeedList.Focus();

      var settings = new Settings();
      this.fileDeliverer = new FileDeliverer(settings.CreateDeliveryPoints(this.guiLogger));

      this.fileLogger.Message("Main Window instantiated.");
    }

    private void DisplayTitle()
    {
      var guiVersion = Assembly.GetExecutingAssembly().GetName().Version;
      this.Title = String.Format("PodFul - v{0}.{1}.{2}",
        guiVersion.Major,
        guiVersion.Minor,
        guiVersion.Build);
    }

    private void AddFeed_Click(Object sender, RoutedEventArgs e)
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
        feed = FeedFunctions.CreateFeed(addFeedWindow.FeedURL, addFeedWindow.FeedDirectory, this.imageResolver);
        this.fileLogger.Message("'" + feed.Title + "' added. Podcasts stored in '" + feed.Directory + "'");
      }
      catch (Exception exception)
      {
        MessageBox.Show("Exception occurred when creating feed:\r\n\r\n" + exception.Message, "Exception occurred.");
        this.fileLogger.Exception("Trying to create new feed: " + exception.Message);
        return;
      }

      var fileCount = GetCountOfExistingMediaFilesForFeed(feed);
      if (fileCount > 0 &&
        MessageBox.Show(String.Format("{0} MP3 file(s) found in '{1}'.\r\n\r\n Attempt to sync the feed against these files?", fileCount, feed.Directory), "Existing files found", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
      {
        var count = this.SyncWithExistingFiles(feed);

        var message = String.Format("{0} MP3 file(s) synced after adding '{1}'", count, feed.Title);
        this.fileLogger.Message(message);
      }

      var resolvedName = this.imageResolver.GetName(feed.ImageFileName);
      feed = Feed.SetImageFileName(feed, resolvedName);

      try
      {
        this.feedCollection.AddFeed(feed);
      }
      catch (Exception exception)
      {
        MessageBox.Show("Exception occurred when adding feed:\r\n\r\n" + exception.Message, "Exception occurred.");
        this.fileLogger.Exception("Trying to add new feed: " + exception.Message);
        return;
      }

      this.FeedList.SelectedItem = feed;
      this.currentFeed = feed;
      this.ScanButton.IsEnabled = true;

      this.DisplayPodcasts();
    }

    private static Int32 GetCountOfExistingMediaFilesForFeed(Feed feed)
    {
      return Directory.GetFiles(feed.Directory, "*.mp3").Length;
    }

    private void RemoveFeed_Click(Object sender, RoutedEventArgs e)
    {
      var dialogResult = MessageBox.Show(String.Format("Remove '{0}'?", this.currentFeed.Title), "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
      if (dialogResult == MessageBoxResult.No)
      {
        return;
      }

      var index = this.FeedList.SelectedIndex;
      var title = this.currentFeed.Title;
      this.feedCollection.RemoveFeed(this.currentFeed);
      this.fileLogger.Message(String.Format("'{0}' removed.", title));

      if (this.feedCollection.Feeds.Count == 0)
      {
        this.FeedList.SelectedIndex = -1;
        this.ScanButton.IsEnabled = false;
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

    private Int32 SyncWithExistingFiles(Feed feed)
    {
      var syncCount = 0;
      for (Int32 podcastIndex = 0; podcastIndex < feed.Podcasts.Length; podcastIndex++)
      {
        var podcast = feed.Podcasts[podcastIndex];
        var fileInfo = new FileInfo(Path.Combine(feed.Directory, podcast.FileName));

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

    private void Settings_Click(Object sender, RoutedEventArgs e)
    {
      var settingsWindow = new SettingsWindow();

      settingsWindow.ShowDialog();  
    }

    private void Scan_Click(Object sender, RoutedEventArgs e)
    {
      var feedIndexes = new Queue<Int32>();
      for (Int32 i = 0; i < this.feedCollection.Feeds.Count; i++)
      {
        feedIndexes.Enqueue(i);
      }

      var feedScanner = new FeedScanner(this.feedCollection, feedIndexes, this.imageResolver, this.fileDeliverer, this.guiLogger);
      var processingWindow = new ProcessingWindow(feedScanner);

      guiLogger.PostMessage = processingWindow.PostMessage;
      feedScanner.SetWindowTitleEvent = processingWindow.SetWindowTitleEventHandler;
      feedScanner.InitialiseProgressEvent = processingWindow.InitialiseProgressEventHandler;
      feedScanner.SetCancelButtonStateEvent = processingWindow.SetCancelButtonStateEventHandler;
      feedScanner.SetProgressEvent = processingWindow.SetProgressEventHandler;
      feedScanner.ResetProgressEvent = processingWindow.ResetProgressEventHandler;

      processingWindow.ShowDialog();
    }

    private void Podcasts_Click(Object sender, RoutedEventArgs e)
    {
      this.DisplayPodcasts();
    }

    private void DisplayPodcasts()
    {
      var selectionWindow = new PodcastsWindow(this.currentFeed);
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
      var feedDownload = new FeedDownload(this.feedCollection, this.currentFeed, podcastIndexes, this.imageResolver, this.fileDeliverer, this.guiLogger);
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
        this.currentFeed = null;
        this.RemoveButton.IsEnabled = false;
        this.DownloadButton.IsEnabled = false;
        return;
      }

      this.RemoveButton.IsEnabled = true;
      this.DownloadButton.IsEnabled = true;

      var feed = this.feedCollection.Feeds[index];
      if (feed == this.currentFeed)
      {
        return;
      }

      this.currentFeed = feed;
    }

    private void FeedList_MouseDoubleClick(Object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      if (this.currentFeed != null)
      {
        this.DisplayPodcasts();
      }

      e.Handled = true;
    }

    private void ScanFeed_Click(Object sender, RoutedEventArgs e)
    {
      var feedIndexes = new Queue<Int32>();
      feedIndexes.Enqueue(this.FeedList.Items.IndexOf(this.currentFeed));
      var feedScanner = new FeedScanner(this.feedCollection, feedIndexes, this.imageResolver, this.fileDeliverer, this.guiLogger);
      var processingWindow = new ProcessingWindow(feedScanner);

      guiLogger.PostMessage = processingWindow.PostMessage;
      feedScanner.SetWindowTitleEvent = processingWindow.SetWindowTitleEventHandler;
      feedScanner.InitialiseProgressEvent = processingWindow.InitialiseProgressEventHandler;
      feedScanner.SetCancelButtonStateEvent = processingWindow.SetCancelButtonStateEventHandler;
      feedScanner.SetProgressEvent = processingWindow.SetProgressEventHandler;
      feedScanner.ResetProgressEvent = processingWindow.ResetProgressEventHandler;

      processingWindow.ShowDialog();
    }

    private void Properties_Click(Object sender, RoutedEventArgs e)
    {
      var propertiesWindow = new FeedPropertiesWindow(this.currentFeed);
      propertiesWindow.ShowDialog();
    }

    private void FeedList_MouseWheel(Object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
      this.FeedList_Scroller.ScrollToVerticalOffset(this.FeedList_Scroller.VerticalOffset - e.Delta);
      e.Handled = true;
    }

    private void Synchronise_Click(Object sender, RoutedEventArgs e)
    {
      var feedUpdated = false;
      for (int i = 0; i < this.currentFeed.Podcasts.Length; i++)
      {
        var podcast = this.currentFeed.Podcasts[i];
        var podcastFilePath = Path.Combine(this.currentFeed.Directory, podcast.URL.Substring(podcast.URL.LastIndexOf('/') + 1));
        var podcastFileInfo = new FileInfo(podcastFilePath);
        if (!podcastFileInfo.Exists)
        {
          continue;
        }

        if (podcast.FileSize != podcastFileInfo.Length)
        {
          podcast = Podcast.SetFileSize(podcastFileInfo.Length, podcast);
          feedUpdated = true;
        }

        if (podcast.DownloadDate != podcastFileInfo.LastWriteTime)
        {
          podcast = Podcast.SetDownloadDate(podcastFileInfo.LastWriteTime, podcast);
          feedUpdated = true;
        }

        this.currentFeed.Podcasts[i] = podcast;
      }

      if (feedUpdated)
      {
        this.feedCollection.UpdateFeed(this.currentFeed);
      }
    }
  }
}
