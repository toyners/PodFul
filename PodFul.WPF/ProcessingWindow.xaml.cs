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

namespace PodFul.WPF
{
  /// <summary>
  /// Interaction logic for ProcessingWindow.xaml
  /// </summary>
  public partial class ProcessingWindow : Window
  {
    public ProcessingWindow(IFeedStorage feedStorage, Queue<Int32> feedIndexes)
    {
      InitializeComponent();
    }

    public ProcessingWindow(IFeedStorage feedStorage, Feed feed, Queue<Int32> podcastIndexes)
    {
      InitializeComponent();
    }

    private void Cancel_Click(Object sender, RoutedEventArgs e)
    {

    }
  }
}
