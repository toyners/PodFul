
namespace PodFul.WPF.Testbed.Processing
{
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using ViewModel;

  public class Scanner
  {
    public void ScanFeeds(IList<FeedViewModel> feeds, IDownloadManagerFactory downloadManagerFactory)
    {
      Task.Factory.StartNew(() =>
      {
        foreach (var feed in feeds)
        {
          feed.Scan(downloadManagerFactory);
        }
      });
    }
  }
}
