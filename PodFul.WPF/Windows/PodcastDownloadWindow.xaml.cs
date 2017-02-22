
namespace PodFul.WPF
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;
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
    private DownloadJobCounter cancelledJobCounter;
    private DownloadJobCounter failedJobCounter;
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
      var runningJobCount = this.downloadManager.ProcessingJobsCount;
      var completedJobCount = this.downloadManager.CompletedJobsCount;
      this.cancelledJobCounter.Count = this.downloadManager.CancelledJobsCount;
      this.failedJobCounter.Count = this.downloadManager.FailedJobsCount;
      //var cancelledJobCount = this.downloadManager.CancelledJobsCount;
      //var failedJobCount = this.downloadManager.FailedJobsCount;

      if (this.downloadManager.CancelledJobsCount > 0 && this.cancelledJobCounter == null)
      {
        Brush cancelledJobCounterBrush = new SolidColorBrush();
        this.cancelledJobCounter = new DownloadJobCounter("", cancelledJobCounterBrush);

      }

      if (this.downloadManager.FailedJobsCount > 0 && this.failedJobCounter == null)
      {
        Brush failedJobCounterBrush = new SolidColorBrush();
        this.cancelledJobCounter = new DownloadJobCounter("", failedJobCounterBrush);
      }

      var title = String.Format("Downloading Podcasts: {0} waiting, {1} running, {2} completed", waitingJobCount, runningJobCount, completedJobCount);

      /*if (cancelledJobCount > 0)
      {
        title += ", " + cancelledJobCount + " cancelled";
      }

      if (failedJobCount > 0)
      {
        title += ", " + failedJobCount + " failed";
      }*/

      if (this.cancelledJobCounter.Count > 0)
      {
        title += ", " + this.cancelledJobCounter.Count + " cancelled";
      }

      if (this.failedJobCounter.Count > 0)
      {
        title += ", " + this.failedJobCounter.Count + " failed";
      }

      var waitingJobText = "Waiting: " + waitingJobCount;
      var processingCountText = "Running: " + runningJobCount;
      var completedCountText = "Completed: " + completedJobCount;
      //var cancelledCountText = (cancelledJobCount == 0 ? String.Empty : "Cancelled: " + cancelledJobCount);
      //var failedCountText = (failedJobCount == 0 ? String.Empty : "Failed: " + failedJobCount);

      Application.Current.Dispatcher.Invoke(() => 
      {
        this.Title = title;
        this.WaitingCount.Text = waitingJobText;
        this.RunningCount.Text = processingCountText;
        this.CompletedCount.Text = completedCountText;
        this.cancelledJobCounter.UpdateTextCount();
        this.failedJobCounter.UpdateTextCount();
        //this.CancelledCount.Text = cancelledCountText;
        //this.FailedCount.Text = failedCountText;

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

  internal class DownloadJobCounter
  {
    private String text;
    private Brush brush;

    public DownloadJobCounter(String text, Brush brush)
    {
      this.text = text;
      this.brush = brush;
    }

    public TextBox TextControl { get; set; }

    public Int32 Count { get; set; }

    public void UpdateTextControlForegroundColor()
    {
      this.TextControl.Foreground = this.brush;
    }

    public void UpdateTextCount()
    {
      this.TextControl.Text = this.text + this.Count;
    }
  }
}
