
namespace PodFul.WPF.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Input;
  using Processing;
  using UI_Support;

  /// <summary>
  /// Interaction logic for PodcastDownloadWindow.xaml
  /// </summary>
  public partial class PodcastDownloadWindow2 : Window
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
    private DownloadManager downloadManager;
    private Boolean isLoaded;
    private ProcessingStates processingState;
    private Boolean isClosing;
    private JobCountDisplayManager jobCountDisplayManager;

    private ObservableCollection<DownloadJob> jobs;
    #endregion

    #region Construction
    public PodcastDownloadWindow2(DownloadManager downloadManager, Boolean hideCompletedDownloadJobs)
    {
      InitializeComponent();

      this.downloadManager = downloadManager;
      this.downloadManager.AllJobsFinishedEvent += this.WorkCompleted;
      this.downloadManager.JobQueuedEvent += this.UpdateCounts;
      this.downloadManager.JobFinishedEvent += this.JobFinishedEventHandler;
      this.downloadManager.JobStartedEvent += this.UpdateCounts;

      this.AllJobs.ItemsSource = downloadManager.Jobs;
      this.jobs = new ObservableCollection<DownloadJob>();
      this.NonCompletedJobs.ItemsSource = jobs;

      var jobCountStatusBarDisplay = new JobCountStatusBarDisplayComponent(this.WaitingCount, this.RunningCount, this.CompletedCount, this.FirstOptionalCount, this.SecondOptionalCount);
      var jobCountWindowTitleDisplay = new JobCountWindowTitleDisplayComponent(this);
      this.jobCountDisplayManager = new JobCountDisplayManager(jobCountStatusBarDisplay, jobCountWindowTitleDisplay);

      this.jobCountDisplayManager.DisplayCounts(); // Display initial counts  

      this.HideCompletedDownloadJobsCheckbox.IsChecked = hideCompletedDownloadJobs;
    }
    #endregion

    #region Methods
    private void CancelAllDownloadJobs()
    {
      this.processingState = ProcessingStates.Cancelling;
      this.CommandButton.Content = "Cancelling";
      this.CommandButton.IsEnabled = false;
      this.downloadManager.CancelAllDownloads();
    }

    private void CancelDownloadJobClick(Object sender, RoutedEventArgs e)
    {
      this.downloadManager.CancelDownload((sender as Button).DataContext);
    }

    private void CommandButtonClick(Object sender, RoutedEventArgs e)
    {
      if (this.processingState == ProcessingStates.Running)
      {
        this.CancelAllDownloadJobs();
      }
      else if (this.processingState == ProcessingStates.Stopped)
      {
        this.Close();
      }
    }

    private void FeedList_MouseWheel(Object sender, MouseWheelEventArgs e)
    {
    }

    private void HideCompletedDownloadJobsClicked(Object sender, RoutedEventArgs e)
    {
      var isChecked = ((CheckBox)sender).IsChecked;
      this.HideCompletedJobs(isChecked.HasValue && isChecked.Value);
    }

    private void HideCompletedJobs(Boolean hideCompletedJobs)
    {
      if (hideCompletedJobs)
      {
        this.AllJobs.Visibility = Visibility.Hidden;
        this.NonCompletedJobs.Visibility = Visibility.Visible;
      }
      else
      {
        this.NonCompletedJobs.Visibility = Visibility.Hidden;
        this.AllJobs.Visibility = Visibility.Visible;
      }
    }

    private void JobFinishedEventHandler(DownloadJob job)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.jobs.Remove(job);
      });

      this.UpdateCounts(job);
    }

    private void JobQueueEventHandler(DownloadJob job)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.jobs.Add(job);
      });

      this.UpdateCounts(job);
    }

    private void UpdateCounts(DownloadJob job)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.jobCountDisplayManager.UpdateCounts(job);
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
          // If the processing is happening then cancel it before allowing the window to close.
          e.Cancel = true;
          this.isClosing = true;
          this.CancelAllDownloadJobs();
        }
        else if (this.processingState == ProcessingStates.Cancelling)
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
        this.processingState = ProcessingStates.Running;
        this.CommandButton.Content = "Cancel All";
        this.downloadManager.StartDownloads();
        
        // Ensure this functionality is only called once.
        this.isLoaded = true;
      }
    }

    private void WorkCompleted()
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
    #endregion
  }
}
