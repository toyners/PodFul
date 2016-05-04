
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Jabberwocky.Toolkit.Object;
  using Library;

  /*public delegate void ResetProgressDelegate(Int64 fileSize);
  public delegate void PostMessageDelegate(String message);
  public delegate void PostMessageWithLineBreakDelegate(String message, Boolean addLineBreak);
  public delegate void DownloadCompleteDelegate(Podcast podcast);
  public delegate void DownloadSuccessfulDelegate(Podcast podcast);*/

  public class PodcastDownload
  {
    private CancellationToken cancellationToken;

    private Action<Int32> updateProgress;

    public PodcastDownload(CancellationToken cancellationToken, Action<Int32> updateProgress)
    {
      cancellationToken.VerifyThatObjectIsNotNull("Parameter 'cancellationToken' is null.");
      if (updateProgress == null)
      {
        throw new Exception("Parameter 'úpdateProgress' is null.");
      }

      this.cancellationToken = cancellationToken;
      this.updateProgress = updateProgress;
    }

    public event Action<Podcast> OnBeforeDownload;

    public event Action<AggregateException, Podcast> OnException;

    public event Action<Podcast> OnSuccessfulDownload;

    public event Action<Podcast> OnCancelledDownload;

    public event Action OnFinish;

    public Boolean Download(String directoryPath, Podcast[] podcasts, Queue<Int32> podcastsIndexes)
    {
      BigFileDownloader downloader = new BigFileDownloader();

      while (podcastsIndexes.Count > 0)
      {
        var podcastIndex = podcastsIndexes.Dequeue();
        var podcast = podcasts[podcastIndex];

        this.OnBeforeDownload?.Invoke(podcast);

        var filePath = Path.Combine(directoryPath, podcast.URL.Substring(podcast.URL.LastIndexOf('/') + 1));
        Task downloadTask = downloader.DownloadAsync(podcast.URL, filePath, this.cancellationToken, this.updateProgress);

        try
        {
          downloadTask.Wait();

          if (downloadTask.IsCanceled)
          {
            this.OnCancelledDownload?.Invoke(podcast);
            // Downloading cancelled. Regardless of what was previously downloaded we will not
            // update the feed file.
            return false;
          }

          this.OnSuccessfulDownload?.Invoke(podcast);

          podcasts[podcastIndex] = Podcast.SetDownloadDate(podcast, DateTime.Now);
        }
        catch (AggregateException exception)
        {
          this.OnException?.Invoke(exception, podcast);
        }
      }

      this.OnFinish?.Invoke();
      return true;
    }
  }
}
