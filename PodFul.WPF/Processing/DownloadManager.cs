
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Threading.Tasks;
  using Library;

  public class DownloadManager
  {
    private Feed feed;
    private FeedCollection feedCollection; // Is this needed? Maybe only the feed
    private IFileDeliverer fileDeliverer;
    private GUILogger guiLogger; // Should this be a gui logger or just a file logger?
    private IImageResolver imageResolver;
    private Queue<Int32> podcastIndexes;

    private Queue<PodcastMonitor> podcasts;

    public DownloadManager(FeedCollection feedCollection, Feed feed, Queue<Int32> podcastIndexes, IImageResolver imageResolver, IFileDeliverer fileDeliverer, GUILogger guiLogger)
    {
      this.feedCollection = feedCollection;
      this.feed = feed;
      this.podcastIndexes = podcastIndexes;
      this.imageResolver = imageResolver;
      this.fileDeliverer = fileDeliverer;
      this.guiLogger = guiLogger;
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
        var downloader = new FileDownloader();
        task = downloader.DownloadAsync(podcast.URL, podcast.FilePath, podcast.CancellationToken, podcast.ProgressEventHandler);
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
          // record missing file exception
          this.ProcessException(new Exception("Podcast file is missing."), podcast);
          return;
        }

        podcast.SetPodcastFileDetails(this.imageResolver, fileInfo.Length);
        podcast.DeliverPodcastFile(this.fileDeliverer, fileInfo.FullName);

        taskCompletionFunc(t);
      });

      task.Start();
    }

    public void PodcastDownloadCompleted()
    {
      this.feedCollection.UpdateFeed(feed);
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

      this.guiLogger.Exception(e.Message);
      podcast.ExceptionMessage = e.Message;
    }
  }
}
