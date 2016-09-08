
namespace PodFul.WPF
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using PodFul.WPF.Processing;

  /// <summary>
  /// Interaction logic for PodcastDownloadWindow.xaml
  /// </summary>
  public partial class PodcastDownloadWindow : Window
  {
    #region Fields
    private Boolean isLoaded;
    private DownloadManager downloadManager;
    private Int32 podcastCount;
    private Int32 threadCount = 1;
    #endregion

    #region Construction
    public PodcastDownloadWindow(DownloadManager downloadManager)
    {
      InitializeComponent();

      this.downloadManager = downloadManager;

      this.PodcastList.ItemsSource = downloadManager.Podcasts;
      this.podcastCount = downloadManager.Podcasts.Count;
    }
    #endregion

    #region Methods
    private void CancelAllDownloads_Click(Object sender, RoutedEventArgs e)
    {
      foreach (var podcast in this.downloadManager.Podcasts)
      {
        podcast.CancelDownload();
      }
    }

    private void FeedList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {
    }

    private void Window_Loaded(Object sender, RoutedEventArgs e)
    {
      if (!this.isLoaded)
      {
        for (Int32 i = 0; i < this.threadCount; i++)
        {
          this.StartPodcastDownload();
        }

        this.CancelAll.IsEnabled = true;
       
        // Ensure this functionality is only called once.
        this.isLoaded = true;
      }
    }

    private void StartPodcastDownload()
    {
      this.downloadManager.DownloadNextPodcast(this.PodcastDownloadCompleted);
    }

    private void PodcastDownloadCompleted(Task task)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        if (--this.podcastCount == 0)
        {
          // Turn off cancel-all button
          this.CancelAll.IsEnabled = false;
          return;
        }

        this.StartPodcastDownload();
      });
    }

    private void CancelDownload_Click(Object sender, RoutedEventArgs e)
    {
      var podcast = (PodcastMonitor)(sender as Button).DataContext;
      podcast.CancelDownload();
      
    }
    #endregion
  }
}
