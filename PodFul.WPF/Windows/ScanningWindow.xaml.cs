
namespace PodFul.WPF.Windows
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Jabberwocky.Toolkit.Object;
  using Library;
  using Logging;
  using Processing;
  using UI_Support;

  /// <summary>
  /// Interaction logic for ScanningWindow.xaml
  /// </summary>
  public partial class ScanningWindow : Window
  {
    #region Enums
    private enum ProcessingStates
    {
      Stopped,
      Running,
      Cancelling
    }
    #endregion

    #region Fields
    private FeedScanner feedScanner;
    private Boolean isLoaded;
    private ProcessingStates processingState;
    private Boolean isClosing;
    private JobCountDisplayManager jobCountDisplayManager;
    #endregion

    #region Construction   
    public ScanningWindow(FeedScanner feedScanner, UILogger logger, IImageResolver imageResolver)
    {
      feedScanner.VerifyThatObjectIsNotNull("Parameter 'feedScanner' is null.");
      logger.VerifyThatObjectIsNotNull("Parameter 'logger' is null.");
      imageResolver.VerifyThatObjectIsNotNull("Parameter 'imageResolver' is null.");

      this.InitializeComponent();

      this.feedScanner = feedScanner;
      this.PodcastList.ItemsSource = feedScanner.Jobs;

      var jobCountStatusBarDisplay = new JobCountStatusBarDisplayComponent(this.WaitingCount, this.RunningCount, this.CompletedCount, this.FirstOptionalCount, this.SecondOptionalCount);
      this.jobCountDisplayManager = new JobCountDisplayManager(jobCountStatusBarDisplay);

      logger.PostMessage = this.PostMessage;
      imageResolver.PostMessage = this.PostMessage;
      feedScanner.SetWindowTitleEvent = this.SetWindowTitleEventHandler;
      feedScanner.ScanCompletedEvent += this.ScanCompletedEventHandler;
      feedScanner.UpdateCountsEvent += this.UpdateCountsEventHandler;
      feedScanner.JobStartedEvent += this.JobStartedEventHandler;
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
      this.processingState = ProcessingStates.Cancelling;
      this.CommandButton.Content = "Cancelling";
      this.CommandButton.IsEnabled = false;
      this.feedScanner.Cancel();
    }

    private void CommandButtonClick(Object sender, RoutedEventArgs e)
    {
      if (this.processingState == ProcessingStates.Running)
      {
        this.CancelScanning();
      }
      else if (this.processingState == ProcessingStates.Stopped)
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
      Application.Current.Dispatcher.Invoke(() =>
      {
        var index = this.feedScanner.Jobs.IndexOf(job);
        var itemContainer = (FrameworkElement)this.PodcastList.ItemContainerGenerator.ContainerFromIndex(index);
        itemContainer.BringIntoView();
      });
    }

    private void InitializeCounts()
    {
      this.jobCountDisplayManager.UpdateCounts(0, 0, 0, 0, 0);
    }

    private void ScanCompletedEventHandler()
    {
      this.processingState = ProcessingStates.Stopped;

      Application.Current.Dispatcher.Invoke(() =>
      {
        if (this.isClosing)
        {
          // Window was set to close while processing (or cancelling) was happening so now that the processing
          // has finished complete the window close.
          this.isClosing = false;
          this.Close();
          return;
        }

        this.CommandButton.IsEnabled = true;
        this.CommandButton.Content = "Close";
      });
    }

    private void UpdateCountsEventHandler(Int32 waitingJobsCount, Int32 runningJobsCount, Int32 completedJobsCount, Int32 cancelledJobsCount, Int32 failedJobsCount)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.jobCountDisplayManager.UpdateCounts(waitingJobsCount, runningJobsCount, completedJobsCount, cancelledJobsCount, failedJobsCount);
      });
    }

    private void WindowClosing(Object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (this.isClosing)
      {
        // Already closing so ignore any further close window commands.
        e.Cancel = true;
      }

      if (!this.isClosing)
      {
        if (this.processingState == ProcessingStates.Running)
        {
          // If the scanning is happening then cancel it before allowing the window to close.
          e.Cancel = true;
          this.isClosing = true;
          this.CancelScanning();
        }
        if (this.processingState == ProcessingStates.Cancelling)
        {
          // Already cancelled via button so don't close until cancelling is complete.
          e.Cancel = true;
          this.isClosing = true;
        }
      }
    }

    private void WindowLoaded(Object sender, RoutedEventArgs e)
    {
      if (!this.isLoaded)
      {
        // Ensure this is only called once.
        this.InitializeCounts();
        this.isLoaded = true;
        this.processingState = ProcessingStates.Running;
        this.feedScanner.Process();
      }
    }
    #endregion
  }
}
