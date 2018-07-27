
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using PodFul.WPF.Processing;
  using PodFul.WPF.ViewModel.TileView;

  public interface IFeedCollectionViewModel
  {
    ObservableCollection<FeedViewModel> Feeds { get; }

    Action<Int32, String> CompletedImageDownloadNotificationEvent { get; set; }
    Action<Int32, String> SkippedImageDownloadNotificationEvent { get; set; }
    Action<Int32, String> StartImageDownloadNotificationEvent { get; set; }
    Action<Int32> TotalImageDownloadsRequiredEvent { get; set; }

    void AddFeed(AddFeedToken addFeedToken, CancellationToken cancelToken);

    void RemoveFeed(FeedViewModel feedViewModel);
  }
}
