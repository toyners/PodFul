using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Jabberwocky.Toolkit.Object;
using Jabberwocky.Toolkit.String;
using PodFul.Library;
using PodFul.WPF.Logging;
using PodFul.WPF.Processing;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for AddFeedFeedback.xaml
  /// </summary>
  public partial class AddFeedProgressWindow : Window
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private String feedURL;
    private String feedPath;
    private IImageResolver imageResolver;
    private ILogController logController;
    private Boolean windowLoaded;
    private Int32 imageDownloadTotal;
    #endregion

    #region Construction
    public AddFeedProgressWindow(String feedURL, String feedPath, IImageResolver imageResolver, ILogController logController)
    {
      feedURL.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'feedURL' is null or empty.");
      feedPath.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'feedPath' is null or empty.");
      imageResolver.VerifyThatObjectIsNotNull("Parameter 'imageResolver' is null.");
      logController.VerifyThatObjectIsNotNull("Parameter 'logController' is null.");

      this.feedURL = feedURL;
      this.feedPath = feedPath;
      this.imageResolver = imageResolver;
      this.logController = logController;

      InitializeComponent();
    }
    #endregion

    #region Properties
    public Feed Feed { get; private set; }
    #endregion

    #region Methods
    private void CancelButtonClick(Object sender, RoutedEventArgs e)
    {
      this.cancellationTokenSource.Cancel();
    }

    private void StartProcessing()
    {
      this.ProgressBar.IsIndeterminate = true;
      var cancelToken = cancellationTokenSource.Token;
      this.StatusMessage.Text = "Reading feed ...";

      Task addFeedTask = Task.Factory.StartNew(() =>
      {
        // Create the feed.
        var feedFilePath = Path.Combine(this.feedPath, "download.rss");
        this.Feed = FeedFunctions.CreateFeed(this.feedURL, feedFilePath, this.feedPath, cancelToken);

        this.imageResolver.TotalDownloadsRequiredEvent += TotalDownloadsRequiredEventHandler;
        this.imageResolver.StartDownloadNotificationEvent += StartDownloadNotificationEventHandler;
        this.imageResolver.ResolvePodcastImagesForFeed(this.Feed, cancelToken);
      }, cancelToken);

      addFeedTask.ContinueWith(task =>
      {
        if (task.IsFaulted)
        {
          this.logController.Message(MainWindow.ExceptionKey, "Trying to create new feed: " + task.Exception.Message);
          Application.Current.Dispatcher.Invoke(() =>
          {
            MessageBox.Show("Exception occurred when creating feed:\r\n\r\n" + task.Exception.Message, "Exception occurred.");
          });
        }
        else if (task.IsCanceled)
        {
          this.logController.Message(MainWindow.InfoKey, "Adding feed from '" + this.feedURL + "' was cancelled.");
        }
        else
        {
          this.logController.Message(MainWindow.InfoKey, "'" + this.Feed.Title + "' added. Podcasts stored in '" + this.feedPath + "'");
        }

        this.imageResolver.TotalDownloadsRequiredEvent -= TotalDownloadsRequiredEventHandler;
        this.imageResolver.StartDownloadNotificationEvent -= StartDownloadNotificationEventHandler;

        Application.Current.Dispatcher.Invoke(() =>
        {
          this.ProgressBar.IsIndeterminate = false; // Turn marque effect
          this.Close();
        });
      });
    }

    private void StartDownloadNotificationEventHandler(Int32 downloadNumber, String imageFilePath)
    {
      throw new NotImplementedException();
    }

    private void TotalDownloadsRequiredEventHandler(Int32 totalDownloads)
    {
      this.imageDownloadTotal = totalDownloads;
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.StatusMessage.Text = "Downloading " + this.imageDownloadTotal + " images";
      });
    }

    private void SetStatusMessage(String message)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.StatusMessage.Text = message;
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
    #endregion
  }
}
