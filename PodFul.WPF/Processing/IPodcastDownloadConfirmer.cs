
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using Library;

  public enum DownloadConfirmationStatus
  {
    CancelScanning,
    ContinueDownloading,
    SkipDownloading,
  }

  public interface IPodcastDownloadConfirmer
  {
    DownloadConfirmationStatus ConfirmPodcastsForDownload(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes);
  }
}
