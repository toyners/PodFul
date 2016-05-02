
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Library;

  public delegate void ResetProgressDelegate(Int64 fileSize);
  public delegate void PostMessageDelegate(String message);
  public delegate void PostMessageWithLineBreakDelegate(String message, Boolean addLineBreak);

  public static class PodcastDownload
  {
    public static event ResetProgressDelegate ResetProgress;

    public static event PostMessageDelegate PostMessage;

    public static event PostMessageWithLineBreakDelegate PostMessageWithLineBreak;

    public static Boolean Download(String directoryPath, Podcast[] podcasts, Queue<Int32> selectedPodcasts)
    {
      BigFileDownloader downloader = new BigFileDownloader();

      var podcast = podcasts[0];

      ResetProgress?.Invoke(podcast.FileSize);
      PostMessageWithLineBreak?.Invoke(String.Format("Downloading \"{0}\" ...", podcast.Title), false);

      var filePath = Path.Combine(directoryPath, podcast.URL.Substring(podcast.URL.LastIndexOf('/') + 1));
      Task downloadTask = downloader.DownloadAsync(podcast.URL, filePath, this.cancellationToken, this.UpdateProgessEventHandler);


      throw new NotImplementedException();
    }
  }
}
