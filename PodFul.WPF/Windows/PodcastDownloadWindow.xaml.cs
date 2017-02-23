
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
      var cancelledJobCount = this.downloadManager.CancelledJobsCount;
      var failedJobCount = this.downloadManager.FailedJobsCount;

      if (cancelledJobCount > 0)
      {
        if (this.cancelledJobCounter == null)
        {
          Brush cancelledJobCounterBrush = new SolidColorBrush();
          this.cancelledJobCounter = new DownloadJobCounter("Cancelled: ", cancelledJobCounterBrush);

          if (this.failedJobCounter != null)
          {
            this.failedJobCounter.TextControl = this.SecondOptionalCount;
          }

          this.cancelledJobCounter.TextControl = this.FirstOptionalCount;
        }

        this.cancelledJobCounter.Count = cancelledJobCount;
      }

      if (failedJobCount > 0)
      {
        if (this.failedJobCounter == null)
        {
          Brush failedJobCounterBrush = new SolidColorBrush();
          this.failedJobCounter = new DownloadJobCounter("Failed: ", failedJobCounterBrush);
          this.failedJobCounter.TextControl = (this.cancelledJobCounter != null ? this.SecondOptionalCount : this.FirstOptionalCount);
        }

        this.failedJobCounter.Count = failedJobCount;
      }

      var title = String.Format("Downloading Podcasts: {0} waiting, {1} running, {2} completed", waitingJobCount, runningJobCount, completedJobCount);
      if (this.cancelledJobCounter.Count > 0)
      {
        title += ", " + this.cancelledJobCounter.Count + " cancelled";
      }

      if (this.failedJobCounter.Count > 0)
      {
        title += ", " + this.failedJobCounter.Count + " failed";
      }

      Application.Current.Dispatcher.Invoke(() => 
      {
        this.Title = title;
        this.WaitingCount.Text = "Waiting: " + waitingJobCount;
        this.RunningCount.Text = "Running: " + runningJobCount;
        this.CompletedCount.Text = "Completed: " + completedJobCount;
        this.cancelledJobCounter.UpdateTextCount();
        this.failedJobCounter.UpdateTextCount();
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
    private TextBlock textBlock;

    public DownloadJobCounter(String text, Brush brush)
    {
      this.text = text;
      this.brush = brush;
    }

    public TextBlock TextControl
    {
      set
      {
        this.textBlock = value;
        this.textBlock.Foreground = this.brush;
      }
    }

    public Int32 Count { get; set; }

    public void UpdateTextCount()
    {
      this.textBlock.Text = this.text + this.Count;
    }
  }
}
