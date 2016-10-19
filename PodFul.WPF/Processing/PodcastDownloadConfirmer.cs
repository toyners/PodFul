
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
    public delegate void ConfirmPodcastsForDownloadDelegate(Feed oldFeed, Feed newFeed, List<Int32> indexes, Action<Boolean, List<Int32>> callback);

    private Int32 confirmPodcastDownloadThreshold;

    private List<Int32> indexes;

    private ConfirmPodcastsForDownloadDelegate confirmPodcastsForDownloadEvent;

    public PodcastDownloadConfirmer(Settings settings, ConfirmPodcastsForDownloadDelegate confirmPodcastsForDownloadEventHandler)
    {
      this.confirmPodcastDownloadThreshold = settings.ConfirmPodcastDownloadThreshold;
      this.confirmPodcastsForDownloadEvent = confirmPodcastsForDownloadEventHandler;
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
