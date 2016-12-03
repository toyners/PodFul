
namespace PodFul.WPF
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Library;
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

    private void FeedList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {
      // TODO: Scroll podcast download list.
    }

    private void JobStartedEventHandler(DownloadJob job)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.PodcastList.ScrollIntoView(job);
      });
    }

    private void InitializeCounts()
    {
      this.WaitingCount.Text = "Waiting: 0";
      this.ProcessingCount.Text = "Processing: 0";
      this.CompletedCount.Text = "Completed: 0";
      this.CancelledCount.Text = String.Empty;
      this.FailedCount.Text = String.Empty;
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

    private void UpdateCountsEventHandler(Int32 waitingJobsCount, Int32 processingJobsCount, Int32 completedJobsCount, Int32 cancelledJobsCount, Int32 failedJobsCount)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.WaitingCount.Text = "Waiting: " + waitingJobsCount;
        this.ProcessingCount.Text = "Processing: " + processingJobsCount;
        this.CompletedCount.Text = "Completed: " + completedJobsCount;
        this.CancelledCount.Text = (cancelledJobsCount == 0 ? String.Empty : "Cancelled: " + cancelledJobsCount);
        this.FailedCount.Text = (failedJobsCount == 0 ? String.Empty : "Failed: " + failedJobsCount);
      });
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
