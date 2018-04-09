
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
    Action<Int32> TotalDownloadsRequiredEvent { get; set; }
    Action<Int32, String> StartDownloadNotificationEvent { get; set; }
    Action<Int32, String> SkippedDownloadNotificationEvent { get; set; }
    Action<Int32, String> CompletedDownloadNotificationEvent { get; set; }
    Action<String, Exception> FailedDownloadNotificationEvent { get; set; }
    #endregion

    #region Methods
    Feed ResolveFeedImage(Feed feed);

    void ResolvePodcastImage(Podcast podcast);
   
    void ResolvePodcastImagesForFeed(Feed feed, CancellationToken cancellationToken);

    void ResolvePodcastImages(Podcast[] podcasts, CancellationToken cancellationToken);
    #endregion
  }
}
