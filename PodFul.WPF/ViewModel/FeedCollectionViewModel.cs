
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using PodFul.WPF.Windows;
  using Processing;

  public class FeedCollectionViewModel : IFeedCollectionViewModel
  {
    private enum ImageEvents
    {
      Completed,
      Skipped,
      Start,
    }

    #region Fields
    private IFeedProcessor feedProcessor;
    private IImageResolver imageResolver; 
    private Action<Int32, String> completedImageDownloadNotificationEvent;
    private Action<Int32, String> skippedImageDownloadNotificationEvent;
    private Action<Int32, String> startImageDownloadNotificationEvent;
    private Action<Int32> totalImageDownloadsRequiredEvent;
    #endregion

    #region Properties
    public ObservableCollection<IFeedViewModel> Feeds { get; private set; }

    public Action<Int32, String> CompletedImageDownloadNotificationEvent
    {
      get { return this.completedImageDownloadNotificationEvent; }
      set { this.SetImageEventHandler(this.completedImageDownloadNotificationEvent, ImageEvents.Completed, value); }
    }

    public Action<Int32, String> SkippedImageDownloadNotificationEvent
    {
      get { return this.skippedImageDownloadNotificationEvent; }
      set { this.SetImageEventHandler(this.skippedImageDownloadNotificationEvent, ImageEvents.Skipped, value); }
    }

    public Action<Int32, String> StartImageDownloadNotificationEvent
    {
      get { return this.startImageDownloadNotificationEvent; }
      set { this.SetImageEventHandler(this.startImageDownloadNotificationEvent, ImageEvents.Start, value); }
    }

    public Action<Int32> TotalImageDownloadsRequiredEvent
    {
      get { return this.totalImageDownloadsRequiredEvent; }
      set
      {
        // Handling the strict event handling on IImageResolver by hooking and unhooking the action 
        // based on the value passed in. Strip this out if the event handling in IImageResolver moves
        // to an action based model (as it should - multi-chaining of event handlers is not required)
        if (this.imageResolver == null)
        {
          return;
        }

        if (this.totalImageDownloadsRequiredEvent != null)
        {
          this.imageResolver.TotalDownloadsRequiredEvent -= this.totalImageDownloadsRequiredEvent;
        }

        if (value != null)
        {
          this.totalImageDownloadsRequiredEvent = value;
          this.imageResolver.TotalDownloadsRequiredEvent += value;
        }
      }
    }
    #endregion

    #region Methods
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

    private void SetImageEventHandler(Action<Int32, String> currentEvent, ImageEvents eventType, Action<Int32, String> newEvent)
    {
      // Handling the strict event handling on IImageResolver by hooking and unhooking the action 
      // based on the value passed in. Strip this out if the event handling in IImageResolver moves
      // to an action based model (as it should - multi-chaining of event handlers is not required)
      if (this.imageResolver == null)
      {
        return;
      }

      if (currentEvent != null)
      {
        switch (eventType)
        {
          case ImageEvents.Completed: this.imageResolver.CompletedDownloadNotificationEvent -= currentEvent; break;
          case ImageEvents.Skipped: this.imageResolver.SkippedDownloadNotificationEvent -= currentEvent; break;
          case ImageEvents.Start: this.imageResolver.StartDownloadNotificationEvent -= currentEvent; break;
        }
      }

      if (newEvent != null)
      {
        currentEvent = newEvent;
        switch (eventType)
        {
          case ImageEvents.Completed: this.imageResolver.CompletedDownloadNotificationEvent += newEvent; break;
          case ImageEvents.Skipped: this.imageResolver.SkippedDownloadNotificationEvent += newEvent; break;
          case ImageEvents.Start: this.imageResolver.StartDownloadNotificationEvent += newEvent; break;
        }
      }
    }
    #endregion
  }
}
