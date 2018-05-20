
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Threading;
  using Library;
  using Miscellaneous;
  using Windows;

  public class TileListViewModel
  {
    public TileListViewModel(IList<Feed> feeds, IFeedCollection feedCollection)
    {
      this.Feeds = new ObservableCollection<FeedViewModel>();
      foreach(var feed in feeds)
      {
        this.Feeds.Add(new FeedViewModel(feed, feedCollection));
      }
    }

    public Action<Int32, String> CompletedImageDownloadNotificationEvent { get; set; }
    public ObservableCollection<FeedViewModel> Feeds { get; private set; }
    public Action<Int32, String> SkippedImageDownloadNotificationEvent { get; set; }
    public Action<Int32, String> StartImageDownloadNotificationEvent { get; set; }
    public Action<Int32> TotalImageDownloadsRequiredEvent { get; set; }

    public void AddFeed(AddFeedToken addFeedToken, CancellationToken cancelToken)
    {
      throw new NotImplementedException();
    }

    public void RemoveFeed(Int32 index)
    {
      throw new NotImplementedException();
    }
  }
}
