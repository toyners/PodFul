
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
    private Boolean isLoaded;
    private DownloadManager downloadManager;

    public PodcastDownloadWindow(DownloadManager downloadManager)
    {
      InitializeComponent();

      this.downloadManager = downloadManager;

      this.PodcastList.ItemsSource = downloadManager.Podcasts;
    }

    private void CancelAll_Click(Object sender, RoutedEventArgs e)
    {
    }

    private void FeedList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {
    }

    private void Window_Loaded(Object sender, RoutedEventArgs e)
    {
      if (!this.isLoaded)
      {
        // Ensure this is only called once.
        this.downloadManager.DownloadNextPodcast();
        this.isLoaded = true;
      }
    }
  }
}
