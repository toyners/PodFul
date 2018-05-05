using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Jabberwocky.Toolkit.Object;
using PodFul.WPF.Logging;
using PodFul.WPF.Processing;
using PodFul.WPF.Testbed.Processing;
using PodFul.WPF.Testbed.ViewModel;
using PodFul.WPF.ViewModel;

namespace PodFul.WPF.Testbed
{
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
      var pageNavigation = ((sender as Button).DataContext) as PodcastPageNavigation;
      if (pageNavigation != null)
      {
        pageNavigation.MoveToNextPage();
        return;
      }

      var jobNavigation = ((sender as Button).DataContext) as JobPageNavigation;
      jobNavigation.MoveToNextPage();
    }

    private void LastPageClick(Object sender, RoutedEventArgs e)
    {
      var pageNavigation = ((sender as Button).DataContext) as PodcastPageNavigation;
      pageNavigation.MoveToLastPage();
    }

    private void FirstPageClick(Object sender, RoutedEventArgs e)
    {
      var pageNavigation = ((sender as Button).DataContext) as PodcastPageNavigation;
      pageNavigation.MoveToFirstPage();
    }

    private void PreviousPageClick(Object sender, RoutedEventArgs e)
    {
      var pageNavigation = ((sender as Button).DataContext) as PodcastPageNavigation;
      pageNavigation.MoveToPreviousPage();
    }

    private void FeedCancelButtonClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }
  }
}
