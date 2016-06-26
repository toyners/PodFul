
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for ProcessingWindow.xaml
  /// </summary>
  public partial class ProcessingWindow : Window
  {
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private TaskScheduler mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private Int64 fileSize;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;
    private Boolean fileSizeNotKnown;
    private String progressSizeLabel;
    private IFeedProcessor feedProcessor;
   
    public ProcessingWindow(IFeedProcessor feedProcessor)
    {
      this.feedProcessor = feedProcessor;

      InitializeComponent();
    }

    public ProcessingWindow(IFeedStorage feedStorage, Feed feed, Queue<Int32> podcastIndexes, IFileDeliverer fileDeliverer)
    {
      InitializeComponent();

      this.Title = "Downloading " + podcastIndexes.Count + " podcast" + (podcastIndexes.Count != 1 ? "s" : String.Empty);

      var cancelToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(false, cancelToken, fileDeliverer);

      Task task = Task.Factory.StartNew(() =>
      {
        this.SetCancelButtonStateEventHandler(true);
        if (podcastDownload.Download(feed.Directory, feed.Podcasts, podcastIndexes))
        {
          feedStorage.Update(feed);
        }

        this.SetCancelButtonStateEventHandler(false);
      }, cancelToken);
    }

    private void Cancel_Click(Object sender, RoutedEventArgs e)
    {
      this.feedProcessor.Cancel();
    }

    public void SetWindowTitleEventHandler(String title)
    {
      new Task(() =>
      {
        this.Title = title;

      }).Start(this.mainTaskScheduler);
    }

    private PodcastDownload InitialisePodcastDownload(Boolean isScanning, CancellationToken cancelToken, IFileDeliverer fileDeliverer)
    {
      var podcastDownload = new PodcastDownload(cancelToken, this.UpdateProgessEventHandler);

      podcastDownload.OnBeforeDownload += (podcast) =>
      {
        this.fileSize = podcast.FileSize;
        this.percentageStepSize = this.fileSize / 100;
        this.downloadedSize = 0;
        this.ResetProgressBar(podcast.FileSize);
        //this.PostMessage(String.Format("Downloading \"{0}\" ... ", podcast.Title), false);
      };

      if (isScanning)
      {
        podcastDownload.OnSuccessfulDownload += (podcast, filePath) =>
        {
          this.PostMessage("Completed.");
          fileDeliverer.Deliver(podcast, filePath);
        };
      }
      else
      {
        podcastDownload.OnSuccessfulDownload += (podcast, filePath) =>
        {
          this.PostMessage("Completed.");
          this.PostMessage(String.Empty); //Blank line to break up text flow
        };
      }

      podcastDownload.OnException += (podcast, exception) =>
      {
        Exception e = exception;
        if (exception is AggregateException)
        {
          e = ((AggregateException)exception).Flatten();
        }

        if (e.InnerException != null)
        {
          e = e.InnerException;
        }

        this.PostMessage(e.Message);
      };

      podcastDownload.OnFinish += () => this.ResetProgressBar(-1);

      return podcastDownload;
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

    private void ResetProgressBar(Int64 expectedFileSize = -1)
    {
      this.fileSizeNotKnown = (expectedFileSize == 0);
      String progressSize;
      if (expectedFileSize > 0)
      {
        var total = expectedFileSize / 1048576.0;
        this.progressSizeLabel = " / " + total.ToString("0.00") + "Mb";
        progressSize = "0.00" + this.progressSizeLabel;
      }
      else
      {
        this.progressSizeLabel = "Mb";
        progressSize = "0.00" + this.progressSizeLabel;
      }

      new Task(() =>
      {
        this.Progress.Value = 0;
        this.ProgressSize.Text = progressSize;
        this.Progress.IsIndeterminate = this.fileSizeNotKnown;
      }).Start(this.mainTaskScheduler);
    }

    public void SetCancelButtonStateEventHandler(Boolean state)
    {
      new Task(() =>
      {
        this.Cancel.IsEnabled = state;
      }).Start(this.mainTaskScheduler);
    }

    private Feed SynchroniseFeed(Feed newFeed, Int32 podcastIndex, Feed oldFeed)
    {
      Int32 i = 0;
      while (podcastIndex < newFeed.Podcasts.Length && i < oldFeed.Podcasts.Length)
      {
        if (!oldFeed.Podcasts[i].Equals(newFeed.Podcasts[podcastIndex]))
        {
          break;
        }

        newFeed.Podcasts[podcastIndex] = oldFeed.Podcasts[i];
        i++;
        podcastIndex++;
      }

      return newFeed;
    }

    private void UpdateProgessEventHandler(Int32 bytesWrittenToFile)
    {
      this.downloadedSize += bytesWrittenToFile;
      var downloadedSizeInMb = this.downloadedSize / 1048576.0;

      if (this.fileSizeNotKnown)
      {
        new Task(() =>
        {
          this.ProgressSize.Text = downloadedSizeInMb.ToString("0.00") + this.progressSizeLabel;
        }).Start(this.mainTaskScheduler);

        return;
      }

      new Task(() =>
      {
        if (this.downloadedSize > this.fileSize)
        {
          this.Progress.Value = 100;
          return;
        }

        Int64 steps = this.downloadedSize / this.percentageStepSize;
        if (steps > this.Progress.Value)
        {
          this.Progress.Value = (Int32)steps;
        }

        this.ProgressSize.Text = downloadedSizeInMb.ToString("0.00") + this.progressSizeLabel;

      }).Start(this.mainTaskScheduler);
    }

    private void Window_Initialized(Object sender, EventArgs e)
    {
      this.feedProcessor.Process();
    }
  }
}
