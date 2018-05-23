
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Processing;
  using ViewModel;

  /// <summary>
  /// Interaction logic for TileListWindow.xaml
  /// </summary>
  public partial class TileListWindow : Window
  {
    private TileListViewModel feedCollectionViewModel;
    private Scanner scanner;

    public TileListWindow(TileListViewModel feedCollectionViewModel)
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
        this.scanner.ScanCompletedEvent = this.ScanCompletedEventHandler;
        this.scanner.ScanFeeds(this.feedCollectionViewModel.Feeds, downloadManagerFactory);
      }
      else if (this.scanState == ScanStates.Running)
      {
        // Start cancelling all feeds
        this.scanner.CancelScanning();
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

    private void ScanCompletedEventHandler()
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
  }
}
