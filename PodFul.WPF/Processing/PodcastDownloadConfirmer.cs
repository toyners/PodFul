
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
    private Int32 confirmPodcastDownloadThreshold;
    #endregion

    #region Construction
    public PodcastDownloadConfirmer(Settings settings)
    {
      this.confirmPodcastDownloadThreshold = settings.ConfirmPodcastDownloadThreshold;
    }
    #endregion

    #region Methods
    public DownloadConfirmationStatus ConfirmPodcastsForDownload(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes)
    {
      if (podcastIndexes.Count >= this.confirmPodcastDownloadThreshold)
      {
        // Breached the threshold of new podcasts in the feed so query the user on what to do.
        Boolean? confirmationResult = null;

        Application.Current.Dispatcher.Invoke(() =>
        {
          var window = new DownloadConfirmation(oldFeed, newFeed);
          window.ShowDialog();
          
          confirmationResult = (window.Result != MessageBoxResult.Cancel);
        });

        // Loop here until the confirmationResult is set on the other thread. This indicates that
        // the user has made their choice.
        while (!confirmationResult.HasValue)
        {
          Thread.Sleep(100);
        }

        if (!confirmationResult.Value)
        {
          return DownloadConfirmationStatus.CancelScanning;
        }
      }

      if (podcastIndexes == null)
      {
        return DownloadConfirmationStatus.SkipDownloading;
      }

      return DownloadConfirmationStatus.ContinueDownloading;
    }
    #endregion
  }
}
