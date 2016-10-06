
namespace PodFul.WPF
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Jabberwocky.Toolkit.String;
  using PodFul.WPF.Processing;

  /// <summary>
  /// Interaction logic for PodcastDownloadWindow.xaml
  /// </summary>
  public partial class PodcastDownloadWindow : Window
  {
    #region Fields
    private Boolean isLoaded;
    private DownloadManager downloadManager;
    private Int32 threadCount = 0;
    #endregion

    #region Construction
    public PodcastDownloadWindow(DownloadManager downloadManager)
    {
      InitializeComponent();

      this.downloadManager = downloadManager;
      this.downloadManager.AllDownloadsCompleted = DownloadCompleted;

      this.PodcastList.ItemsSource = downloadManager.Jobs;

      this.Title = "Downloading " + downloadManager.Jobs.Count + " Podcast".Pluralize((UInt32)downloadManager.Jobs.Count);
    }
    #endregion

    #region Methods
    private void CancelAllDownloads_Click(Object sender, RoutedEventArgs e)
    {
      this.CancelAll.IsEnabled = false;
      this.downloadManager.CancelAllDownloads();      
    }

    private void CancelDownload_Click(Object sender, RoutedEventArgs e)
    {
      this.downloadManager.CancelDownload((sender as Button).DataContext);
    }

    private void FeedList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {
    }

    private void Window_Loaded(Object sender, RoutedEventArgs e)
    {
      if (!this.isLoaded)
      {
        this.CancelAll.IsEnabled = true;
        this.downloadManager.StartDownloads();
        
        // Ensure this functionality is only called once.
        this.isLoaded = true;
      }
    }

    private void DownloadCompleted()
    {
      Application.Current.Dispatcher.Invoke(() => { this.CancelAll.IsEnabled = false; });
    }
    #endregion
  }
}
