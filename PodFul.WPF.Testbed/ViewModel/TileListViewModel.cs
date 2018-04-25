
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
  using Windows;

  public class TileListViewModel
  {
    public TileListViewModel(IList<Feed> feeds)
    {
      this.Feeds = new ObservableCollection<FeedViewModel2>();
      foreach(var feed in feeds)
      {
        this.Feeds.Add(new FeedViewModel2(feed));
      }
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

  public class FeedViewModel2 : NotifyPropertyChangedBase
  {
    public FeedViewModel2(Feed feed)
    {
      this.Title = feed.Title;
      this.Description = feed.Description;
      this.PodcastNavigation = new PodcastPageNavigation(feed.Podcasts);

      var jobs = new List<TestDownloadJob>();
      for (var number = 1; number <= feed.Podcasts.Length; number++)
      {
        jobs.Add(new TestDownloadJob { Title = "DownloadJob " + number });
      }

      this.JobNavigation = new JobPageNavigation(jobs, 2);
    }

    public String Title { get; private set; }
    public String Description { get; private set; }
    public PodcastPageNavigation PodcastNavigation { get; set; }
    public JobPageNavigation JobNavigation { get; set; }
    public Boolean IsScanning { get; private set; }

    public void Scan()
    {
      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.IsScanning = true;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("IsScanning"));
      });

      Thread.Sleep(2000);

      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.IsScanning = false;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("IsScanning"));
      });
    }
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

  public class JobPageNavigation : NotifyPropertyChangedBase
  {
    private Int32 pageNumber = 1;
    private JobPageViewModel currentPage;
    private ObservableCollection<JobPageViewModel> pages;

    public JobPageNavigation(IList<TestDownloadJob> jobs, Int32 jobCount = 1)
    {
      this.pages = new ObservableCollection<JobPageViewModel>();
      for (var index = 0; index < jobs.Count; index += jobCount)
      {
        var lastIndex = index + jobCount - 1;
        if (lastIndex >= jobs.Count)
        {
          lastIndex = jobs.Count - 1;
        }

        this.pages.Add(new JobPageViewModel(jobs, index, lastIndex));
      }

      this.currentPage = this.pages[0];
    }

    public JobPageViewModel CurrentPage { get { return this.currentPage; } }

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

  public class JobPageViewModel
  {
    public JobPageViewModel(IList<TestDownloadJob> jobs, Int32 firstJobIndex, Int32 lastJobIndex)
    {
      this.Jobs = new List<JobViewModel>(lastJobIndex - firstJobIndex + 1);
      while (firstJobIndex <= lastJobIndex)
      {
        this.Jobs.Add(new JobViewModel(jobs[firstJobIndex++]));
      }
    }

    public List<JobViewModel> Jobs { get; private set; }
  }

  public class JobViewModel
  {
    public JobViewModel(TestDownloadJob job)
    {
      this.Title = job.Title;
    }

    public String Title { get; private set; }
  }

  public class TestDownloadJob
  {
    public String Title;
  }
}
