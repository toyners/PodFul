
namespace PodFul.WPF.Processing
{
  using System;
  using System.Threading;
  using Library;

  public class ImageResolver : IImageResolver
  {
    #region Fields
    private String imageDirectoryPath;
    private String defaultImagePath;
    #endregion

    #region Construction
    public ImageResolver(string imageDirectoryPath, string defaultImagePath)
    {
      this.imageDirectoryPath = imageDirectoryPath;
      this.defaultImagePath = defaultImagePath;
    }
    #endregion

    public event Action<Int32> TotalDownloadsRequiredEvent;

    public event Action<Int32, String> StartDownloadNotificationEvent;

    public event Action<Int32, String> SkippedDownloadNotificationEvent;

    public event Action<Int32, String> CompletedDownloadNotificationEvent;

    public event Action<String, Exception> FailedDownloadNotificationEvent;

    #region Methods
    public Feed ResolveFeedImage(Feed feed)
    {
      return ImageFunctions.resolveImageForFeed(feed, this.imageDirectoryPath, this.defaultImagePath, FailedDownloadNotificationEvent);
    }

    public void ResolvePodcastImagesForFeed(Feed feed, CancellationToken cancellationToken)
    {
      ImageFunctions.resolveImagesForPodcasts(
        feed.Podcasts, 
        this.imageDirectoryPath, 
        this.defaultImagePath, 
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