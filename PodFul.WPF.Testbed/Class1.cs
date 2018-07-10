
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;

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
      throw new NotImplementedException();
    }
  }

  public class FileDownloadProxy : IFileDownloadProxy
  {
    public void Download(string url, string destination, CancellationToken cancelToken, Action<int> downloadProgressEventHandler)
    {
      throw new NotImplementedException();
    }
  }
}
