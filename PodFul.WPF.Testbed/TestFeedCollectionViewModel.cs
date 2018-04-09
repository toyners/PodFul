
namespace PodFul.WPF.Testbed
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using ViewModel;
  using Windows;

  public class TestFeedCollectionViewModel : IFeedCollectionViewModel
  {
    public ObservableCollection<IFeedViewModel> Feeds { get; private set; }
    public Action<Int32, String> CompletedImageDownloadNotificationEvent { get; set; }
    public Action<Int32, String> SkippedImageDownloadNotificationEvent { get; set; }
    public Action<Int32, String> StartImageDownloadNotificationEvent { get; set; }
    public Action<Int32> TotalImageDownloadsRequiredEvent { get; set; }

    public void AddFeed(AddFeedToken addFeedToken, CancellationToken cancelToken)
    {
      TotalImageDownloadsRequiredEvent?.Invoke(5);

      for (var index = 1; index <= 5; index++)
      {
        StartImageDownloadNotificationEvent?.Invoke(index, "Image " + index + " download started");
        Thread.Sleep(1000);
        CompletedImageDownloadNotificationEvent?.Invoke(index, "Image " + index + " download completed.");
      }
    }

    public void RemoveFeed(Int32 index)
    {
      throw new NotImplementedException();
    }
  }
}
