
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

    private void CommandButtonClick(Object sender, RoutedEventArgs e)
    {
      var mockLogger = new MockLogger();
      var downloadManagerFactory = new DownloadManagerFactory(mockLogger);
      var scanner = new Scanner();
      scanner.ScanFeeds(this.feedCollectionViewModel.Feeds, downloadManagerFactory);
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
      throw new NotImplementedException();
    }
  }
}
