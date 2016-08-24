
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Library;

  public class DownloadManager
  {
    public void Download(String directory, Podcast[] podcasts, Queue<Int32> podcastsIndexes, Int32 threadCount)
    {
      while (podcastsIndexes.Count > 0)
      {
        var podcastIndex = podcastsIndexes.Dequeue();
        var podcast = podcasts[podcastIndex];

        try
        {

        }
        catch (Exception e)
        {
        }
      }
    }
  }
}
