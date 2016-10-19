
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Windows;
  using Library;
  using Processing;
  using Windows;

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
      feedScanner.ConfirmPodcastsForDownloadEvent = ScanningWindow.ConfirmPodcastsForDownloadEventHandler;
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

    public static void ConfirmPodcastsForDownloadEventHandler(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes, Action<Boolean, List<Int32>> callback)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        var window = new DownloadConfirmation(oldFeed, newFeed, podcastIndexes);
        window.ShowDialog();
        var result = window.Result;
        if (result == MessageBoxResult.Cancel)
        {
          callback(false, null);
          return;
        }

        if (result == MessageBoxResult.No)
        {
          callback(true, null);
          return;
        }

        callback(true, window.PodcastIndexes);
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
      // TODO: Scroll podcast download list.
    }

    private void CancelDownload_Click(Object sender, RoutedEventArgs e)
    {
      // TODO: Complete cancellation.
    }
    #endregion
  }
}
