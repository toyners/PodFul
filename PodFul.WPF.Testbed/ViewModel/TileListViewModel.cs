
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Threading;
  using Jabberwocky.Toolkit.WPF;
  using Library;
  using Processing;
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

    public Action<Int32, String> CompletedImageDownloadNotificationEvent { get; set; }
    public ObservableCollection<FeedViewModel2> Feeds { get; private set; }
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

  public class FeedViewModel2 : NotifyPropertyChangedBase
  {
    private Feed feed;

    public enum ScanStates
    {
      Idle,
      Running,
      Completed
    }

    public FeedViewModel2(Feed feed)
    {
      this.feed = feed;
      this.PodcastNavigation = new PodcastPageNavigation(feed.Podcasts);
      this.JobNavigation = new JobPageNavigation();
    }

    public String Title { get { return this.feed.Title; } }
    public String Description { get { return this.feed.Description; } }
    public PodcastPageNavigation PodcastNavigation { get; set; }
    public JobPageNavigation JobNavigation { get; set; }
    public String FeedScanProgressMessage { get; private set; }
    public ScanStates FeedScanState { get; private set; }

    public void Scan(IDownloadManagerFactory downloadManagerFactory)
    {
      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanState = ScanStates.Running;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("FeedScanState"));
      });

      this.UpdateScanProgressMessage("Scanning feed");
      Thread.Sleep(2000);

      this.UpdateScanProgressMessage("Searching for new podcasts ... ");
      Thread.Sleep(2000);

      var newPodcastCount = 1;
      this.UpdateScanProgressMessage(newPodcastCount + " podcasts found.");

      var jobs = new List<DownloadJob>();
      for (var number = 1; number <= newPodcastCount; number++)
      {
        jobs.Add(new DownloadJob(null, feed, null));
      }

      this.UpdateScanProgressMessage("Updating feed");
      Thread.Sleep(2000);

      this.UpdateScanProgressMessage("Downloading podcasts");
      this.JobNavigation.AddJobs(jobs, 2);

      var downloadManager = downloadManagerFactory.Create();
      downloadManager.AddJobs(jobs);

      this.UpdateScanProgressMessage("Done");

      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanState = ScanStates.Completed;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("FeedScanState"));
      });
    }

    private void UpdateScanProgressMessage(String progressMessage)
    {
      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanProgressMessage = progressMessage;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("FeedScanProgressMessage"));
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
    private Int32 pageNumber;
    private JobPageViewModel currentPage;
    //private ObservableCollection<JobPageViewModel> pages = new ObservableCollection<JobPageViewModel>();
    private List<JobPageViewModel> pages = new List<JobPageViewModel>();
    private Boolean hasJobs;

    public JobPageViewModel CurrentPage { get { return this.currentPage; } }

    public Int32 TotalPages { get { return this.pages.Count; } }

    public Boolean CanMoveBack { get { return this.pageNumber > 1; } }

    public Boolean CanMoveForward { get { return this.pageNumber < this.TotalPages; } }

    public Boolean HasJobs
    {
      get { return this.hasJobs; }
      private set
      {
        this.SetField(ref this.hasJobs, value, "HasJobs");
      }
    }

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
        
        this.TryInvokePropertyChanged(
           new PropertyChangedEventArgs("CurrentPage"),
           new PropertyChangedEventArgs("PageNumber"),
           new PropertyChangedEventArgs("CanMoveBack"),
           new PropertyChangedEventArgs("CanMoveForward")
        );
      }
    }

    public void AddJobs(IList<DownloadJob> jobs, Int32 jobCount)
    {
      this.pages.Clear();
      this.pageNumber = 0;
      for (var index = 0; index < jobs.Count; index += jobCount)
      {
        var lastIndex = index + jobCount - 1;
        if (lastIndex >= jobs.Count)
        {
          lastIndex = jobs.Count - 1;
        }

        this.pages.Add(new JobPageViewModel(jobs, index, lastIndex));
      }

      this.PageNumber = 1;
      this.HasJobs = true;
      this.TryInvokePropertyChanged(
        new PropertyChangedEventArgs("TotalPages"));
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

  public class JobPageViewModel : NotifyPropertyChangedBase
  {
    public JobPageViewModel(IList<DownloadJob> jobs, Int32 firstJobIndex, Int32 lastJobIndex)
    {
      this.Jobs = new List<DownloadJob>();
      while (firstJobIndex <= lastJobIndex)
      {
        this.Jobs.Add(jobs[firstJobIndex++]);
      }
    }

    public List<DownloadJob> Jobs { get; private set; }
  }

  public class JobViewModel : NotifyPropertyChangedBase
  {
    private Int32 progressValue;

    public JobViewModel(TestDownloadJob job)
    {
      this.Title = job.Title;
    }

    public String Title { get; private set; }
    public String Description { get { return ""; } }
    public Int32 ProgressValue
    {
      get { return this.progressValue; }
      set
      {
        this.SetField(ref this.progressValue, value, "ProgressValue");
      }
    }
    /*public Boolean UseMarqueProgressStyle { get { return false; } }
    public String StatusMessage { get { return ""; } }
    public String StatusColor { get { return ""; } }
    public String StatusWeight { get { return ""; } }
    public String ExceptionMessage { get { return ""; } }

    public String ProgressMajorSize { get { return ""; } }
    public String ProgressMinorSize { get { return ""; } }
    public String ProgressUnit { get { return ""; } }*/
  }

  public class TestDownloadJob
  {
    public String Title;
  }

  public class DownloadJobProcessor
  {
    public void Process(IList<JobViewModel> jobs)
    {
      foreach (var job in jobs)
      {
        job.ProgressValue = 20;
        Thread.Sleep(500);

        job.ProgressValue = 40;
        Thread.Sleep(500);

        job.ProgressValue = 60;
        Thread.Sleep(500);

        job.ProgressValue = 80;
        Thread.Sleep(500);

        job.ProgressValue = 100;
        Thread.Sleep(500);
      }
    }
  }
}
