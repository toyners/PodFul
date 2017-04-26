
namespace PodFul.WPF.Processing
{
  using System;
  using System.Threading;
  using Library;

  public class ImageResolver : IImageResolver
  {
    #region Fields
    private String imageDirectoryPath;
    #endregion

    #region Construction
    public ImageResolver(string imageDirectoryPath, string defaultImagePath)
    {
      this.imageDirectoryPath = imageDirectoryPath;
      this.DefaultImagePath = defaultImagePath;
    }
    #endregion

    #region Properties
    public String DefaultImagePath { get; private set; }
    #endregion

    #region Events
    public event Action<Int32> TotalDownloadsRequiredEvent;

    public event Action<Int32, String> StartDownloadNotificationEvent;

    public event Action<Int32, String> SkippedDownloadNotificationEvent;

    public event Action<Int32, String> CompletedDownloadNotificationEvent;

    public event Action<String, Exception> FailedDownloadNotificationEvent;
    #endregion

    #region Methods
    public Feed ResolveFeedImage(Feed feed)
    {
      return ImageFunctions.resolveImageForFeed(feed, this.imageDirectoryPath, this.DefaultImagePath, FailedDownloadNotificationEvent);
    }

    public void ResolvePodcastImage(Podcast podcast)
    {
      ImageFunctions.resolvePodcastImage(
        podcast,
        this.imageDirectoryPath,
        this.DefaultImagePath,
        this.FailedDownloadNotificationEvent);
    }

    public void ResolvePodcastImagesForFeed(Feed feed, CancellationToken cancellationToken)
    {
      ImageFunctions.resolveImagesForPodcasts(
        feed.Podcasts, 
        this.imageDirectoryPath, 
        this.DefaultImagePath, 
        this.TotalDownloadsRequiredEvent, 
        this.StartDownloadNotificationEvent, 
        this.SkippedDownloadNotificationEvent, 
        this.CompletedDownloadNotificationEvent, 
        this.FailedDownloadNotificationEvent, 
        cancellationToken);
    }

    public void ResolvePodcastImages(Podcast[] podcasts, CancellationToken cancellationToken)
    {
      ImageFunctions.resolveImagesForPodcasts(
        podcasts,
        this.imageDirectoryPath,
        this.DefaultImagePath,
        this.TotalDownloadsRequiredEvent,
        this.StartDownloadNotificationEvent,
        this.SkippedDownloadNotificationEvent,
        this.CompletedDownloadNotificationEvent,
        this.FailedDownloadNotificationEvent,
        cancellationToken);
    }
    #endregion
  }
}