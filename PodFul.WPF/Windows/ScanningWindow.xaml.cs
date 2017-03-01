﻿
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
    #region Fields
    private FeedScanner feedScanner;
    private Boolean isLoaded;
    private Boolean isProcessing;
    private Boolean isClosing;
    private JobCountDisplayManager jobCountDisplayManager;
    #endregion

    #region Construction   
    public static ScanningWindow CreateWindow(FeedScanner feedScanner, UILogger logger, IImageResolver imageResolver)
    {
      feedScanner.VerifyThatObjectIsNotNull("Parameter 'feedScanner' is null.");
      logger.VerifyThatObjectIsNotNull("Parameter 'logger' is null.");
      imageResolver.VerifyThatObjectIsNotNull("Parameter 'imageResolver' is null.");

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

      var jobCountStatusBarDisplay = new JobCountStatusBarDisplayComponent(this.WaitingCount, this.RunningCount, this.CompletedCount, this.FirstOptionalCount, this.SecondOptionalCount);
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

        if (this.isClosing)
        {
          this.Close();
        }
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

      if (this.isProcessing && !this.isClosing)
      {
        // If the scanning is happening then cancel it before allowing the window to close.
        e.Cancel = true;
        this.isClosing = true;
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
