
namespace PodFul.WPF
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using System.Windows.Media;
  using Miscellaneous;
  using Processing;
  using UI_Support;

  /// <summary>
  /// Interaction logic for PodcastDownloadWindow.xaml
  /// </summary>
  public partial class PodcastDownloadWindow : Window
  {
    #region Fields
    private DownloadManager downloadManager;
    private Boolean isLoaded;
    private Boolean isProcessing;
    private JobCountDisplayManager jobCountDisplayManager;
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

      var jobCountStatusBarDisplay = new JobCountStatusBarDisplayComponent(this.WaitingCount, this.RunningCount, this.CompletedCount, this.FirstOptionalCount, this.SecondOptionalCount);
      var jobCountWindowTitleDisplay = new JobCountWindowTitleDisplayComponent(this);
      this.jobCountDisplayManager = new JobCountDisplayManager(jobCountStatusBarDisplay, jobCountWindowTitleDisplay);

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
      this.jobCountDisplayManager.UpdateCounts(
        this.downloadManager.WaitingJobsCount,
        this.downloadManager.ProcessingJobsCount,
        this.downloadManager.CompletedJobsCount,
        this.downloadManager.CancelledJobsCount,
        this.downloadManager.FailedJobsCount);
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
