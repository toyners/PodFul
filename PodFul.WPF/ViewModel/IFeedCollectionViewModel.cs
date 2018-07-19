﻿
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using PodFul.WPF.Processing;

  public interface IFeedCollectionViewModel
  {
    ObservableCollection<TreeViewItemViewModelBad> Feeds { get; }

    Action<Int32, String> CompletedImageDownloadNotificationEvent { get; set; }
    Action<Int32, String> SkippedImageDownloadNotificationEvent { get; set; }
    Action<Int32, String> StartImageDownloadNotificationEvent { get; set; }
    Action<Int32> TotalImageDownloadsRequiredEvent { get; set; }

    void AddFeed(AddFeedToken addFeedToken, CancellationToken cancelToken);

    void RemoveFeed(Int32 index);
  }
}
