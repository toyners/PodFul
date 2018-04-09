
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using Library;
  using Logging;

  public class FeedProcessor : IFeedProcessor
  {
    private ILogController logController;
    private IFeedStorage feedStorage;

    public FeedProcessor(IFeedStorage feedStorage, ILogController logController)
    {
      this.feedStorage = feedStorage;
      this.logController = logController;
    }

    public IList<Feed> Feeds { get { return this.feedStorage.Feeds; } }

    public Action FinishedAddingFeed { get; set; }

    public Action StartedAddingFeed { get; set; }

    public Feed this[Int32 index]
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public Feed AddFeed(String directory, String url, String defaultPodcastImageFilePath, CancellationToken cancelToken)
    {
      try
      {
        var feedFilePath = Path.Combine(directory, "download.rss");
        var feed = FeedFunctions.CreateFeed(url, feedFilePath, directory, defaultPodcastImageFilePath, cancelToken);
        this.feedStorage.Add(feed);
        this.logController.Message(LoggerKeys.InfoKey, "'" + feed.Title + "' added. Podcasts stored in '" + directory + "'");
        return feed;
      }
      catch (OperationCanceledException oce)
      {
        this.logController.Message(LoggerKeys.InfoKey, "Adding feed from '" + url + "' was cancelled.");
      }
      catch (AggregateException ae)
      {
        var flattenedMessage = String.Empty;
        var flattenedException = ae.Flatten();
        foreach (var exception in flattenedException.InnerExceptions)
        {
          flattenedMessage += exception.Message + " ";
        }

        throw new Exception(flattenedMessage);
      }
      catch (Exception e)
      {
        this.logController.Message(LoggerKeys.ExceptionKey, "Trying to create new feed: " + e.Message);
        throw e;
      }

      return null;
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
