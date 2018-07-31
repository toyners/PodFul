
namespace PodFul.WPF.Processing.TileView
{
  using System;
  using System.Threading;
  using PodFul.Library;

  public class FileDownloadProxy : IFileDownloadProxy
  {
    public void Download(string url, string destination, CancellationToken cancelToken, Action<int> downloadProgressEventHandler)
    {
      var fileDownloader = new FileDownloader();
      fileDownloader.Download(url, destination, cancelToken, downloadProgressEventHandler);
    }
  }
}
