
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Threading;
  using Jabberwocky.Toolkit.WPF;
  using Library;
  using Processing;
  using WPF.Processing;

  public class FeedViewModel : NotifyPropertyChangedBase
  {
    private Feed feed;
    private ScanStates scanState;
    private Processing.IDownloadManager downloadManager;

    public enum ScanStates
    {
      Idle,
      Waiting,
      Running,
      Completed,
      Cancelled,
      Failed
    }

    public FeedViewModel(Feed feed)
    {
      this.feed = feed;
      this.PodcastNavigation = new PodcastPageNavigation();
      this.PodcastNavigation.SetPages(this.feed.Podcasts);
      this.JobNavigation = new JobPageNavigation();
    }

    public String Title { get { return this.feed.Title; } }
    public String Description { get { return this.feed.Description; } }
    public PodcastPageNavigation PodcastNavigation { get; set; }
    public JobPageNavigation JobNavigation { get; set; }
    public String FeedScanProgressMessage { get; private set; }
    public ScanStates FeedScanState
    {
      get { return this.scanState; }
      private set
      {
        if (this.scanState == value)
        {
          return;
        }

        this.SetField(ref this.scanState, value, "FeedScanState");
      }
    }

    public void CancelScan()
    {
      if (this.scanState == ScanStates.Completed)
      {
        return;
      }

      try
      {
        if (this.downloadManager != null)
        {
          this.downloadManager.CancelJobs();
          return;
        }
      }
      finally
      {
        this.scanState = ScanStates.Cancelled;
        this.UpdateScanProgressMessage("Cancelled");
      }
    }

    public void InitialiseForScan()
    {
      this.FeedScanState = ScanStates.Waiting;
    }

    public void Reset()
    {
      this.JobNavigation.Reset();
      this.UpdateScanProgressMessage(String.Empty);
      this.FeedScanState = ScanStates.Idle;
    }

    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public void Scan(IDownloadManagerFactory downloadManagerFactory)
    {
      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanState = ScanStates.Running;
      });

      this.UpdateScanProgressMessage("Updating feed");
      this.cancellationTokenSource = new CancellationTokenSource();
      var cancelToken = this.cancellationTokenSource.Token;
      var feedFilePath = Path.Combine(this.feed.Directory, "download.rss");
      var newFeed = FeedFunctions.UpdateFeed(feed, feedFilePath, cancelToken);

      // Creating the new feed may have taken a while - check for cancellation before processing podcasts.
      cancelToken.ThrowIfCancellationRequested();

      this.UpdateScanProgressMessage("Searching for new podcasts ... ");
      var podcastIndexes = this.BuildNewPodcastIndexList(feed, newFeed);
      var feedHasNewPodcasts = (podcastIndexes.Count > 0);

      var downloadConfirmation = (!newFeed.CompleteDownloadsOnScan ? DownloadConfirmationStatus.SkipDownloading : DownloadConfirmationStatus.ContinueDownloading);
      if (feedHasNewPodcasts && downloadConfirmation != DownloadConfirmationStatus.SkipDownloading)
      {
        /*this.LogNewPodcastsFromFeed(this.logController.GetLogger<FileLogger>(LoggerKeys.InfoKey), newFeed, podcastIndexes);

        podcastDownloadConfirmer.ConfirmDownloadThreshold = feed.ConfirmDownloadThreshold;
        downloadConfirmation = podcastDownloadConfirmer.ConfirmPodcastsForDownload(feed, newFeed, podcastIndexes);
        if (downloadConfirmation == DownloadConfirmationStatus.CancelScanning)
        {
          var feedMessage = podcastIndexes.Count + " podcasts found";
          var logMessage = feedMessage + " (Scan cancelled).";
          this.logController.Message(LoggerKeys.InfoKey, logMessage).Message(LoggerKeys.UiKey, feedMessage + "\r\n");
          scanReport.Message(feedMessage + " for \"" + feed.Title + "\".");
          this.cancellationTokenSource.Cancel();
        }*/
      }

      List<JobViewModel> jobs = null;
      var jobCount = "No";
      if (this.feed.Podcasts.Length > 0)
      {
        jobs = new List<JobViewModel>();
        foreach (var podcast in this.feed.Podcasts)
        {
          jobs.Add(new JobViewModel(podcast, feed));
        }

        jobCount = jobs.Count.ToString();
      }

      this.UpdateScanProgressMessage("Saving feed (" + jobCount + " podcasts found).");
      Thread.Sleep(1000);

      if (jobs != null)
      {
        this.UpdateScanProgressMessage("Downloading " + jobs.Count + " podcasts");
        this.JobNavigation.SetPages(jobs);

        this.downloadManager = downloadManagerFactory.Create();
        var jobFinishedCount = 0;
        this.downloadManager.JobFinishedEvent = j =>
        {
          System.Windows.Application.Current.Dispatcher.Invoke(() =>
          {
            jobFinishedCount++;
            if (jobFinishedCount % 2 == 0 && this.JobNavigation.CanMoveForward)
            {
              this.JobNavigation.PageNumber += 1;
            }
          });
        };

        downloadManager.AddJobs(jobs);
        downloadManager.StartWaitingJobs();
      }

      this.UpdateScanProgressMessage("Done");

      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanState = ScanStates.Completed;
      });
    }

    private List<Int32> BuildNewPodcastIndexList(Feed oldFeed, Feed newFeed)
    {
      var podcastIndexes = new List<Int32>();
      if (oldFeed.Podcasts.Length == 0)
      {
        for (Int32 i = 0; i < newFeed.Podcasts.Length; i++)
        {
          podcastIndexes.Add(i);
        }
      }
      else
      {
        Int32 podcastIndex = 0;
        var firstPodcast = oldFeed.Podcasts[0];
        while (podcastIndex < newFeed.Podcasts.Length && !newFeed.Podcasts[podcastIndex].Equals(firstPodcast))
        {
          podcastIndexes.Add(podcastIndex);
          podcastIndex++;
        }
      }

      return podcastIndexes;
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
}
