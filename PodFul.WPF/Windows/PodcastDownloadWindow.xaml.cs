
namespace PodFul.WPF
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
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
    private Boolean isClosing;
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
    private void CancelDownloadJobClick(Object sender, RoutedEventArgs e)
    {
      this.downloadManager.CancelDownload((sender as Button).DataContext);
    }

    private void CommandButtonClick(Object sender, RoutedEventArgs e)
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
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.jobCountDisplayManager.UpdateCounts(
          this.downloadManager.WaitingJobsCount,
          this.downloadManager.ProcessingJobsCount,
          this.downloadManager.CompletedJobsCount,
          this.downloadManager.CancelledJobsCount,
          this.downloadManager.FailedJobsCount);
      });
    }

    private void WindowClosing(Object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (this.isClosing)
      {
        // Already closing so ignore any further close window commands.
        e.Cancel = true;
      }

      if (this.isProcessing && !this.isClosing)
      {
        // If the processing is happening then cancel it before allowing the window to close.
        e.Cancel = true;
        this.downloadManager.CancelAllDownloads();
        this.isClosing = true;
      }
    }

    private void WindowLoaded(Object sender, RoutedEventArgs e)
    {
      if (!this.isLoaded)
      {
        this.isProcessing = true;
        this.CommandButton.Content = "Cancel All";
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
        this.CommandButton.IsEnabled = true;
        this.CommandButton.Content = "Close";

        if (this.isClosing)
        {
          // Window was set to close while processing was happening so now that the processing
          // has finished complete the window close.
          this.Close();
        }
      });
    }
    #endregion
  }
}
