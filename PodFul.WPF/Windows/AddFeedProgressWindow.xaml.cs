using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Jabberwocky.Toolkit.Object;
using Jabberwocky.Toolkit.String;
using PodFul.Library;
using PodFul.WPF.Logging;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for AddFeedFeedback.xaml
  /// </summary>
  public partial class AddFeedProgressWindow : Window
  {
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private String feedURL;
    private String feedPath;

    private IImageResolver imageResolver;
    private ILogger fileLogger;

    private Boolean windowLoaded;

    public AddFeedProgressWindow(String feedURL, String feedPath, IImageResolver imageResolver, ILogger fileLogger)
    {
      feedURL.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'feedURL' is null or empty.");
      feedPath.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'feedPath' is null or empty.");
      imageResolver.VerifyThatObjectIsNotNull("Parameter 'imageResolver' is null.");
      fileLogger.VerifyThatObjectIsNotNull("Parameter 'fileLogger' is null.");

      this.feedURL = feedURL;
      this.feedPath = feedPath;
      this.imageResolver = imageResolver;
      this.fileLogger = fileLogger;

      InitializeComponent();
    }

    public Feed Feed { get; set; }

    private void CancelButtonClick(Object sender, RoutedEventArgs e)
    {
      this.cancellationTokenSource.Cancel();
    }

    private void StartProcessing()
    {
      this.ProgressBar.IsIndeterminate = true;
      var cancelToken = cancellationTokenSource.Token;

      Task addFeedTask = Task.Factory.StartNew(() =>
      {
        // Create the feed and add to storage.
        this.Feed = FeedFunctions.CreateFeed(this.feedURL, this.feedPath, this.imageResolver, cancelToken);
      }, cancelToken);

      addFeedTask.ContinueWith(task =>
      {
        if (task.IsFaulted)
        {
          this.fileLogger.Exception("Trying to create new feed: " + task.Exception.Message);
          Application.Current.Dispatcher.Invoke(() =>
          {
            MessageBox.Show("Exception occurred when creating feed:\r\n\r\n" + task.Exception.Message, "Exception occurred.");
          });
        }
        else if (task.IsCanceled)
        {
          this.fileLogger.Message("Adding feed from '" + this.feedURL + "' was cancelled.");
        }
        else
        {
          this.fileLogger.Message("'" + this.Feed.Title + "' added. Podcasts stored in '" + this.feedPath + "'");
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
          this.ProgressBar.IsIndeterminate = false; // Turn marque effect
          this.Close();
        });
      });
    }

    private void WindowClosing(Object sender, System.ComponentModel.CancelEventArgs e)
    {
      this.cancellationTokenSource.Cancel();
    }

    private void WindowLoaded(Object sender, RoutedEventArgs e)
    {
      if (!this.windowLoaded)
      {
        this.windowLoaded = true;
        this.StartProcessing();
      }
    }
  }
}
