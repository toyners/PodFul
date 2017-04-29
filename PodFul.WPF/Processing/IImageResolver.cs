
namespace PodFul.WPF.Processing
{
  using System;
  using System.Threading;
  using Library;

  public interface IImageResolver
  {
    #region Properties
    string DefaultImagePath { get; }
    #endregion

    #region Events
    event Action<Int32> TotalDownloadsRequiredEvent;
    event Action<Int32, String> StartDownloadNotificationEvent;
    event Action<Int32, String> SkippedDownloadNotificationEvent;
    event Action<Int32, String> CompletedDownloadNotificationEvent;
    event Action<String, Exception> FailedDownloadNotificationEvent;
    #endregion

    #region Methods
    Feed ResolveFeedImage(Feed feed);

    void ResolvePodcastImage(Podcast podcast);
   
    void ResolvePodcastImagesForFeed(Feed feed, CancellationToken cancellationToken);

    void ResolvePodcastImages(Podcast[] podcasts, CancellationToken cancellationToken);
    #endregion
  }
}
