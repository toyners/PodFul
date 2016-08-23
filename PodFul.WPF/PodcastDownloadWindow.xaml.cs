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

namespace PodFul.WPF
{
  /// <summary>
  /// Interaction logic for PodcastDownloadWindow.xaml
  /// </summary>
  public partial class PodcastDownloadWindow : Window
  {
    public PodcastDownloadWindow()
    {
      InitializeComponent();
    }

    private void CancelAll_Click(Object sender, RoutedEventArgs e)
    {
    }

    private void FeedList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {
    }
  }
}
