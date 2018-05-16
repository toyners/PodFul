
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Threading;
  using TestSupport;
  using Windows;
  using WPF.ViewModel;

  public class MultipleFeedCollectionViewModel : IFeedCollectionViewModel
  {
    public MultipleFeedCollectionViewModel()
    {
      var feedImageFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Question-Mark.jpg");
      var podcasts1 = new []
      {
        Setup.createTestPodcast("Podcast1-A", "Description for Podcast1-A", "", DateTime.MinValue, 1L, DateTime.MinValue, "", "", ""),
        Setup.createTestPodcast("Podcast1-B", "Description for Podcast1-B", "", DateTime.MinValue, 1L, DateTime.MinValue, "", "", ""),
        Setup.createTestPodcast("Podcast1-C", "Description for Podcast1-C", "", DateTime.MinValue, 1L, DateTime.MinValue, "", "", ""),
      };

      var feed1 = Setup.createTestFullFeedFromParameters("Feed 1", "Description for Feed1", "", "", feedImageFilePath, "", "",
        DateTime.MinValue, DateTime.MinValue,
        true, true, true,
        podcasts1);

      var podcasts2 = new[]
      {
        Setup.createTestPodcast("Podcast2-A", "Description for Podcast2-A", "", DateTime.MinValue, 1L, DateTime.MinValue, "", "", ""),
        Setup.createTestPodcast("Podcast2-B", "Description for Podcast2-B", "", DateTime.MinValue, 1L, DateTime.MinValue, "", "", ""),
        Setup.createTestPodcast("Podcast2-C", "Description for Podcast2-C", "", DateTime.MinValue, 1L, DateTime.MinValue, "", "", ""),
      };

      var feed2 = Setup.createTestFullFeedFromParameters("Feed 2", "Description for Feed2", "", "", feedImageFilePath, "", "",
        DateTime.MinValue, DateTime.MinValue,
        true, true, true,
        podcasts2);

      this.Feeds = new ObservableCollection<TreeViewItemViewModelBad>();
      this.Feeds.Add(new WPF.ViewModel.FeedViewModelBad(feed1));
      this.Feeds.Add(new WPF.ViewModel.FeedViewModelBad(feed2));
    }

    public Action<Int32, String> CompletedImageDownloadNotificationEvent { get; set; }
    public ObservableCollection<TreeViewItemViewModelBad> Feeds { get; private set; }
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
