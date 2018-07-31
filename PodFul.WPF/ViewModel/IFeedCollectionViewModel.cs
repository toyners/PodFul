
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using PodFul.Library;
  using PodFul.WPF.ViewModel.TileView;

  public interface IFeedCollectionViewModel
  {
    ObservableCollection<FeedViewModel> Feeds { get; }

    void AddFeed(Feed feed);
    void RemoveFeed(FeedViewModel feedViewModel);
  }
}
