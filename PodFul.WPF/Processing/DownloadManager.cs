
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Library;

  public class DownloadManager
  {
    private Feed currentFeed;
    private FeedCollection feedCollection;
    private IFileDeliverer fileDeliverer;
    private GUILogger guiLogger;
    private IImageResolver imageResolver;
    private Queue<Int32> podcastIndexes;

    private Queue<PodcastMonitor> podcasts;

    public DownloadManager(FeedCollection feedCollection, Feed currentFeed, Queue<Int32> podcastIndexes, IImageResolver imageResolver, IFileDeliverer fileDeliverer, GUILogger guiLogger)
    {
      this.feedCollection = feedCollection;
      this.currentFeed = currentFeed;
      this.podcastIndexes = podcastIndexes;
      this.imageResolver = imageResolver;
      this.fileDeliverer = fileDeliverer;
      this.guiLogger = guiLogger;
    }

    public ObservableCollection<PodcastMonitor> Podcasts { get; private set; } 

    public Boolean DownloadNextPodcast()
    {
      if (this.podcasts.Count == 0)
      {
        return false;
      }

      try
      {
        var podcast = this.podcasts.Dequeue();
        var downloader = new FileDownloader();
        downloader.DownloadAsync(podcast.URL, podcast.FilePath, podcast.CancellationToken, podcast.ProgressEventHandler);
      }
      catch (Exception e)
      {
        this.guiLogger.Exception(e.Message);
      }

      return true;
    }
  }
}
