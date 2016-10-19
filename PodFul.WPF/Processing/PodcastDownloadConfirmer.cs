
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Windows;
  using Library;
  using Windows;

  public class PodcastDownloadConfirmer : IPodcastDownloadConfirmer
  {
    private Int32 confirmPodcastDownloadThreshold;

    public PodcastDownloadConfirmer(Settings settings)
    {
      this.confirmPodcastDownloadThreshold = settings.ConfirmPodcastDownloadThreshold;
    }

    public DownloadConfirmationStatus ConfirmPodcastsForDownload(Feed oldFeed, Feed newFeed, List<Int32> podcastIndexes)
    {
      if (podcastIndexes.Count >= this.confirmPodcastDownloadThreshold)
      {
        Boolean? confirmationResult = null;

        Application.Current.Dispatcher.Invoke(() =>
        {
          var window = new DownloadConfirmation(oldFeed, newFeed, podcastIndexes);
          window.ShowDialog();
          
          confirmationResult = (window.Result != MessageBoxResult.Cancel);
        });

        while (!confirmationResult.HasValue)
        {
          Thread.Sleep(100);
        }

        if (!confirmationResult.Value)
        {
          return DownloadConfirmationStatus.CancelScanning;
        }
      }

      if (podcastIndexes.Count == 0)
      {
        return DownloadConfirmationStatus.SkipDownloading;
      }

      return DownloadConfirmationStatus.ContinueDownloading;
    }
  }
}
