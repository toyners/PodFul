
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using Library;

  public interface IFeedProcessor
  {
    IList<Feed> Feeds { get; }
    Action StartedAddingFeed { get; set; }
    Action FinishedAddingFeed { get; set; }

    Feed this[Int32 index] { get; set; }

    Feed AddFeed(String directory, String url, String defaultPodcastImageFilePath, CancellationToken cancelToken);
    void RemoveFeed(Feed feed);
    void RemoveFeed(Int32 index);
    void ScanFeeds(IList<Int32> indexes);
  }
}
