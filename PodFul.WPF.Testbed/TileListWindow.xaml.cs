
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Jabberwocky.Toolkit.Path;
  using Processing;
  using ViewModel;
  using Windows;

  /// <summary>
  /// Interaction logic for TileListWindow.xaml
  /// </summary>
  public partial class TileListWindow : Window
  {
    private TileListViewModel feedCollectionViewModel;
    private Scanner scanner;
    private Int32 individualScanCount;
    private IFileDownloadProxyFactory fileDownProxyFactory;

    public TileListWindow(TileListViewModel feedCollectionViewModel, IFileDownloadProxyFactory fileDownProxyFactory)
    {
      InitializeComponent();

      this.feedCollectionViewModel = feedCollectionViewModel;
      this.FeedList.DataContext = this.feedCollectionViewModel;
    }

    private void AddFeedButtonClick(Object sender, RoutedEventArgs e)
    {

    }

    private void RemoveFeedClick(Object sender, RoutedEventArgs e)
    {

    }

    private void SettingsButtonClick(Object sender, RoutedEventArgs e)
    {

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

        var mockLogger = new MockLogger();
        var downloadManagerFactory = new DownloadManagerFactory(mockLogger);
        this.scanner = new Scanner();
        this.scanner.ScanCompletedEvent = this.FullScanCompletedEventHandler;
        this.scanner.ScanFeeds(this.feedCollectionViewModel.Feeds, downloadManagerFactory);
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

      Task.Factory.StartNew(() =>
      {
        podcastViewModel.IndividualDownload();
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
      var mockLogger = new MockLogger();
      var downloadManagerFactory = new DownloadManagerFactory(mockLogger);
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
      scanner.ScanFeeds(new[] { feedViewModel }, downloadManagerFactory);
    }
  }
}