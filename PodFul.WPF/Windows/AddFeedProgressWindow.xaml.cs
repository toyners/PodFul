using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Jabberwocky.Toolkit.Object;
using PodFul.Library;
using PodFul.WPF.Logging;
using PodFul.WPF.Processing;
using PodFul.WPF.ViewModel;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for AddFeedFeedback.xaml
  /// </summary>
  public partial class AddFeedProgressWindow : Window
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private IImageResolver imageResolver;
    private ILogController logController;
    private Boolean windowLoaded;
    private Int32 imageDownloadTotal;
    private Int32 imageDownloadCount;
    private IFeedFactory feedFactory;

    private IFeedCollectionViewModel feedCollectionViewModel;
    #endregion

    #region Construction
    public AddFeedProgressWindow(IFeedFactory feedFactory, ILogController logController, IImageResolver imageResolver)
    {
      feedFactory.VerifyThatObjectIsNotNull("Parameter 'feedFactory' is null.");
      logController.VerifyThatObjectIsNotNull("Parameter 'logController' is null.");

      this.feedFactory = feedFactory;
      this.imageResolver = imageResolver;
      this.logController = logController;

      InitializeComponent();
    }

    private AddFeedToken addFeedToken;
    public AddFeedProgressWindow(IFeedCollectionViewModel feedCollectionViewModel, AddFeedToken addFeedToken)
    {
      InitializeComponent();

      this.feedCollectionViewModel = feedCollectionViewModel;
      this.feedCollectionViewModel.CompletedImageDownloadNotificationEvent = this.CompletedDownloadNotificationEventHandler;
      this.feedCollectionViewModel.SkippedImageDownloadNotificationEvent = this.SkipDownloadNotificationEventHandler;
      this.feedCollectionViewModel.StartImageDownloadNotificationEvent = this.StartDownloadNotificationEventHandler;
      this.feedCollectionViewModel.TotalImageDownloadsRequiredEvent = this.TotalDownloadsRequiredEventHandler;

      this.addFeedToken = addFeedToken;
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

    private void AddFeedForListView()
    {
      this.ProgressBar.IsIndeterminate = true;
      var cancelToken = cancellationTokenSource.Token;
      this.StatusMessage.Text = "Reading feed information ...";

      Task addFeedTask = Task.Factory.StartNew(() =>
      {
        // Create the feed.
        this.Feed = this.feedFactory.Create(cancelToken);
        
        if (this.imageResolver != null)
        {
          this.imageResolver.TotalDownloadsRequiredEvent = this.TotalDownloadsRequiredEventHandler;
          this.imageResolver.StartDownloadNotificationEvent += this.StartDownloadNotificationEventHandler;
          this.imageResolver.SkippedDownloadNotificationEvent += this.SkipDownloadNotificationEventHandler;
          this.imageResolver.CompletedDownloadNotificationEvent += this.CompletedDownloadNotificationEventHandler;
          this.imageResolver.ResolvePodcastImagesForFeed(this.Feed, cancelToken);
        }
      }, cancelToken);

      addFeedTask.ContinueWith(task =>
      {
        if (task.IsFaulted)
        {
          var flattenedMessage = String.Empty;
          var flattenedException = task.Exception.Flatten();
          foreach(var exception in flattenedException.InnerExceptions)
          {
            flattenedMessage += exception.Message + " ";
          }

          this.logController.Message(LoggerKeys.ExceptionKey, "Trying to create new feed: " + flattenedMessage);
          Application.Current.Dispatcher.Invoke(() =>
          {
            MessageBox.Show("Exception occurred when creating feed:\r\n\r\n" + flattenedMessage, "Exception occurred.");
          });
        }
        else if (task.IsCanceled)
        {
          this.logController.Message(LoggerKeys.InfoKey, "Adding feed from '" + this.feedFactory.FeedURL + "' was cancelled.");
        }
        else
        {
          this.logController.Message(LoggerKeys.InfoKey, "'" + this.Feed.Title + "' added. Podcasts stored in '" + this.Feed.Directory + "'");
        }

        if (this.imageResolver != null)
        {
          this.imageResolver.TotalDownloadsRequiredEvent -= this.TotalDownloadsRequiredEventHandler;
          this.imageResolver.StartDownloadNotificationEvent -= this.StartDownloadNotificationEventHandler;
          this.imageResolver.SkippedDownloadNotificationEvent -= this.SkipDownloadNotificationEventHandler;
          this.imageResolver.CompletedDownloadNotificationEvent -= this.CompletedDownloadNotificationEventHandler;
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
          this.ProgressBar.IsIndeterminate = false; // Turn off marque effect
          this.Close();
        });
      });
    }

    private void AddFeedForTreeView()
    {
      this.ProgressBar.IsIndeterminate = true;
      var cancelToken = cancellationTokenSource.Token;
      this.StatusMessage.Text = "Reading feed ...";

      Task addFeedTask = Task.Factory.StartNew(() =>
      {
        // Create the feed.
        //Application.Current.Dispatcher.Invoke(() =>
        ///{
          this.feedCollectionViewModel.AddFeed(this.addFeedToken, cancelToken);
        //});
      }, cancelToken);

      addFeedTask.ContinueWith(task =>
      {
        if (task.IsFaulted)
        {
          var flattenedMessage = String.Empty;
          var flattenedException = task.Exception.Flatten();
          foreach (var exception in flattenedException.InnerExceptions)
          {
            flattenedMessage += exception.Message + " ";
          }

          Application.Current.Dispatcher.Invoke(() =>
          {
            MessageBox.Show("Exception occurred when creating feed:\r\n\r\n" + flattenedMessage, "Exception occurred.");
          });
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
          this.ProgressBar.IsIndeterminate = false; // Turn off marque effect
          this.Close();
        });
      });
    }

    private void CompletedDownloadNotificationEventHandler(Int32 downloadNumber, String imageFilePath)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.ProgressBar.Value = this.imageDownloadCount;
      });
    }

    private void SkipDownloadNotificationEventHandler(Int32 downloadNumber, String imageFilePath)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.imageDownloadCount++;
        this.StatusMessage.Text = "Skipped " + this.imageDownloadCount + " of " + this.imageDownloadTotal + " Images ...";
        this.ProgressBar.Value = this.imageDownloadCount;
      });
    }

    private void StartDownloadNotificationEventHandler(Int32 downloadNumber, String imageFilePath)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.imageDownloadCount++;
        this.StatusMessage.Text = "Downloading " + this.imageDownloadCount + " of " + this.imageDownloadTotal + " Images ...";
      });
    }

    private void TotalDownloadsRequiredEventHandler(Int32 totalDownloads)
    {
      this.imageDownloadTotal = totalDownloads;
      this.imageDownloadCount = 0;
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.CancelButton.Content = "Skip";
        this.ProgressBar.IsIndeterminate = false;
        this.ProgressBar.Maximum = totalDownloads - 1;
        this.ProgressBar.Value = 0; 
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
        if (this.feedCollectionViewModel != null)
        {
          this.AddFeedForTreeView();
        }
        else
        {
          this.AddFeedForListView();
        }
      }
    }
    #endregion
  }
}
