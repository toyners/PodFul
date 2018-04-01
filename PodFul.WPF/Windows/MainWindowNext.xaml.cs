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
using PodFul.Library;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for MainWindowNext.xaml
  /// </summary>
  public partial class MainWindowNext : Window
  {
    public MainWindowNext()
    {
      InitializeComponent();
    }

    private void AddFeedButtonClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void FullScanButtonClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void RemoveFeedClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void SettingsButtonClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }
  }

  public interface IPodFul
  {
    IList<Feed> Feeds { get; }
    Action<Int32> StartedAddingFeed { get; set; }

    Feed AddFeed(String directory, String url);
    void RemoveFeed(Feed feed);
    void RemoveFeed(Int32 index);
    void ScanFeeds(IList<Int32> indexes);
  }
}
