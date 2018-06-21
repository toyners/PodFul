
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using PodFul.Library;

  public class NewDownloadManager : INewDownloadManager
  {
    private CancellationTokenSource cancellationTokenSource;
    private Queue<Int32> podcastIndexes;
    private Feed feed;

    public Int32 Count { get; private set; }

    public Action<Int32> DownloadProgressEventHandler { get; set; }
    public event Action<Podcast> DownloadCompletedEvent;
    public Action<Podcast> DownloadStartingEvent { get; set; }

    public void AddJobs(IList<Int32> podcastIndexes, Feed feed)
    {
      this.podcastIndexes = new Queue<Int32>(podcastIndexes);
      this.feed = feed;
      this.Count = podcastIndexes.Count;
    }

    public void CancelJobs()
    {
      if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested)
      {
        this.cancellationTokenSource.Cancel();
      }
    }

    public void DownloadPodcasts()
    {
      this.cancellationTokenSource = new CancellationTokenSource();
      var cancelToken = this.cancellationTokenSource.Token;
      var fileDownloader = new FileDownloader();

      while (this.podcastIndexes.Count > 0)
      {
        var podcastIndex = this.podcastIndexes.Dequeue();
        var podcast = this.feed.Podcasts[podcastIndex];

        this.DownloadStartingEvent?.Invoke(podcast);

        var filePath = Path.Combine(this.feed.Directory, podcast.FileDetails.FileName);
        fileDownloader.Download(podcast.URL, filePath, cancelToken, this.DownloadProgressEventHandler);

        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
          throw new FileNotFoundException(String.Format("Podcast file '{0}' is missing.", filePath));
        }

        podcast.SetFileDetails(fileInfo.Length, DateTime.Now);

        this.DownloadCompletedEvent?.Invoke(podcast);
      }
    }
  }
}
