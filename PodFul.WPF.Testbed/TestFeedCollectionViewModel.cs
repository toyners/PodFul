
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using ViewModel;
  using Windows;

  public class TestFeedCollectionViewModel : IFeedCollectionViewModel
  {
    public TestFeedCollectionViewModel()
    {
      this.Feeds = new ObservableCollection<IFeedViewModel>();
    }

    public ObservableCollection<IFeedViewModel> Feeds { get; private set; }
    public Action<Int32, String> CompletedImageDownloadNotificationEvent { get; set; }
    public Action<Int32, String> SkippedImageDownloadNotificationEvent { get; set; }
    public Action<Int32, String> StartImageDownloadNotificationEvent { get; set; }
    public Action<Int32> TotalImageDownloadsRequiredEvent { get; set; }
    
    public void AddFeed(AddFeedToken addFeedToken, CancellationToken cancelToken)
    {
      this.Feeds.Add(new FeedViewModel(new Library.Feed("title", "", "", "", "", "", "", new Library.Podcast[0], DateTime.MinValue, DateTime.MinValue, true, true, true, 3u)));

      TotalImageDownloadsRequiredEvent?.Invoke(5);

      for (var index = 1; index <= 5; index++)
      {
        StartImageDownloadNotificationEvent?.Invoke(index, "Image " + index);
        Thread.Sleep(1000);
        CompletedImageDownloadNotificationEvent?.Invoke(index, "Image " + index);
      }
    }

    public void RemoveFeed(Int32 index)
    {
      throw new NotImplementedException();
    }
  }
}
