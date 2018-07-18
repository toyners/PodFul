namespace PodFul.WPF.Processing.TileView
{
  public class FileDownloadProxyFactory : IFileDownloadProxyFactory
  {
    public IFileDownloadProxy Create()
    {
      return new FileDownloadProxy();
    }
  }
}