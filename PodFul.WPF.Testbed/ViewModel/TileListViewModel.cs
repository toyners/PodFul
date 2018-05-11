
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
      this.Feeds = new ObservableCollection<FeedViewModel>();
      foreach(var feed in feeds)
      {
        this.Feeds.Add(new FeedViewModel(feed));
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
