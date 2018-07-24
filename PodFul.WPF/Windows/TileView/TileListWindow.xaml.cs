
namespace PodFul.WPF.Windows.TileView
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Reflection;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Jabberwocky.Toolkit.Assembly;
  using Jabberwocky.Toolkit.IO;
  using Jabberwocky.Toolkit.Path;
  using PodFul.Library;
  using PodFul.WPF.Logging;
  using PodFul.WPF.Miscellaneous;
  using PodFul.WPF.Processing;
  using PodFul.WPF.Processing.TileView;
  using PodFul.WPF.ViewModel.TileView;

  /// <summary>
  /// Interaction logic for TileListWindow.xaml
  /// </summary>
  public partial class TileListWindow : Window
  {
    private const String defaultImageName = "question-mark.png";
    private TileListViewModel feedCollectionViewModel;
    private Scanner scanner;
    private Int32 individualScanCount;
    private Int32 individualDownloadCount;
    private ILogController logController;
    private Settings settings;
    private String defaultImagePath;
    private String imageDirectory;

    public TileListWindow(Settings settings, String feedDirectory)
    {
      FileLogger exceptionLogger = null;
      try
      {
        exceptionLogger = new FileLogger();
        var fileLogger = new FileLogger();

        this.logController = new LogController(new Dictionary<String, ILogger>{
          { LoggerKeys.InfoKey, fileLogger },
          { LoggerKeys.ExceptionKey, exceptionLogger}});

        InitializeComponent();

        this.DisplayTitle();

        this.settings = settings;

        DirectoryOperations.EnsureDirectoryExists(feedDirectory);
        var feedStorage = new JSONFileStorage(feedDirectory);
        var feedCollection = new FeedCollection(feedStorage);
        var fileDownloadProxyFactory = new FileDownloadProxyFactory();
        this.feedCollectionViewModel = new TileListViewModel(feedCollection, fileDownloadProxyFactory);
        this.FeedList.DataContext = this.feedCollectionViewModel;

        this.imageDirectory = Path.Combine(feedDirectory, "Images");
        DirectoryOperations.EnsureDirectoryExists(this.imageDirectory);

        this.defaultImagePath = Path.Combine(feedDirectory, defaultImageName);
        if (!File.Exists(this.defaultImagePath))
        {
          Assembly.GetExecutingAssembly().CopyEmbeddedResourceToFile("PodFul.WPF.Resources." + defaultImageName, this.defaultImagePath);
        }

        fileLogger.Message("Tile Window instantiated.");
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

    private static Int32 GetCountOfExistingMediaFilesForFeed(Feed feed)
    {
      return Directory.GetFiles(feed.Directory, "*.mp3").Length;
    }

    private void AddFeedButtonClick(Object sender, RoutedEventArgs e)
    {
      // Use dialog to get the feed directory and url.
      AddFeedToken addFeedToken;
      if (!this.TryGetFeedCreationData(out addFeedToken))
      {
        return;
      }

      // Open dialog to show progress of adding new feed to the feed processor.
      var addFeedProgressWindow = new AddFeedProgressWindow(this.feedCollectionViewModel, addFeedToken);

      addFeedProgressWindow.Owner = this;
      addFeedProgressWindow.ShowDialog();
      Feed feed = addFeedProgressWindow.Feed;
      if (feed == null)
      {
        // Cancelled or Faulted - nothing more to be done.
        return;
      }

      IImageResolver imageResolver = null;
      if (this.settings.DownloadImagesWhenAddingFeeds)
      {
        imageResolver = new ImageResolver(this.imageDirectory, this.defaultImagePath);
      }

      var fileCount = GetCountOfExistingMediaFilesForFeed(feed);
      if (fileCount > 0 &&
        MessageBox.Show(String.Format("{0} MP3 file(s) found in '{1}'.\r\n\r\n Attempt to sync the feed against these files?", fileCount, feed.Directory), "Existing files found", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
      {
        var count = PodcastSynchroniser.Synchronise(feed);

        var message = String.Format("{0} MP3 file(s) synchronised after adding '{1}'", count, feed.Title);
        //this.logController.Message(LoggerKeys.InfoKey, message);
        MessageBox.Show(String.Format("{0} MP3 file(s) synchronised.", count), "Synchronisation completed", MessageBoxButton.OK, MessageBoxImage.Information);
      }

      if (imageResolver != null)
      {
        feed = imageResolver.ResolveFeedImage(feed);
      }

      try
      {
        //this.feedCollection.AddFeed(feed);
      }
      catch (Exception exception)
      {
        MessageBox.Show("Exception occurred when adding feed:\r\n\r\n" + exception.Message, "Exception occurred.");
        //this.logController.Message(LoggerKeys.ExceptionKey, "Trying to add new feed: " + exception.Message);
        return;
      }
    }

    private void DisplayTitle()
    {
      var guiVersion = Assembly.GetExecutingAssembly().GetName().Version;
      this.Title = String.Format("PodFul - v{0}.{1}.{2}",
        guiVersion.Major,
        guiVersion.Minor,
        guiVersion.Build);
    }

    private void RemoveFeedClick(Object sender, RoutedEventArgs e)
    {
      /*var dialogResult = MessageBox.Show(String.Format("Remove '{0}'?", this.currentFeed.Title), "Confirm Removal", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes);
      if (dialogResult == MessageBoxResult.No)
      {
        return;
      }

      var index = this.FeedList.SelectedIndex;
      var title = this.currentFeed.Title;
      this.feedCollection.RemoveFeed(this.currentFeed);
      this.logController.Message(LoggerKeys.InfoKey, String.Format("'{0}' removed.", title));

      if (this.feedCollectionViewModel.Count == 0)
      {
        this.FeedList.SelectedIndex = -1;
        this.CommandButton.IsEnabled = false;
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

      this.FeedList.SelectedIndex = index;*/
    }

    private void SettingsButtonClick(Object sender, RoutedEventArgs e)
    {
      var settingsWindow = new SettingsWindow(this.settings);
      settingsWindow.Owner = this;
      settingsWindow.ShowDialog();
    }

    private Boolean TryGetFeedCreationData(out AddFeedToken addFeedToken)
    {
      addFeedToken = default(AddFeedToken);

      var addFeedWindow = new AddFeedWindow();
      addFeedWindow.Owner = this;
      var dialogResult = addFeedWindow.ShowDialog();
      if (!dialogResult.HasValue || !dialogResult.Value)
      {
        return false;
      }

      addFeedToken = new AddFeedToken(addFeedWindow.FeedDirectory, addFeedWindow.FeedURL, "");
      return true;
    }

    private enum ScanStates
    {
      Idle,
      Running,
      Completed,
    }

    private ScanStates scanState = ScanStates.Idle;

    private void CommandButtonClick(Object sender, RoutedEventArgs e)
    {
      if (this.scanState == ScanStates.Idle)
      {
        this.CommandButton.Content = "Cancel All";
        this.scanState = ScanStates.Running;
        foreach (var feed in this.feedCollectionViewModel.Feeds)
        {
          feed.InitialiseForScan();
        }

        this.scanner = new Scanner();
        this.scanner.ScanCompletedEvent = this.FullScanCompletedEventHandler;
        this.scanner.ScanFeeds(this.feedCollectionViewModel.Feeds);
      }
      else if (this.scanState == ScanStates.Running)
      {
        // Cancel all feeds
        this.scanner.CancelScan();
      }
      else if (this.scanState == ScanStates.Completed)
      {
        // Reset after scanning all feeds
        this.CommandButton.Content = "Scan All";
        this.scanState = ScanStates.Idle;

        foreach (var feed in this.feedCollectionViewModel.Feeds)
        {
          feed.Reset();
        }
      }
    }

    private void FullScanCompletedEventHandler()
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.CommandButton.Content = "Reset";
        this.scanState = ScanStates.Completed;
      });
    }

    private void FeedList_SelectionChanged(Object sender, SelectionChangedEventArgs e)
    {

    }

    private void FeedList_MouseDoubleClick(Object sender, MouseButtonEventArgs e)
    {

    }

    private void FeedList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {

    }

    private void NextPageClick(Object sender, RoutedEventArgs e)
    {
      var jobNavigation = ((sender as Button).DataContext) as IPageNavigation;
      jobNavigation.MoveToNextPage();
    }

    private void LastPageClick(Object sender, RoutedEventArgs e)
    {
      var pageNavigation = ((sender as Button).DataContext) as IPageNavigation;
      pageNavigation.MoveToLastPage();
    }

    private void FirstPageClick(Object sender, RoutedEventArgs e)
    {
      var pageNavigation = ((sender as Button).DataContext) as IPageNavigation;
      pageNavigation.MoveToFirstPage();
    }

    private void PreviousPageClick(Object sender, RoutedEventArgs e)
    {
      var pageNavigation = ((sender as Button).DataContext) as IPageNavigation;
      pageNavigation.MoveToPreviousPage();
    }

    private void FeedCancelButtonClick(Object sender, RoutedEventArgs e)
    {
      var feedViewModel = (sender as Button).DataContext as FeedViewModel;
      feedViewModel.CancelScan();
    }

    private void DownloadConfirmationThresholdTextChanged(Object sender, TextChangedEventArgs e)
    {
      var feedViewModel = (sender as TextBox).DataContext as FeedViewModel;
    }

    private void FeedDirectoryPathTextChanged(Object sender, TextChangedEventArgs e)
    {
      var feedViewModel = (sender as TextBox).DataContext as FeedViewModel;
    }

    private void ChangeFeedDirectoryClick(Object sender, RoutedEventArgs e)
    {
      var feedViewModel = (sender as Button).DataContext as FeedViewModel;
      var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
      if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
      {
        return;
      }

      var selectedPath = PathOperations.CompleteDirectoryPath(folderBrowserDialog.SelectedPath);
      if (feedViewModel.FeedDirectoryPath == selectedPath)
      {
        return;
      }

      feedViewModel.FeedDirectoryPath = selectedPath;
    }

    private void CancelDownloadClick(object sender, RoutedEventArgs e)
    {
      var podcastViewModel = (PodcastViewModel)((Button)sender).DataContext;
      podcastViewModel.CancelDownload();
    }

    private void DownloadPodcastClick(Object sender, RoutedEventArgs e)
    {
      var podcastViewModel = (PodcastViewModel)((Button)sender).DataContext;
      this.CommandButton.IsEnabled = false;

      var downloadTask = Task.Factory.StartNew(() =>
      {
        this.individualDownloadCount++;
        podcastViewModel.IndividualDownload();
      });

      downloadTask.ContinueWith((task) =>
      {
        this.individualDownloadCount--;
        if (this.individualDownloadCount == 0)
        {
          Application.Current.Dispatcher.Invoke(() =>
          {
            this.CommandButton.IsEnabled = true;
          });
        }
      });
    }

    private void PodcastInfoClick(Object sender, RoutedEventArgs e)
    {
      var podcastViewModel = (PodcastViewModel)((sender as Button).DataContext);
      var podcastProperties = new PodcastProperties(podcastViewModel);
      podcastProperties.Owner = this;
      podcastProperties.ShowDialog();
    }

    private void FeedScanButtonClick(object sender, RoutedEventArgs e)
    {
      var feedViewModel = (sender as Button).DataContext as FeedViewModel;

      feedViewModel.InitialiseForScan();
      var scanner = new Scanner();
      scanner.ScanCompletedEvent = () =>
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
          feedViewModel.Reset();
          this.individualScanCount--;
          if (this.individualScanCount == 0)
          {
            this.CommandButton.IsEnabled = true;
          }
        });
      };

      this.individualScanCount++;
      this.CommandButton.IsEnabled = false;
      scanner.ScanFeeds(new[] { feedViewModel });
    }
  }
}