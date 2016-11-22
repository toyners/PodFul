
namespace PodFul.WPF
{
  using System;
  using System.Windows;
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
    #endregion

    #region Construction   
    public static ScanningWindow CreateWindow(FeedScanner feedScanner, GUILogger logger, IImageResolver imageResolver)
    {
      var processingWindow = new ScanningWindow(feedScanner);

      logger.PostMessage = processingWindow.PostMessage;
      imageResolver.PostMessage = processingWindow.PostMessage;
      feedScanner.SetWindowTitleEvent = processingWindow.SetWindowTitleEventHandler;
      feedScanner.SetCancelButtonStateEvent = processingWindow.SetCancelButtonStateEventHandler;
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

    public void SetCancelButtonStateEventHandler(Boolean state)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.Cancel.IsEnabled = state;
        this.Cancel.Content = "Cancel";
      });
    }

    private void Cancel_Click(Object sender, RoutedEventArgs e)
    {
      this.Cancel.IsEnabled = false;
      this.Cancel.Content = "Cancelling";
      this.feedScanner.Cancel();
    }

    private void CancelDownload_Click(Object sender, RoutedEventArgs e)
    {
      // TODO: Complete cancellation.
    }

    private void FeedList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {
      // TODO: Scroll podcast download list.
    }

    private void JobStartedEventHandler(DownloadJob job)
    {
      this.PodcastList.ScrollIntoView(job);
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

    private void Window_Loaded(Object sender, RoutedEventArgs e)
    {
      if (!this.isLoaded)
      {
        // Ensure this is only called once.
        this.feedScanner.Process();
        this.isLoaded = true;
      }
    }
    #endregion
  }
}
