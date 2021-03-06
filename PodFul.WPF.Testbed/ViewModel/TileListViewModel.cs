﻿
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using Miscellaneous;
  using PodFul.WPF.Processing;
  using WPF.Windows;

  public class TileListViewModel
  {
    private IFileDownloadProxyFactory fileDownloadProxyFactory;

    public TileListViewModel(IFeedCollection feedCollection, IFileDownloadProxyFactory fileDownloadProxyFactory)
    {
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

    public void RemoveFeed(Int32 index)
    {
      throw new NotImplementedException();
    }
  }
}
