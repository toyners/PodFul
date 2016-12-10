﻿
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Reflection;
  using System.Threading;
  using System.Windows;
  using System.Windows.Controls;
  using Jabberwocky.Toolkit.Assembly;
  using Jabberwocky.Toolkit.IO;
  using Logging;
  using Miscellaneous;
  using PodFul.FileDelivery;
  using PodFul.Library;
  using PodFul.WPF.Processing;
  using Windows;

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
    private FileDeliveryLogger fileDeliveryLogger;
    private Timer contextMenuTimer;
    private Settings settings;
    private IPodcastDownloadConfirmer podcastDownloadConfirmer;

    public MainWindow()
    {
      try
      {
        this.fileLogger = new FileLogger();
        this.guiLogger = new GUILogger(this.fileLogger);
        this.fileDeliveryLogger = new FileDeliveryLogger();

        InitializeComponent();

        this.DisplayTitle();

        this.settings = new Settings(ConfigurationManager.AppSettings["SettingsPath"], this.fileDeliveryLogger);

        var feedDirectory = ConfigurationManager.AppSettings["FeedDirectory"];
        DirectoryOperations.EnsureDirectoryExists(feedDirectory);
        var feedStorage = new JSONFileStorage(feedDirectory);
        this.feedCollection = new FeedCollection(feedStorage);

        var imageDirectory = Path.Combine(feedDirectory, "Images");
        DirectoryOperations.EnsureDirectoryExists(imageDirectory);

        String defaultImagePath = Path.Combine(feedDirectory, defaultImageName);
        if (!File.Exists(defaultImagePath))
        {
          Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("PodFul.WPF." + defaultImageName, defaultImagePath);
        }

        this.imageResolver = new ImageResolver(imageDirectory, defaultImagePath, true);

        this.FeedList.ItemsSource = feedCollection.Feeds;
        if (this.feedCollection.Feeds.Count > 0)
        {
          this.FeedList.SelectedIndex = 0;
          this.currentFeed = this.feedCollection.Feeds[0];
          this.ScanButton.IsEnabled = true;
        }

        this.FeedList.Focus();

        this.fileDeliverer = new FileDeliverer(settings.DeliveryPoints);

        this.podcastDownloadConfirmer = new PodcastDownloadConfirmer(settings.ConfirmPodcastDownloadThreshold);

        this.fileLogger.Message("Main Window instantiated.");
      }
      catch (Exception exception)
      {
        this.fileLogger.Exception(exception.Message + ": " + exception.StackTrace);
        var message = String.Format("Exception occurred during startup. Exception message is\r\n{0}\r\n\r\nPodFul will close.", exception.Message);
        MessageBox.Show(message, "PodFul Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        throw;
      }
    }

    private static Int32 GetCountOfExistingMediaFilesForFeed(Feed feed)
    {
      return Directory.GetFiles(feed.Directory, "*.mp3").Length;
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
      addFeedWindow.Owner = this;
      var dialogResult = addFeedWindow.ShowDialog();
      if (!dialogResult.HasValue || !dialogResult.Value)
      {
        return;
      }

      var addFeedProgressWindow = new AddFeedProgressWindow(addFeedWindow.FeedURL, addFeedWindow.FeedDirectory, this.imageResolver, this.fileLogger);
      addFeedProgressWindow.Owner = this;
      addFeedProgressWindow.ShowDialog();
      Feed feed = addFeedProgressWindow.Feed;
      if (feed == null)
      {
        // Cancelled or Faulted - nothing more to be done.
        return;
      }

      var fileCount = GetCountOfExistingMediaFilesForFeed(feed);
      if (fileCount > 0 &&
        MessageBox.Show(String.Format("{0} MP3 file(s) found in '{1}'.\r\n\r\n Attempt to sync the feed against these files?", fileCount, feed.Directory), "Existing files found", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
      {
        var count = PodcastSynchroniser.Synchronise(feed);

        var message = String.Format("{0} MP3 file(s) synced after adding '{1}'", count, feed.Title);
        this.fileLogger.Message(message);
      }

      var imagePath = this.imageResolver.GetName(feed.ImageFileName, feed.ImageURL);
      feed = Feed.SetImageFileName(feed, imagePath);
      
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

    private void Settings_Click(Object sender, RoutedEventArgs e)
    {
      var settingsWindow = new SettingsWindow(this.settings);
      settingsWindow.ShowDialog();  
    }

    private Queue<Int32> GetIndexForCurrentFeed()
    {
      var feedIndexes = new Queue<Int32>();
      feedIndexes.Enqueue(this.FeedList.Items.IndexOf(this.currentFeed));
      return feedIndexes;
    }

    private Queue<Int32> GetIndexesForAllFeeds()
    {
      var feedIndexes = new Queue<Int32>();
      for (Int32 i = 0; i < this.feedCollection.Feeds.Count; i++)
      {
        feedIndexes.Enqueue(i);
      }

      return feedIndexes;
    }

    private void Podcasts_Click(Object sender, RoutedEventArgs e)
    {
      this.DisplayPodcastsWithPause();
    }

    private void DownloadPodcastsCallback(Object state)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.contextMenuTimer = null;

        var selectedIndexes = this.GetSelectedPodcasts();
        if (selectedIndexes != null)
        {
          this.DownloadPodcasts(selectedIndexes);
        }
      });
    }

    private List<Int32> GetSelectedPodcasts()
    {
      var selectionWindow = new PodcastsWindow(this.currentFeed);
      var startDownloading = selectionWindow.ShowDialog();

      if (startDownloading == null || !startDownloading.Value)
      {
        return null;
      }

      return selectionWindow.SelectedIndexes;
    }

    private void DisplayPodcastsWithPause()
    {
      this.contextMenuTimer = new Timer(DownloadPodcastsCallback, null, 200, Timeout.Infinite);
    }

    private void DisplayPodcasts()
    {
      var selectedIndexes = this.GetSelectedPodcasts();
      if (selectedIndexes != null)
      {
        this.DownloadPodcasts(selectedIndexes);
      }
    }

    private void DownloadPodcasts(List<Int32> selectedIndexes)
    {
      // Sort the indexes into descending order. Podcasts will be downloaded
      // in Chronological order.
      selectedIndexes.Sort((x, y) => { return y - x; });

      var podcastDownloadManager = new DownloadManager(this.guiLogger, this.settings.ConcurrentDownloadCount);
      podcastDownloadManager.AddJobs(this.CreateDownloadJobs(selectedIndexes));

      var podcastDownloadWindow = new PodcastDownloadWindow(podcastDownloadManager);
      podcastDownloadWindow.ShowDialog();
    }

    private IEnumerable<DownloadJob> CreateDownloadJobs(List<Int32> podcastIndexes)
    {
      List<DownloadJob> jobs = new List<DownloadJob>();
      foreach (var index in podcastIndexes)
      { 
        var podcast = this.currentFeed.Podcasts[index];
        // No file delivery for manual downloading so file deliverer parameter is null.
        var downloadJob = new DownloadJob(podcast, this.currentFeed, this.feedCollection, null, this.imageResolver);

        jobs.Add(downloadJob);
      }

      return jobs;
    }

    private void FeedList_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {
      var index = (sender as ListBox).SelectedIndex;
      if (index == -1)
      {
        this.currentFeed = null;
        this.RemoveButton.IsEnabled = false;
        this.PodcastsButton.IsEnabled = false;
        return;
      }

      this.RemoveButton.IsEnabled = true;
      this.PodcastsButton.IsEnabled = true;

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

    private void PerformScan(Queue<Int32> feedIndexes)
    {
      var downloadManager = new DownloadManager(this.guiLogger, this.settings.ConcurrentDownloadCount);
      var feedScanner = new FeedScanner(this.feedCollection, feedIndexes, this.imageResolver, this.fileDeliverer, this.fileDeliveryLogger, this.guiLogger, this.podcastDownloadConfirmer, downloadManager);
      var scanningWindow = ScanningWindow.CreateWindow(feedScanner, this.guiLogger, this.imageResolver);
      scanningWindow.ShowDialog();
    }

    private void Properties_Click(Object sender, RoutedEventArgs e)
    {
      var propertiesWindow = new FeedPropertiesWindow(this.currentFeed);
      propertiesWindow.ShowDialog();
    }

    private void Scan_Click(Object sender, RoutedEventArgs e)
    {
      this.PerformScan(this.GetIndexesForAllFeeds());
    }

    private void ScanFeed_Click(Object sender, RoutedEventArgs e)
    {
      this.PerformScan(this.GetIndexForCurrentFeed());
    }

    private void Synchronise_Click(Object sender, RoutedEventArgs e)
    {
      var count = PodcastSynchroniser.Synchronise(this.currentFeed);

      if (count > 0)
      {
        this.feedCollection.UpdateFeed(this.currentFeed);
        MessageBox.Show(String.Format("{0} MP3 file(s) snychronised.", count), "Synchronisation completed", MessageBoxButton.OK, MessageBoxImage.Information);
      }
      else
      {
        MessageBox.Show("No files synchronised", "Synchronisation completed");
      }
    }

    private void FeedList_MouseWheel(Object sender, System.Windows.Input.MouseWheelEventArgs e)
    {
      this.FeedList_Scroller.ScrollToVerticalOffset(this.FeedList_Scroller.VerticalOffset - e.Delta);
      e.Handled = true;
    }
  }
}
