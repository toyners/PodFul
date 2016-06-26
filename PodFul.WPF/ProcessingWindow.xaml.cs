
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

    public void InitialiseProgressEventHandler(String text, Boolean isIndeterminate)
    {
      new Task(() =>
      {
        this.Progress.Value = 0;
        this.ProgressSize.Text = text;
        this.Progress.IsIndeterminate = isIndeterminate;
      }).Start(this.mainTaskScheduler);
    }

    public void ResetProgressEventHandler()
    {
      new Task(() =>
      {
        this.Progress.Value = 0;
        this.ProgressSize.Text = String.Empty;
        this.Progress.IsIndeterminate = false;
      }).Start(this.mainTaskScheduler);
    }

    public void SetProgressEventHandler(String text, Int32 value)
    {
      new Task(() =>
      { 
        if (this.Progress.IsIndeterminate)
        {         
          this.ProgressSize.Text = text;
          return;
        }
        
        this.Progress.Value = value;
        this.ProgressSize.Text = text;

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

    private void Window_Initialized(Object sender, EventArgs e)
    {
      this.feedProcessor.Process();
    }
  }
}
