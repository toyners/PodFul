
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Library;

  public class DownloadManager
  {
    public IEnumerable<Boolean> Download(String directoryPath, PodcastMonitor[] podcasts, Int32 threadCount)
    {
      for (int i = 0; i < podcasts.Length; i++)
      {
        var podcast = podcasts[i];

        try
        {
          var downloader = new FileDownloader();
          downloader.DownloadAsync(podcast.URL, podcast.FilePath, podcast.CancellationToken, podcast.ProgressEventHandler);

          //var filePath = Path.Combine(directoryPath, podcast.FilePath);

          //downloader.DownloadAsync(podcast.URL, podcast.FilePath, podcast.CancellationToken)

        }
        catch (Exception e)
        {
          // Log exception and then continue;
        }

        yield return true;
      }

      yield return false;

    }
  }
}
