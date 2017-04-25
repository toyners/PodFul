
namespace PodFul.WPF.Processing
{
  using System;
  using System.Threading;
  using Library;

  public interface IImageResolver
  {
    event Action<Int32> TotalDownloadsRequiredEvent;
    event Action<Int32, String> StartDownloadNotificationEvent;
    event Action<Int32, String> SkippedDownloadNotificationEvent;
    event Action<Int32, String> CompletedDownloadNotificationEvent;
    event Action<String, Exception> FailedDownloadNotificationEvent;

    string DefaultImagePath { get; }

    Feed ResolveFeedImage(Feed feed);

    void ResolvePodcastImagesForFeed(Feed feed, CancellationToken cancellationToken);
  }
}
