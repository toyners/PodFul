
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Library;

  public class FeedDownload : FeedProcessor, IFeedProcessor
  {
    private Feed feed;

    public FeedDownload(
      FeedCollection feedCollection,
      Feed feed,
      Queue<Int32> podcastIndexes,
      IImageResolver imageResolver,
      IFileDeliverer fileDeliverer,
      ILogger log) : base(feedCollection, podcastIndexes, imageResolver, fileDeliverer, log)
    {
      this.feed = feed;
    }

    public void Process()
    {
      var title = "Downloading " + this.indexes.Count + " podcast" + (this.indexes.Count != 1 ? "s" : String.Empty);
      this.SetWindowTitleEvent?.Invoke(title);

      var cancelToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(cancelToken);

      Task task = Task.Factory.StartNew(() =>
      {
        this.SetCancelButtonStateEvent?.Invoke(true);
        if (podcastDownload.Download(this.feed.Directory, this.feed.Podcasts, this.indexes))
        {
          feedCollection.UpdateFeed(this.feed);
        }

        this.SetCancelButtonStateEvent?.Invoke(false);

      }, cancelToken);
    }

    protected override void OnSuccessfulDownload(Podcast podcast, String filePath)
    {
      this.log.Message("Completed.");
      this.fileDeliverer.Deliver(podcast, filePath);
      this.log.Message(String.Empty); //Blank line to break up text flow
    }
  }
}
