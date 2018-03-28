
namespace PodFul.WPF.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Controls;
  using Jabberwocky.Toolkit.Assembly;
  using Jabberwocky.Toolkit.IO;
  using Jabberwocky.Toolkit.Path;
  using Logging;
  using Miscellaneous;
  using PodFul.FileDelivery;
  using PodFul.Library;
  using PodFul.WPF.Processing;

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    #region Fields
    public const String CombinedKey = "INFO+UI";
    public const String ExceptionKey = "EXCEPTION";
    public const String InfoKey = "INFO";
    public const String UiKey = "UI";

    private const String defaultImageName = "Question-Mark.jpg";
    private FeedCollection feedCollection;
    private Feed currentFeed;
    private LogController logController;
    private IFileDeliveryLogger fileDeliveryLogger;
    private Settings settings;
    private String defaultImagePath;
    private String imageDirectory;
    #endregion

    #region Construction
    public MainWindow()
    {
      FileLogger exceptionLogger = null;
      try
      {
        var fileLogger = new FileLogger();
        var guiLogger = new UILogger();
        var combinedLogger = new CombinedLogger(fileLogger, guiLogger);
        this.fileDeliveryLogger = new FileDeliveryLogger();
        exceptionLogger = new FileLogger();

        this.logController = new LogController(new Dictionary<String, ILogger>{
          { InfoKey, fileLogger },
          { ExceptionKey, exceptionLogger},
          { CombinedKey, combinedLogger },
          { UiKey, guiLogger}});

        InitializeComponent();

        this.DisplayTitle();

        this.settings = new Settings(ConfigurationManager.AppSettings["SettingsPath"]);

        var feedDirectory = PathOperations.CompleteDirectoryPath(ConfigurationManager.AppSettings["FeedDirectory"]);
        DirectoryOperations.EnsureDirectoryExists(feedDirectory);
        var feedStorage = new JSONFileStorage(feedDirectory);
        this.feedCollection = new FeedCollection(feedStorage);

        this.imageDirectory = Path.Combine(feedDirectory, "Images");
        DirectoryOperations.EnsureDirectoryExists(this.imageDirectory);

        this.defaultImagePath = Path.Combine(feedDirectory, defaultImageName);
        if (!File.Exists(this.defaultImagePath))
        {
          Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("PodFul.WPF." + defaultImageName, this.defaultImagePath);
        }

        this.FeedList.ItemsSource = feedCollection.ObservableFeeds;
        if (this.feedCollection.Count > 0)
        {
          this.FeedList.SelectedIndex = 0;
          this.currentFeed = this.feedCollection[0];
          this.ScanButton.IsEnabled = true;
        }

        this.FeedList.Focus();

        fileLogger.Message("Main Window instantiated.");
      }
      catch (Exception exception)
      {
        var fullExceptionMessage = exception.Message + ": " + exception.StackTrace;
        exceptionLogger?.Message(fullExceptionMessage);

        var message = String.Format("Exception occurred during startup. Exception message is\r\n{0}\r\n\r\nPodFul will close.", exception.Message);
        MessageBox.Show(message, "PodFul Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        throw;
      }
    }
    #endregion

    #region Methods
    private static Int32 GetCountOfExistingMediaFilesForFeed(Feed feed)
    {
      return Directory.GetFiles(feed.Directory, "*.mp3").Length;
    }

    private IFileDeliverer CreateFileDeliverer()
    {
      var deliveryPoints = DeliveryPointCreator.CreateDeliveryPoints(this.settings.DeliveryPointData, this.fileDeliveryLogger, this.fileDeliveryLogger);
      return new FileDeliverer(deliveryPoints);
    }

    private void DisplayTitle()
    {
      var guiVersion = Assembly.GetExecutingAssembly().GetName().Version;
      this.Title = String.Format("PodFul - v{0}.{1}.{2}",
        guiVersion.Major,
        guiVersion.Minor,
        guiVersion.Build);
    }

    private void AddFeedButtonClick(Object sender, RoutedEventArgs e)
    {
      var addFeedWindow = new AddFeedWindow();
      addFeedWindow.Owner = this;
      var dialogResult = addFeedWindow.ShowDialog();
      if (!dialogResult.HasValue || !dialogResult.Value)
      {
        return;
      }

      IFeedFactory feedFactory = new FeedFactory(addFeedWindow.FeedDirectory, addFeedWindow.FeedURL, this.defaultImagePath);
      IImageResolver imageResolver = null;
      if (this.settings.DownloadImagesWhenAddingFeeds)
      {
        imageResolver = new ImageResolver(this.imageDirectory, this.defaultImagePath);
      }

      var addFeedProgressWindow = new AddFeedProgressWindow(
        feedFactory,
        this.logController,
        imageResolver);

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

        var message = String.Format("{0} MP3 file(s) synchronised after adding '{1}'", count, feed.Title);
        this.logController.Message(InfoKey, message);
        MessageBox.Show(String.Format("{0} MP3 file(s) synchronised.", count), "Synchronisation completed", MessageBoxButton.OK, MessageBoxImage.Information);
      }

      if (imageResolver != null)
      {
        feed = imageResolver.ResolveFeedImage(feed);
      }

      try
      {
        this.feedCollection.AddFeed(feed);
      }
      catch (Exception exception)
      {
        MessageBox.Show("Exception occurred when adding feed:\r\n\r\n" + exception.Message, "Exception occurred.");
        this.logController.Message(ExceptionKey, "Trying to add new feed: " + exception.Message);
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
      this.logController.Message(InfoKey, String.Format("'{0}' removed.", title));

      if (this.feedCollection.Count == 0)
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

    private void SettingsButtonClick(Object sender, RoutedEventArgs e)
    {
      var settingsWindow = new SettingsWindow(this.settings);
      settingsWindow.Owner = this;
      settingsWindow.ShowDialog();

    }

    private Queue<Int32> GetIndexForCurrentFeed()
    {
      var feedIndexes = new Queue<Int32>();
      feedIndexes.Enqueue(this.FeedList.Items.IndexOf(this.currentFeed));
      return feedIndexes;
    }

    private Queue<Int32> GetIndexesForAllEnabledFeeds()
    {
      var feedIndexes = this.feedCollection.ObservableFeeds.Select((feed, index) => new { Index = index, Value = feed })
        .Select(x => x.Index);

      return new Queue<int>(feedIndexes.ToList());
    }

    private void Podcasts_Click(Object sender, RoutedEventArgs e)
    {
      this.DisplayPodcasts();
    }

    private List<Int32> GetSelectedPodcasts(out Boolean deliverManualDownloadsToDeliveryPoints)
    {
      var selectionWindow = new PodcastsWindow(this.currentFeed, this.settings.DeliverManualDownloadsToDeliveryPoints);
      selectionWindow.Owner = this;
      var startDownloading = selectionWindow.ShowDialog();
      deliverManualDownloadsToDeliveryPoints = selectionWindow.DeliverManualDownloadsToDeliveryPoints;
      if (startDownloading == null || !startDownloading.Value)
      {
        return null;
      }

      return selectionWindow.SelectedIndexes;
    }

    private void DisplayPodcasts()
    {
      Boolean deliverManualDownloadsToDeliveryPoints = false;
      var selectedIndexes = this.GetSelectedPodcasts(out deliverManualDownloadsToDeliveryPoints);
      if (selectedIndexes != null && selectedIndexes.Count > 0)
      {
        // Sort the indexes into descending order. Podcasts will be downloaded
        // in Chronological order.
        selectedIndexes.Sort((x, y) => { return y - x; });
        IImageResolver imageResolver = this.CreateImageResolver();
        var jobs = JobCollectionFactory.CreateJobsFromSelectedIndexesOfFeed(this.currentFeed, selectedIndexes, this.feedCollection, imageResolver);
        this.DownloadPodcasts(jobs, deliverManualDownloadsToDeliveryPoints);
      }
    }

    private void DownloadPodcasts(IEnumerable<DownloadJob> jobs, Boolean deliverManualDownloadsToDeliveryPoints)
    {
      var fileDeliverer = (deliverManualDownloadsToDeliveryPoints ? this.CreateFileDeliverer() : null);
      var jobNeedsLocationEventHandler = JobNeedsLocationEventHandlerFactory.CreateJobNeedsLocationEventHandler(null);
      var downloadManager = DownloadManager.Create(this.logController.GetLogger(CombinedKey), this.settings.ConcurrentDownloadCount, jobNeedsLocationEventHandler, fileDeliverer);
      var podcastDownloadWindow = new PodcastDownloadWindow(downloadManager, this.settings.HideCompletedJobs);

      // Add the jobs after creating the window so that job queued event will fire.
      downloadManager.AddJobs(jobs);

      podcastDownloadWindow.Owner = this;
      podcastDownloadWindow.ShowDialog();
    }

    private IImageResolver CreateImageResolver()
    {
      IImageResolver imageResolver = new ImageResolver(this.imageDirectory, this.defaultImagePath);
      imageResolver.CompletedDownloadNotificationEvent += (downloadNumber, imageURL) =>
      {
        var message = "Downloaded '" + imageURL + "'";
        this.logController.GetLogger<FileLogger>(InfoKey).Message(message);
      };

      imageResolver.FailedDownloadNotificationEvent += (imageURL, exception) =>
      {
        var message = "Failed to download '" + imageURL + "'. Exception: " + exception.Message;
        this.logController.GetLogger<FileLogger>(InfoKey).Message(message);
      };

      return imageResolver;
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

      var feed = this.feedCollection[index];
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
      var fileDeliverer = this.CreateFileDeliverer();
      var combinedLogger = this.logController.GetLogger(CombinedKey);
      var downloadManager = DownloadManager.Create(combinedLogger, this.settings.ConcurrentDownloadCount, null, fileDeliverer);

      IImageResolver imageResolver = this.CreateImageResolver();
      var feedScanner = FeedScanner.Create(this.feedCollection, feedIndexes, imageResolver, this.fileDeliveryLogger, this.logController, downloadManager);

      var scanningWindow = new ScanningWindow((UInt32)feedIndexes.Count, 
        feedScanner, 
        downloadManager, 
        this.settings.HideCompletedJobs);

      var logger = this.logController.GetLogger<UILogger>(UiKey);
      logger.PostMessage = scanningWindow.PostMessage;

      scanningWindow.Owner = this;
      scanningWindow.ShowDialog();

      if (!downloadManager.GotFailedJobs)
      {
        return;
      }

      var retryWindow = new RetryWindow(downloadManager.FailedJobs);
      var dialogResult = retryWindow.ShowDialog();
      if (!dialogResult.GetValueOrDefault())
      {
        return;
      }

      var retryJobs = JobCollectionFactory.FilterJobsByIndex(downloadManager.FailedJobs, retryWindow.RetryJobIndexes);

      this.DownloadPodcasts(retryJobs, true);
    }

    private void PropertiesMenuItemClick(Object sender, RoutedEventArgs e)
    {
      var propertiesWindow = new FeedPropertiesWindow(this.currentFeed);
      propertiesWindow.Owner = this;

      var isDirty = propertiesWindow.ShowDialog();
      Feed updatedFeed = null;
      if (isDirty.HasValue && isDirty.Value)
      {
        // Properties window can be dirty without the actual data changing (i.e. user put the same
        // value in). So only update the feed if something has really changed.

        if (this.currentFeed.Directory != propertiesWindow.DirectoryPath.Text)
        {
          updatedFeed = Feed.SetDirectory(propertiesWindow.DirectoryPath.Text, this.currentFeed);
        }

        if (this.currentFeed.DoScan != propertiesWindow.DoScan ||
            this.currentFeed.CompleteDownloadsOnScan != propertiesWindow.DoDownload ||
            this.currentFeed.DeliverDownloadsOnScan != propertiesWindow.DoDelivery)
        {
          updatedFeed = Feed.SetScanningFlags(propertiesWindow.DoScan,
            propertiesWindow.DoDownload,
            propertiesWindow.DoDelivery,
            this.currentFeed);
        }

        UInt32 value;
        if (UInt32.TryParse(propertiesWindow.ConfirmDownloadThreshold.Text, out value) && this.currentFeed.ConfirmDownloadThreshold != value)
        {
          updatedFeed = Feed.SetConfirmDownloadThreshold(value, this.currentFeed);
        }
      }

      if (updatedFeed != null)
      {
        this.currentFeed = updatedFeed;
        this.feedCollection.UpdateFeedContent(this.currentFeed);
        var index = this.feedCollection.ObservableFeeds.IndexOf(this.currentFeed);
        this.feedCollection[index] = this.currentFeed;
      }
    }

    private void FullScanButtonClick(Object sender, RoutedEventArgs e)
    {
      this.PerformScan(this.GetIndexesForAllEnabledFeeds());
    }

    private void ScanFeedContextMenuClick(Object sender, RoutedEventArgs e)
    {
      this.PerformScan(this.GetIndexForCurrentFeed());
    }

    private void Synchronise_Click(Object sender, RoutedEventArgs e)
    {
      var count = PodcastSynchroniser.Synchronise(this.currentFeed);

      if (count > 0)
      {
        this.feedCollection.UpdateFeedContent(this.currentFeed);
        MessageBox.Show(String.Format("{0} MP3 file(s) synchronised.", count), "Synchronisation completed", MessageBoxButton.OK, MessageBoxImage.Information);
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
    #endregion
  }
}
