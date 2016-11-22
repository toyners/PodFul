
namespace PodFul.WPF
{
  using System;
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
    private DownloadManager downloadManager;
    private Boolean isLoaded;
    private Boolean isProcessing;
    #endregion

    #region Construction
    public PodcastDownloadWindow(DownloadManager downloadManager)
    {
      InitializeComponent();

      this.downloadManager = downloadManager;
      this.downloadManager.AllDownloadsCompletedEvent += this.WorkCompleted;
      this.downloadManager.JobStartedEvent += this.UpdateCounts;
      this.downloadManager.JobFinishedEvent += this.UpdateCounts;

      this.PodcastList.ItemsSource = downloadManager.Jobs;

      this.UpdateCounts();
    }
    #endregion

    #region Methods
    private void CancelDownload_Click(Object sender, RoutedEventArgs e)
    {
      this.downloadManager.CancelDownload((sender as Button).DataContext);
    }

    private void CommandButton_Click(Object sender, RoutedEventArgs e)
    {
      if (this.isProcessing)
      {
        this.CommandButton.Content = "Cancelling";
        this.CommandButton.IsEnabled = false;
        this.downloadManager.CancelAllDownloads();
      }
      else
      {
        this.Close();
      }
    }

    private void FeedList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {
    }

    private void UpdateCounts(DownloadJob job)
    {
      this.UpdateCounts();
    }

    private void UpdateCounts()
    {
      var waitingJobCount = this.downloadManager.WaitingJobsCount;
      var processingJobCount = this.downloadManager.ProcessingJobsCount;
      var completedJobCount = this.downloadManager.CompletedJobsCount;
      var cancelledJobCount = this.downloadManager.CancelledJobsCount;
      var failedJobCount = this.downloadManager.FailedJobsCount;

      var title = String.Format("Downloading Podcasts: {0} waiting, {1} processing, {2} completed", waitingJobCount, processingJobCount, completedJobCount);

      if (cancelledJobCount > 0)
      {
        title += ", " + cancelledJobCount + " cancelled";
      }

      if (failedJobCount > 0)
      {
        title += ", " + failedJobCount + " failed";
      }

      var waitingJobText = "Waiting: " + waitingJobCount;
      var processingCountText = "Processing: " + processingJobCount;
      var completedCountText = "Completed: " + completedJobCount;
      var cancelledCountText = (cancelledJobCount == 0 ? String.Empty : "Cancelled: " + cancelledJobCount);
      var failedCountText = (failedJobCount == 0 ? String.Empty : "Failed: " + failedJobCount);

      Application.Current.Dispatcher.Invoke(() => 
      {
        this.Title = title;
        this.WaitingCount.Text = waitingJobText;
        this.ProcessingCount.Text = processingCountText;
        this.CompletedCount.Text = completedCountText;
        this.CancelledCount.Text = cancelledCountText;
        this.FailedCount.Text = failedCountText;
      });
    }

    private void Window_Loaded(Object sender, RoutedEventArgs e)
    {
      if (!this.isLoaded)
      {
        this.isProcessing = true;
        this.CommandButton.Content = "Cancel";
        this.downloadManager.StartDownloads();
        
        // Ensure this functionality is only called once.
        this.isLoaded = true;
      }
    }

    private void WorkCompleted()
    {
      this.isProcessing = false;
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.CommandButton.Content = "Close";
        this.CommandButton.IsEnabled = true;
      });
    }
    #endregion
  }
}
