
namespace PodFul.WPF
{
  using System;
  using System.Collections.ObjectModel;
  using System.Windows;
  using System.Windows.Input;
  using PodFul.WPF.Processing;

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
}
