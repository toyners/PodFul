
namespace PodFul.WPF
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using System.Windows;
  using System.Windows.Input;

  /// <summary>
  /// Interaction logic for PodcastDownloadWindow.xaml
  /// </summary>
  public partial class PodcastDownloadWindow : Window
  {
    public PodcastDownloadWindow(ObservableCollection<PodcastMonitor> podcasts)
    {
      InitializeComponent();

      this.PodcastList.ItemsSource = podcasts;
    }

    private void CancelAll_Click(Object sender, RoutedEventArgs e)
    {
    }

    private void FeedList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {
    }
  }

  public class PodcastMonitor
  {
    public String Name;

    public String ProgressMajorSize;

    public String ProgressMinorSize;

    public String ProgressUnit;

    public String FilePath;

    public String URL;

    public CancellationToken CancellationToken;

    public void ProgressEventHandler(int bytesRead)
    {

    }
  }
}
