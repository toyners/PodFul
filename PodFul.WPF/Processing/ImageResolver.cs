
namespace PodFul.WPF.Processing
{
  using System;
  using System.Threading;
  using Library;

  public class ImageResolver
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

    #region Methods
    public Feed ResolveFeedImage(Feed feed)
    {
      return ImageFunctions.resolveImageForFeed(feed, this.imageDirectoryPath, this.defaultImagePath, null);
    }

    public void ResolvePodcastImagesForFeed(Feed feed, CancellationToken cancellationToken)
    {
      ImageFunctions.resolveImagesForPodcasts(feed.Podcasts, this.imageDirectoryPath, this.defaultImagePath, null, null, null, null, null, cancellationToken);
    }
    #endregion
  }
}
