
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using Library;
  using Windows;

  public class TestFeedProcessor : IFeedProcessor
  {
    public IList<Feed> Feeds { get; private set; }

    public Action FinishedAddingFeed { get; set; }

    public Action StartedAddingFeed { get; set; }

    public TestFeedProcessor()
    {
      this.Feeds = new List<Feed>();
    }

    public Feed AddFeed(String directory, String url, String defaultPodcastImageFilePath, CancellationToken cancelToken)
    {
      this.StartedAddingFeed?.Invoke();

      var feed = new Feed("Title", "Description", "", "", "", "", "",
        new Podcast[0], DateTime.MinValue, DateTime.MinValue, true, true, true, 3u);

      this.Feeds.Add(feed);

      this.FinishedAddingFeed?.Invoke();

      return feed;
    }

    public void RemoveFeed(Int32 index)
    {
      throw new NotImplementedException();
    }

    public void RemoveFeed(Feed feed)
    {
      throw new NotImplementedException();
    }

    public void ScanFeeds(IList<Int32> indexes)
    {
      throw new NotImplementedException();
    }
  }
}
