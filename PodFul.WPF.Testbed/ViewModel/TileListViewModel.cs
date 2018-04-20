
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.IO;
  using System.Threading;
  using Jabberwocky.Toolkit.WPF;
  using Library;
  using TestSupport;
  using Windows;

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

  public class PodcastPageNavigation : NotifyPropertyChangedBase
  {
    private Int32 pageNumber = 1;
    private PodcastPageViewModel2 currentPage;
    private ObservableCollection<PodcastPageViewModel2> pages;

    public PodcastPageNavigation(IList<Podcast> podcasts, Int32 podcastCount = 1)
    {
      this.pages = new ObservableCollection<PodcastPageViewModel2>();
      for (var index = 0; index < podcasts.Count; index += podcastCount)
      {
        this.pages.Add(new PodcastPageViewModel2(podcasts, index, index + podcastCount - 1));
      }

      this.currentPage = this.pages[0];
    }

    public PodcastPageViewModel2 CurrentPage { get { return this.currentPage; } }

    public Int32 TotalPages { get { return this.pages.Count; } }

    public Boolean CanMoveBack { get { return this.pageNumber > 1; } }

    public Boolean CanMoveForward { get { return this.pageNumber < this.TotalPages; } }

    public Int32 PageNumber
    {
      get { return this.pageNumber; }
      set
      {
        if (this.pageNumber == value)
        {
          return;
        }

        this.pageNumber = value;
        this.currentPage = this.pages[this.pageNumber - 1];
        this.TryInvokePropertyChanged(new[]
        {
           new PropertyChangedEventArgs("CurrentPage"),
           new PropertyChangedEventArgs("PageNumber"),
           new PropertyChangedEventArgs("CanMoveBack"),
           new PropertyChangedEventArgs("CanMoveForward"),
        });
      }
    }

    public void MoveToNextPage()
    {
      this.PageNumber++;
    }

    public void MoveToFirstPage()
    {
      this.PageNumber = 1;
    }

    public void MoveToPreviousPage()
    {
      this.PageNumber--;
    }

    public void MoveToLastPage()
    {
      this.PageNumber = this.pages.Count;
    }
  }

  public class PodcastPageViewModel2
  {
    public PodcastPageViewModel2(IList<Podcast> podcasts, Int32 firstPodcastIndex, Int32 lastPodcastIndex)
    {
      this.Podcasts = new List<PodcastViewModel2>(lastPodcastIndex - firstPodcastIndex + 1);
      while (firstPodcastIndex <= lastPodcastIndex)
      {
        this.Podcasts.Add(new PodcastViewModel2(podcasts[firstPodcastIndex++]));
      }
    }

    public List<PodcastViewModel2> Podcasts { get; private set; }
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
