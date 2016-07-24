
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using Jabberwocky.Toolkit.Object;
  using Library;

  public class PodcastDownload
  {
    private readonly CancellationToken cancellationToken;

    private readonly Action<Int32> updateProgress;

    public PodcastDownload(CancellationToken cancellationToken, Action<Int32> updateProgress)
    {
      cancellationToken.VerifyThatObjectIsNotNull("Parameter 'cancellationToken' is null.");
      updateProgress.VerifyThatObjectIsNotNull("Parameter 'updateProgress' is null.");
      this.cancellationToken = cancellationToken;
      this.updateProgress = updateProgress;
    }

    public event Action<Podcast> OnBeforeDownload;

    public event Action<Podcast, Exception> OnException;

    public event Action<Podcast, String> OnSuccessfulDownload;

    public event Action<Podcast> OnCancelledDownload;

    public event Action OnFinish;

    public Boolean Download(String directoryPath, Podcast[] podcasts, Queue<Int32> podcastsIndexes)
    {
      var downloader = new FileDownloader();

      while (podcastsIndexes.Count > 0)
      {
        var podcastIndex = podcastsIndexes.Dequeue();
        var podcast = podcasts[podcastIndex];

        try
        {
          this.OnBeforeDownload?.Invoke(podcast);

          var filePath = Path.Combine(directoryPath, podcast.URL.Substring(podcast.URL.LastIndexOf('/') + 1));

          downloader.Download(podcast.URL, filePath, this.cancellationToken, this.updateProgress);

          if (this.cancellationToken.IsCancellationRequested)
          {
            this.OnCancelledDownload?.Invoke(podcast);
            return false;
          }

          this.OnSuccessfulDownload?.Invoke(podcast, filePath);

          podcast = Podcast.SetDownloadDate(DateTime.Now, podcast);

          var fileLength = new FileInfo(filePath).Length;
          if (podcast.FileSize != fileLength)
          {
            podcast = Podcast.SetFileSize(fileLength, podcast);
          }

          podcasts[podcastIndex] = podcast;
        }
        catch (AggregateException exception)
        {
          this.OnException?.Invoke(podcast, exception);
        }
      }

      this.OnFinish?.Invoke();
      return true;
    }
  }
}
