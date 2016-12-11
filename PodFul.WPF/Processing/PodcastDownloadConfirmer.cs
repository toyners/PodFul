
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Windows;
  using Library;
  using Windows;

  /// <summary>
  /// Provides the user an opportunity to either cancel scanning, skip downloading or finalise the list
  /// of podcasts to download.
  /// </summary>
  public class PodcastDownloadConfirmer : IPodcastDownloadConfirmer
  {
    #region Fields
    private UInt32 confirmPodcastDownloadThreshold;
    #endregion

    #region Construction
    public PodcastDownloadConfirmer(UInt32 confirmPodcastDownloadThreshold)
    {
      this.confirmPodcastDownloadThreshold = confirmPodcastDownloadThreshold;
    }
    #endregion

    #region Methods
    public DownloadConfirmationStatus ConfirmPodcastsForDownload(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes)
    {
      if (podcastIndexes.Count < this.confirmPodcastDownloadThreshold)
      {
        return DownloadConfirmationStatus.ContinueDownloading;
      }

      // Breached the threshold of new podcasts in the feed so query the user on what to do.
      Boolean gotResult = false;
      MessageBoxResult result = MessageBoxResult.Cancel;

      Application.Current.Dispatcher.Invoke(() =>
      {
        var window = new DownloadConfirmationWindow(oldFeed, newFeed);
        window.ShowDialog();
        gotResult = true;  

        result = window.Result;
        if (result == MessageBoxResult.Yes)
        {
          podcastIndexes.Clear();
          podcastIndexes.AddRange(window.PodcastIndexes);
        }
      });

      // Loop here until the gotResult is set on the other thread. This indicates that
      // the user has made their choice.
      while (!gotResult)
      {
        Thread.Sleep(100);
      }

      if (result == MessageBoxResult.Cancel)
      {
        return DownloadConfirmationStatus.CancelScanning;
      }

      if (result == MessageBoxResult.No)
      {
        return DownloadConfirmationStatus.SkipDownloading;
      }

      return DownloadConfirmationStatus.ContinueDownloading;
    }
    #endregion
  }
}
