
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
    private FeedCollection feedCollection;
    private IFileDeliverer fileDeliverer;
    private GUILogger guiLogger;
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
      try
      {
        var podcast = this.podcasts.Dequeue();
        var downloader = new FileDownloader();
        task = downloader.DownloadAsync(podcast.URL, podcast.FilePath, podcast.CancellationToken, podcast.ProgressEventHandler);

        task.ContinueWith(t =>
        {
          if (t.Exception != null)
          {
            podcast.ExceptionEventHandler(t.Exception);
            return;
          }

          var fileInfo = new System.IO.FileInfo(podcast.FilePath);
          if (!fileInfo.Exists)
          {
            // record missing file exception
            podcast.ExceptionEventHandler(new Exception("Podcast file is missing."));
            return;
          }

          podcast.SetPodcastFileDetails(fileInfo.Length, this.imageResolver);
          podcast.DeliverPodcastFile(this.fileDeliverer, fileInfo.FullName);

          taskCompletionFunc(t);
        });
      }
      catch (Exception e)
      {
        // Catch any exceptions regarding the setup of the task. May not be necessary
        this.guiLogger.Exception(e.Message);
        return;
      }

      task.Start();
    }

    public void PodcastDownloadCompleted()
    {
      this.feedCollection.UpdateFeed(feed);
    }
  }
}
