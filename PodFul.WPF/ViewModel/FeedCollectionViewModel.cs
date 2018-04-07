
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using PodFul.WPF.Windows;
  using Processing;

  public class FeedCollectionViewModel : IFeedCollectionViewModel
  {
    private IFeedProcessor feedProcessor;
    private IImageResolver imageResolver; 
    private Action<Int32, String> completedDownloadNotificationEvent;

    public ObservableCollection<IFeedViewModel> Feeds { get; private set; }

    public Action<Int32, String> CompletedDownloadNotificationEvent
    {
      get { return this.completedDownloadNotificationEvent; }
      set
      {
        // Handling the strict event handling on IImageResolver by hooking and unhooking the action 
        // based on the value passed in. Strip this out if the event handling on IImageResolver moves
        // to an action based model (as it should - multi-chaining of event handlers is not required)
        if (this.imageResolver == null)
        {
          return;
        }

        if (this.completedDownloadNotificationEvent != null)
        {
          this.imageResolver.CompletedDownloadNotificationEvent -= this.completedDownloadNotificationEvent;
        }

        if (value != null)
        {
          this.completedDownloadNotificationEvent = value;
          this.imageResolver.CompletedDownloadNotificationEvent += this.completedDownloadNotificationEvent;
        }
      }
    }

    public FeedCollectionViewModel(IFeedProcessor feedProcessor)
    {
      this.feedProcessor = feedProcessor;
      this.Feeds = new ObservableCollection<IFeedViewModel>();

      if (feedProcessor.Feeds != null && feedProcessor.Feeds.Count > 0)
      {
        foreach (var feed in feedProcessor.Feeds)
        {
          this.Feeds.Add(new FeedViewModel(feed));
        }
      }
    }

    public void AddFeed(AddFeedToken addFeedToken, CancellationToken cancelToken)
    {
      var feed = this.feedProcessor.AddFeed(addFeedToken.Directory, addFeedToken.Url, addFeedToken.DefaultPodcastImageFilePath, cancelToken);
      if (this.imageResolver != null)
      {
        this.imageResolver.ResolvePodcastImagesForFeed(feed, cancelToken);
      }

      this.Feeds.Add(new FeedViewModel(feed));
    }

    public void RemoveFeed(Int32 index)
    {
      throw new NotImplementedException();
    }
  }
}
