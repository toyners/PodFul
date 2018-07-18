
namespace PodFul.WPF.Processing.TileView
{
  using System;
  using System.Threading;

  public class FileDownloadProxy : IFileDownloadProxy
  {
    public void Download(string url, string destination, CancellationToken cancelToken, Action<int> downloadProgressEventHandler)
    {
      throw new NotImplementedException();
    }
  }
}
