
namespace PodFul.WPF.ViewModel.TileView
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using Miscellaneous;
  using PodFul.Library;
  using PodFul.WPF.Processing;
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

    public Action<Int32, String> CompletedImageDownloadNotificationEvent { get; set; }
    public ObservableCollection<FeedViewModel> Feeds { get; private set; }
    public Action<Int32, String> SkippedImageDownloadNotificationEvent { get; set; }
    public Action<Int32, String> StartImageDownloadNotificationEvent { get; set; }
    public Action<Int32> TotalImageDownloadsRequiredEvent { get; set; }

    public void AddFeed(AddFeedToken addFeedToken, CancellationToken cancelToken)
    {
      throw new NotImplementedException();
    }

    public void RemoveFeed(FeedViewModel feedViewModel)
    {
      var index = this.Feeds.IndexOf(feedViewModel);
      this.feedCollection.RemoveFeed(index);
      this.Feeds.RemoveAt(index);
    }
  }
}
