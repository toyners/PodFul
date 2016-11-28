using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Jabberwocky.Toolkit.Object;
using Jabberwocky.Toolkit.String;
using PodFul.Library;

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

    private void CancelButtonClick(Object sender, RoutedEventArgs e)
    {
      this.cancellationTokenSource.Cancel();
    }

    private void StartProcessing()
    {
      this.ProgressBar.IsIndeterminate = true;
      var cancelToken = cancellationTokenSource.Token;

      Feed feed = null;
      Task addFeedTask = Task.Factory.StartNew(() =>
      {
        // Create the feed and add to storage.
        try
        {
          feed = FeedFunctions.CreateFeed(this.feedURL, this.feedPath, this.imageResolver, CancellationToken.None);
          this.fileLogger.Message("'" + feed.Title + "' added. Podcasts stored in '" + feed.Directory + "'");
        }
        catch (Exception exception)
        {
          MessageBox.Show("Exception occurred when creating feed:\r\n\r\n" + exception.Message, "Exception occurred.");
          this.fileLogger.Exception("Trying to create new feed: " + exception.Message);
          return;
        }
      });

      Task feedAddedTask = addFeedTask.ContinueWith((task) =>
      {
        if (task.IsCompleted)
        {
          this.fileLogger.Message("'" + feed.Title + "' added. Podcasts stored in '" + this.feedPath + "'");
        }
        else if (task.IsCanceled)
        {
          this.fileLogger.Message("Adding feed from '" + this.feedURL + "' was cancelled.");
        }
        else if (task.IsFaulted)
        {
          this.fileLogger.Exception("Trying to create new feed: " + task.Exception.Message);
        }
      });
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
