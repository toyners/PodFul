
namespace PodFul.WPF
{
  using System;
  using System.Threading.Tasks;
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
    private Int32 podcastCount;

    public PodcastDownloadWindow(DownloadManager downloadManager)
    {
      InitializeComponent();

      this.downloadManager = downloadManager;

      this.PodcastList.ItemsSource = downloadManager.Podcasts;
      this.podcastCount = downloadManager.Podcasts.Count;
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
        this.StartPodcastDownload();
        this.isLoaded = true;
      }
    }

    private void StartPodcastDownload()
    {
      this.downloadManager.DownloadNextPodcast(this.PodcastDownloadCompleted);
      
    }

    private void PodcastDownloadCompleted(Task task)
    {
      if (--this.podcastCount == 0)
      {
        // Turn off cancel all button
        return;
      }


      this.StartPodcastDownload();
    }
  }
}
