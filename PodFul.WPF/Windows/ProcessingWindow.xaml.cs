
namespace PodFul.WPF
{
  using System;
  using System.Threading.Tasks;
  using System.Windows;
  using Library;
  using Processing;

  /// <summary>
  /// Interaction logic for ProcessingWindow.xaml
  /// </summary>
  public partial class ProcessingWindow : Window
  {
    private TaskScheduler mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private FeedProcessor feedProcessor;
    private Boolean isLoaded;

    #region Construction   
    public static ProcessingWindow CreateWindow(FeedProcessor feedProcessor, GUILogger logger, IImageResolver imageResolver)
    {
      var processingWindow = new ProcessingWindow(feedProcessor);

      logger.PostMessage = processingWindow.PostMessage;
      imageResolver.PostMessage = processingWindow.PostMessage;
      feedProcessor.SetWindowTitleEvent = processingWindow.SetWindowTitleEventHandler;
      feedProcessor.InitialiseProgressEvent = processingWindow.InitialiseProgressEventHandler;
      feedProcessor.SetCancelButtonStateEvent = processingWindow.SetCancelButtonStateEventHandler;
      feedProcessor.SetProgressEvent = processingWindow.SetProgressEventHandler;
      feedProcessor.ResetProgressEvent = processingWindow.ResetProgressEventHandler;

      return processingWindow;
    }

    public ProcessingWindow(FeedProcessor feedProcessor)
    {
      this.feedProcessor = feedProcessor;

      this.InitializeComponent();
    }
    #endregion
    
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

    public void InitialiseProgressEventHandler(String majorSize, String minorSize, String unit, Boolean isIndeterminate)
    {
      new Task(() =>
      {
        /*this.Progress.Value = 0;
        this.ProgressMajorSize.Text = majorSize;
        this.ProgressMinorSize.Text = minorSize;
        this.ProgressUnit.Text = unit;
        this.Progress.IsIndeterminate = isIndeterminate;*/
      }).Start(this.mainTaskScheduler);
    }

    public void ResetProgressEventHandler()
    {
      new Task(() =>
      {
        /*this.Progress.Value = 0;
        this.ProgressMajorSize.Text = String.Empty;
        this.ProgressMinorSize.Text = String.Empty;
        this.ProgressUnit.Text = String.Empty;
        this.Progress.IsIndeterminate = false;*/
      }).Start(this.mainTaskScheduler);
    }

    public void SetProgressEventHandler(String majorSize, String minorSize, Int32 value)
    {
      new Task(() =>
      {
        /*this.ProgressMajorSize.Text = majorSize;
        this.ProgressMinorSize.Text = minorSize;
        if (this.Progress.IsIndeterminate)
        {         
          return;
        }
        
        this.Progress.Value = value;*/

      }).Start(this.mainTaskScheduler);
    }

    public void SetCancelButtonStateEventHandler(Boolean state)
    {
      new Task(() =>
      {
        this.Cancel.IsEnabled = state;
        this.Cancel.Content = "Cancel";
      }).Start(this.mainTaskScheduler);
    }

    private void Cancel_Click(Object sender, RoutedEventArgs e)
    {
      this.Cancel.IsEnabled = false;
      this.Cancel.Content = "Cancelling";
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

    private void FeedList_MouseWheel(Object sender, System.Windows.Input.MouseWheelEventArgs e)
    {

    }

    private void CancelDownload_Click(Object sender, RoutedEventArgs e)
    {

    }

    private void JobAddedEventHandler(PodcastMonitor job)
    {
      // Add the job to the list
    }
  }
}
