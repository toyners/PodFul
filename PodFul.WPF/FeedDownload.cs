
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Jabberwocky.Toolkit.String;
  using Library;

  public class FeedDownload : FeedProcessor
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

    public override void Process()
    {
      var title = "Downloading " + this.indexes.Count + " podcast".Pluralize((UInt32)this.indexes.Count);
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
