
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

  /// <summary>
  /// Provides method to confirm podcasts for downloading. Will return one of three options 1) Cancel feed scanning,
  /// 2) Skip downloading or 3) Continue downloading with the finalised podcast list
  /// </summary>
  public interface IPodcastDownloadConfirmer
  {
    UInt32 ConfirmDownloadThreshold { get; set; }

    DownloadConfirmationStatus ConfirmPodcastsForDownload(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes);
  }
}
