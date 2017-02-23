
namespace PodFul.WPF
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Library;
  using Miscellaneous;
  using Processing;

  /// <summary>
  /// Interaction logic for ScanningWindow.xaml
  /// </summary>
  public partial class ScanningWindow : Window
  {
    #region Fields
    private FeedScanner feedScanner;
    private Boolean isLoaded;
    private Boolean isProcessing;
    private Boolean closeAfterScan;
    private JobCountDisplayManager jobCountDisplayManager;
    #endregion

    #region Construction   
    public static ScanningWindow CreateWindow(FeedScanner feedScanner, GUILogger logger, IImageResolver imageResolver)
    {
      var processingWindow = new ScanningWindow(feedScanner);

      logger.PostMessage = processingWindow.PostMessage;
      imageResolver.PostMessage = processingWindow.PostMessage;
      feedScanner.SetWindowTitleEvent = processingWindow.SetWindowTitleEventHandler;
      feedScanner.ScanCompletedEvent += processingWindow.ScanCompletedEventHandler; 
      feedScanner.UpdateCountsEvent += processingWindow.UpdateCountsEventHandler;
      feedScanner.JobStartedEvent += processingWindow.JobStartedEventHandler;
      return processingWindow;
    }

    public ScanningWindow(FeedScanner feedScanner)
    {
      this.InitializeComponent();

      this.feedScanner = feedScanner;
      this.PodcastList.ItemsSource = feedScanner.Jobs;

      var jobCountStatusBarDisplay = new JobCountStatusBarDisplay(this.WaitingCount, this.RunningCount, this.CompletedCount, this.FirstOptionalCount, this.SecondOptionalCount);
      this.jobCountDisplayManager = new JobCountDisplayManager(jobCountStatusBarDisplay);
    }
    #endregion

    #region Methods
    public void SetWindowTitleEventHandler(String title)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.Title = title;
      });
    }

    public void PostMessage(String message)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.Feedback.Text += message;
        this.FeedbackScroller.ScrollToBottom();
      });
    }

    private void CancelScanning()
    {
      this.CommandButton.IsEnabled = false;
      this.CommandButton.Content = "Cancelling";
      this.feedScanner.Cancel();
    }

    private void CommandButtonClick(Object sender, RoutedEventArgs e)
    {
      if (this.isProcessing)
      {
        this.CancelScanning();
      }
      else
      {
        this.Close();
      }
    }

    private void CancelDownload_Click(Object sender, RoutedEventArgs e)
    {
      this.feedScanner.CancelDownload((sender as Button).DataContext);
    }

    private void PodcastListMouseWheel(Object sender, MouseWheelEventArgs e)
    {
      // TODO: Scroll podcast download list.
    }

    private void JobStartedEventHandler(DownloadJob job)
    {
      /*Application.Current.Dispatcher.Invoke(() =>
      {
        this.PodcastList.ScrollIntoView(job);
      });*/
    }

    private void InitializeCounts()
    {
      this.jobCountDisplayManager.UpdateCounts(0, 0, 0, 0, 0);
    }

    private void ScanCompletedEventHandler()
    {
      this.isProcessing = false;

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.CommandButton.IsEnabled = true;
        this.CommandButton.Content = "Close";

        if (this.closeAfterScan)
        {
          this.Close();
        }
      });
    }

    private void UpdateCountsEventHandler(Int32 waitingJobsCount, Int32 runningJobsCount, Int32 completedJobsCount, Int32 cancelledJobsCount, Int32 failedJobsCount)
    {
      this.jobCountDisplayManager.UpdateCounts(waitingJobsCount, runningJobsCount, completedJobsCount, cancelledJobsCount, failedJobsCount);
    }

    private void WindowClosing(Object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (this.isProcessing && !this.closeAfterScan)
      {
        e.Cancel = true;
        this.closeAfterScan = true;
        this.CancelScanning();
      }
    }

    private void WindowLoaded(Object sender, RoutedEventArgs e)
    {
      if (!this.isLoaded)
      {
        // Ensure this is only called once.
        this.InitializeCounts();
        this.isLoaded = true;
        this.isProcessing = true;
        this.feedScanner.Process();
      }
    }
    #endregion
  }
}
