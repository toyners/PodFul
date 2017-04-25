
namespace PodFul.WPF.Processing
{
  using System;
  using System.IO;
  using System.Threading;
  using Jabberwocky.Toolkit.String;
  using Library;

  public class FeedFactory : IFeedFactory
  {
    private String path;
    private String defaultPodcastImageFilePath;

    public String FeedURL { get; private set; }

    public FeedFactory(String path, String url, String defaultPodcastImageFilePath)
    {
      path.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'path' is null or empty.");
      url.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'url' is null or empty.");
      defaultPodcastImageFilePath.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'defaultPodcastImageFilePath' is null or empty.");

      this.path = path;
      this.FeedURL = url;
      this.defaultPodcastImageFilePath = defaultPodcastImageFilePath;
    }

    public Feed Create(CancellationToken cancelToken)
    {
      var feedFilePath = Path.Combine(this.path, "download.rss");
      return FeedFunctions.CreateFeed(this.FeedURL, feedFilePath, this.path, this.defaultPodcastImageFilePath, cancelToken);
    }
  }
}
