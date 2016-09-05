
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Threading.Tasks;
  using System.Windows;
  using Library;

  public class DownloadManager
  {
    private Feed feed;
    private FeedCollection feedCollection; // Is this needed? Maybe only the feed
    private IFileDeliverer fileDeliverer;
    private ILogger logger;
    private IImageResolver imageResolver;

    private Queue<PodcastMonitor> podcasts;

    public DownloadManager(FeedCollection feedCollection, Feed feed, List<Int32> podcastIndexes, IImageResolver imageResolver, IFileDeliverer fileDeliverer, ILogger logger)
    {
      this.feedCollection = feedCollection;
      this.feed = feed;
      this.imageResolver = imageResolver;
      this.fileDeliverer = fileDeliverer;
      this.logger = logger;

      this.podcasts = new Queue<PodcastMonitor>();
      this.Podcasts = new ObservableCollection<PodcastMonitor>();
      foreach (var index in podcastIndexes)
      {
        var podcast = feed.Podcasts[index];
        var podcastMonitor = new PodcastMonitor(podcast, podcast.FileDetails.FileSize, feed.Directory);

        this.podcasts.Enqueue(podcastMonitor);
        this.Podcasts.Add(podcastMonitor);
      }
    }

    public ObservableCollection<PodcastMonitor> Podcasts { get; private set; } 

    public void DownloadNextPodcast(Action<Task> taskCompletionFunc)
    {
      if (this.podcasts.Count == 0)
      {
        return;
      }

      Task task = null;
      var podcast = this.podcasts.Dequeue();

      try
      {
        task = Task.Factory.StartNew(() =>
        {
          var downloader = new FileDownloader();
          downloader.Download(podcast.URL, podcast.FilePath, podcast.CancellationToken, podcast.ProgressEventHandler);
        }, podcast.CancellationToken);
      }
      catch (Exception e)
      {
        // Catch any exceptions regarding the setup of the task. May not be necessary
        this.ProcessException(e, podcast);
        return;
      }

      task.ContinueWith(t =>
      {
        if (t.Exception != null)
        {
          this.ProcessException(t.Exception, podcast);
          return;
        }

        var fileInfo = new System.IO.FileInfo(podcast.FilePath);
        if (!fileInfo.Exists)
        {
          // TODO: Change to file not found exception
          this.ProcessException(new Exception(String.Format("Podcast file '{0}' is missing.", podcast.FilePath)), podcast);
          return;
        }

        podcast.SetPodcastFileDetails(this.imageResolver, fileInfo.Length);
        //TODO: Turn on podcast delivery -> podcast.DeliverPodcastFile(this.fileDeliverer, fileInfo.FullName);
        podcast.DownloadCompleted();

        this.feedCollection.UpdateFeed(feed);

        taskCompletionFunc(t);
      });
    }

    private void ProcessException(Exception exception, PodcastMonitor podcast)
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

      this.logger.Exception(e.Message);

      Application.Current.Dispatcher.Invoke(() =>
      {
        podcast.Message = e.Message;
      });
    }
  }
}
