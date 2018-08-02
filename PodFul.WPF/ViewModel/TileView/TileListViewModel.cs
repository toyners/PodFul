
namespace PodFul.WPF.ViewModel.TileView
{
  using System.Collections.ObjectModel;
  using Miscellaneous;
  using PodFul.Library;
  using PodFul.WPF.Processing.TileView;

  public class TileListViewModel : IFeedCollectionViewModel
  {
    private readonly IFileDownloadProxyFactory fileDownloadProxyFactory;
    private readonly IFeedCollection feedCollection;

    public TileListViewModel(IFeedCollection feedCollection, IFileDownloadProxyFactory fileDownloadProxyFactory)
    {
      this.feedCollection = feedCollection;
      this.fileDownloadProxyFactory = fileDownloadProxyFactory;
      this.Feeds = new ObservableCollection<FeedViewModel>();
      for (var i = 0; i < feedCollection.Count; i++)
      {
        this.Feeds.Add(new FeedViewModel(i, feedCollection, null, this.fileDownloadProxyFactory));
      }
    }

    public ObservableCollection<FeedViewModel> Feeds { get; private set; }

    public void AddFeed(Feed feed)
    {
      this.feedCollection.AddFeed(feed);
      var feedViewModel = new FeedViewModel(this.feedCollection.Count - 1, this.feedCollection, null, this.fileDownloadProxyFactory);
      this.Feeds.Add(feedViewModel);
    }

    public void RemoveFeed(FeedViewModel feedViewModel)
    {
      var index = this.Feeds.IndexOf(feedViewModel);
      this.feedCollection.RemoveFeed(index);
      this.Feeds.RemoveAt(index);
    }
  }
}
