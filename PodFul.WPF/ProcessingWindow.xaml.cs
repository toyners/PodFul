﻿
namespace PodFul.WPF
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;

  /// <summary>
  /// Interaction logic for ProcessingWindow.xaml
  /// </summary>
  public partial class ProcessingWindow : Window
  {
    private TaskScheduler mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private IFeedProcessor feedProcessor;
    private Boolean isLoaded;
   
    public ProcessingWindow(IFeedProcessor feedProcessor)
    {
      this.feedProcessor = feedProcessor;

      InitializeComponent();
    }

    public void SetWindowTitleEventHandler(String title)
    {
      new Task(() =>
      {
        this.Title = title;

      }).Start(this.mainTaskScheduler);
    }

    public void PostMessage(String message)
    {
      new Task(() =>
      {
        this.Feedback.Text += message;
        this.FeedbackScroller.ScrollToBottom();
      }).Start(this.mainTaskScheduler);
    }

    public void InitialiseProgressEventHandler(String size, String unit, Boolean isIndeterminate)
    {
      new Task(() =>
      {
        this.Progress.Value = 0;
        this.ProgressSize.Text = size;
        this.ProgressUnit.Text = unit;
        this.Progress.IsIndeterminate = isIndeterminate;
      }).Start(this.mainTaskScheduler);
    }

    public void ResetProgressEventHandler()
    {
      new Task(() =>
      {
        this.Progress.Value = 0;
        this.ProgressSize.Text = String.Empty;
        this.ProgressUnit.Text = String.Empty;
        this.Progress.IsIndeterminate = false;
      }).Start(this.mainTaskScheduler);
    }

    public void SetProgressEventHandler(String size, Int32 value)
    {
      new Task(() =>
      { 
        if (this.Progress.IsIndeterminate)
        {         
          this.ProgressSize.Text = size;
          return;
        }
        
        this.Progress.Value = value;
        this.ProgressSize.Text = size;

      }).Start(this.mainTaskScheduler);
    }

    public void SetCancelButtonStateEventHandler(Boolean state)
    {
      new Task(() =>
      {
        this.Cancel.IsEnabled = state;
      }).Start(this.mainTaskScheduler);
    }

    private void Cancel_Click(Object sender, RoutedEventArgs e)
    {
      this.feedProcessor.Cancel();
    }

    private void Window_Loaded(Object sender, RoutedEventArgs e)
    {
      if (!this.isLoaded)
      {
        // Ensure this is only called once.
        this.feedProcessor.Process();
        this.isLoaded = true;
      }
    }
  }
}
