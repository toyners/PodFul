
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using Windows;

  public interface IFeedCollectionViewModel
  {
    ObservableCollection<IFeedViewModel> Feeds { get; }

    Action<Int32, String> CompletedDownloadNotificationEvent { get; set; }

    void AddFeed(AddFeedToken addFeedToken, CancellationToken cancelToken);

    void RemoveFeed(Int32 index);
  }
}
