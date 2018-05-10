
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
      this.PodcastNavigation = new PodcastPageNavigation();
      this.PodcastNavigation.SetPages(this.feed.Podcasts, 2, (podcasts, firstIndex, lastIndex) => { return new PodcastPageViewModel2(podcasts, firstIndex, lastIndex); });
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

      var jobs = new List<JobViewModel>();
      foreach (var podcast in this.feed.Podcasts)
      {
        jobs.Add(new JobViewModel(podcast, feed));
      }

      this.UpdateScanProgressMessage(jobs.Count + " podcasts found.");

      this.UpdateScanProgressMessage("Updating feed");
      Thread.Sleep(2000);

      this.UpdateScanProgressMessage("Downloading podcasts");
      this.JobNavigation.SetPages(jobs, 2, (j, f, l) => { return new JobPageViewModel(j, f, l); });

      var downloadManager = downloadManagerFactory.Create();
      downloadManager.AddJobs(jobs);
      downloadManager.StartWaitingJobs();

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

  public class JobPageViewModel : NotifyPropertyChangedBase
  {
    public JobPageViewModel(IList<JobViewModel> jobs, Int32 firstJobIndex, Int32 lastJobIndex)
    {
      this.Jobs = new List<JobViewModel>();
      while (firstJobIndex <= lastJobIndex)
      {
        this.Jobs.Add(jobs[firstJobIndex++]);
      }
    }

    public List<JobViewModel> Jobs { get; private set; }
  }
}
