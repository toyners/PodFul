
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Threading;
  using Library;
  using TestSupport;
  using Windows;
  using WPF.ViewModel;

  public class TileListViewModel
  {
    public TileListViewModel()
    {
      var feedImageFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Question-Mark.jpg");
      var podcasts1 = new[]
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
        Setup.createTestPodcast("Podcast2-A", "Description for Podcast2-A", "", DateTime.MinValue, 1l, DateTime.MinValue, "", "", ""),
        Setup.createTestPodcast("Podcast2-B", "Description for Podcast2-B", "", DateTime.MinValue, 1l, DateTime.MinValue, "", "", ""),
        Setup.createTestPodcast("Podcast2-C", "Description for Podcast2-C", "", DateTime.MinValue, 1l, DateTime.MinValue, "", "", ""),
      };

      var feed2 = Setup.createTestFullFeedFromParameters("Feed 2", "Description for Feed2", "", "", feedImageFilePath, "", "",
        DateTime.MinValue, DateTime.MinValue,
        true, true, true,
        podcasts2);

      this.Feeds = new ObservableCollection<FeedViewModel2>();
      this.Feeds.Add(new FeedViewModel2(feed1));
      this.Feeds.Add(new FeedViewModel2(feed2));
    }

    public Action<Int32, String> CompletedImageDownloadNotificationEvent
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public ObservableCollection<FeedViewModel2> Feeds { get; private set; }

    public Action<Int32, String> SkippedImageDownloadNotificationEvent
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public Action<Int32, String> StartImageDownloadNotificationEvent
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public Action<Int32> TotalImageDownloadsRequiredEvent
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public void AddFeed(AddFeedToken addFeedToken, CancellationToken cancelToken)
    {
      throw new NotImplementedException();
    }

    public void RemoveFeed(Int32 index)
    {
      throw new NotImplementedException();
    }
  }

  public class FeedViewModel2
  {
    public FeedViewModel2(Feed feed)
    {
      this.Title = feed.Title;
      this.Navigation = new PodcastPageNavigation(feed.Podcasts);
    }

    public String Title { get; private set; }
    public PodcastPageNavigation Navigation { get; set; }
  }

  public class PodcastPageNavigation
  {
    public PodcastPageNavigation(IList<Podcast> podcasts)
    {
      this.Pages = new ObservableCollection<PodcastPageViewModel2>();
      for (var index = 0; index < podcasts.Count; index++)
      {
        this.Pages.Add(new PodcastPageViewModel2(podcasts, index));
      }

      this.CurrentPage = this.Pages[0];
    }

    public ObservableCollection<PodcastPageViewModel2> Pages { get; set; }

    public PodcastPageViewModel2 CurrentPage { get; set; }

    public void MoveToNextPage()
    {
      throw new NotImplementedException();
    }
  }

  public class PodcastPageViewModel2
  {
    public PodcastPageViewModel2(IList<Podcast> podcasts, Int32 index)
    {
      this.Podcasts = new ObservableCollection<PodcastViewModel2>();
      this.Podcasts.Add(new PodcastViewModel2(podcasts[index]));
    }

    public ObservableCollection<PodcastViewModel2> Podcasts { get; set; }
  }

  public class PodcastViewModel2
  {
    public PodcastViewModel2(Podcast podcast)
    {
      this.Title = podcast.Title;
    }

    public String Title { get; private set; }
  }
}
