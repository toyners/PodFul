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
using PodFul.WPF.ViewModel;

namespace PodFul.WPF.Testbed
{
  /// <summary>
  /// Interaction logic for TileListWindow.xaml
  /// </summary>
  public partial class TileListWindow : Window
  {
    private IFeedCollectionViewModel feedCollectionViewModel;

    public TileListWindow(IFeedCollectionViewModel feedCollectionViewModel)
    {
      InitializeComponent();

      this.feedCollectionViewModel = feedCollectionViewModel;
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

    private void FullScanButtonClick(Object sender, RoutedEventArgs e)
    {

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
  }
}
