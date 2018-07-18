
namespace PodFul.WPF.Processing.TileView
{
  using System;
  using System.Threading;

  public interface IFileDownloadProxy
  {
    void Download(String url, String destination, CancellationToken cancelToken, Action<Int32> downloadProgressEventHandler);
  }
}
