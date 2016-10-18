
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Library;

  public enum DownloadConfirmationStatus
  {
    CancelScanning,
    ContinueDownloading,
    SkipDownloading,
  }

  interface IPodcastDownloadConfirmer
  {
    DownloadConfirmationStatus ConfirmPodcastsForDownload(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes);
  }
}
