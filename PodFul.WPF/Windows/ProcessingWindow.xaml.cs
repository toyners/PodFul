
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
    private FeedScanner feedScanner;
    private Boolean isLoaded;

    #region Construction   
    public static ProcessingWindow CreateWindow(FeedScanner feedScanner, GUILogger logger, IImageResolver imageResolver)
    {
      var processingWindow = new ProcessingWindow(feedScanner);

      logger.PostMessage = processingWindow.PostMessage;
      imageResolver.PostMessage = processingWindow.PostMessage;
      feedScanner.SetWindowTitleEvent = processingWindow.SetWindowTitleEventHandler;
      feedScanner.SetCancelButtonStateEvent = processingWindow.SetCancelButtonStateEventHandler;

      return processingWindow;
    }

    public ProcessingWindow(FeedScanner feedScanner)
    {
      this.feedScanner = feedScanner;

      this.InitializeComponent();
    }
    #endregion
    
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

    private void Window_Loaded(Object sender, RoutedEventArgs e)
    {
      if (!this.isLoaded)
      {
        // Ensure this is only called once.
        this.feedScanner.Process();
        this.isLoaded = true;
      }
    }

    private void FeedList_MouseWheel(Object sender, System.Windows.Input.MouseWheelEventArgs e)
    {

    }

    private void CancelDownload_Click(Object sender, RoutedEventArgs e)
    {

    }

    private void JobAddedEventHandler(DownloadJob job)
    {
      // Add the job to the list
    }
  }
}
