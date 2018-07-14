
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using PodFul.Library;

  public interface IFileDownloadProxyFactory
  {
    IFileDownloadProxy Create();
  }

  public interface IFileDownloadProxy
  {
    void Download(String url, String destination, CancellationToken cancelToken, Action<Int32> downloadProgressEventHandler);
  }

  public class FileDownloadProxyFactory : IFileDownloadProxyFactory
  {
    public IFileDownloadProxy Create()
    {
      return new FileDownloadProxy();
    }
  }

  public class FileDownloadProxy : IFileDownloadProxy
  {
    public void Download(string url, string destination, CancellationToken cancelToken, Action<int> downloadProgressEventHandler)
    {
      var fileDownloader = new FileDownloader();
      fileDownloader.Download(url, destination, cancelToken, downloadProgressEventHandler);
    }
  }
}
