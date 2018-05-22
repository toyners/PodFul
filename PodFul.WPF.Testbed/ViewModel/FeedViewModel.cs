
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Threading;
  using System.Windows;
  using Jabberwocky.Toolkit.WPF;
  using Library;
  using Miscellaneous;
  using Processing;
  using WPF.Processing;

  public class FeedViewModel : NotifyPropertyChangedBase
  {
    private CancellationTokenSource cancellationTokenSource = null;
    private Feed feed;
    private ScanStates scanState;
    private Processing.IDownloadManager downloadManager;
    private IImageResolver imageResolver;
    private IFeedCollection feedCollection;

    public enum ScanStates
    {
      Idle,
      Waiting,
      Running,
      Completed,
      Cancelled,
      Failed
    }

    public FeedViewModel(Feed feed, IFeedCollection feedCollection)
    {
      this.feed = feed;
      this.PodcastNavigation = new PodcastPageNavigation();
      this.PodcastNavigation.SetPages(this.feed.Podcasts);
      this.JobNavigation = new JobPageNavigation();
      this.feedCollection = feedCollection;
    }

    public String Title { get { return this.feed.Title; } }
    public String Description { get { return this.feed.Description; } }
    public PodcastPageNavigation PodcastNavigation { get; set; }
    public JobPageNavigation JobNavigation { get; set; }
    public String FeedScanProgressMessage { get; private set; }
    public String FeedScanFailedMessage { get; private set; }
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

    public void Scan(IDownloadManagerFactory downloadManagerFactory)
    {
      try
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
          this.FeedScanState = ScanStates.Running;
        });

        this.UpdateScanProgressMessage("Processing ...");
        this.cancellationTokenSource = new CancellationTokenSource();
        var cancelToken = this.cancellationTokenSource.Token;
        var feedFilePath = Path.Combine(this.feed.Directory, "download.rss");
        var newFeed = FeedFunctions.UpdateFeed(feed, feedFilePath, cancelToken);

        // Creating the new feed may have taken a while - check for cancellation before processing podcasts.
        cancelToken.ThrowIfCancellationRequested();

        this.UpdateScanProgressMessage("Searching for new podcasts ...");
        var podcastIndexes = this.BuildNewPodcastIndexList(feed, newFeed);

        if (podcastIndexes.Count == 0)
        {
          this.UpdateScanProgressMessage("Saving feed (No podcasts found).");
          newFeed = Feed.SetUpdatedDate(DateTime.Now, newFeed);
          this.feedCollection.UpdateFeedContent(newFeed);
          return;
        }

        var downloadConfirmation = (!newFeed.CompleteDownloadsOnScan ? DownloadConfirmationStatus.SkipDownloading : DownloadConfirmationStatus.ContinueDownloading);
        if (downloadConfirmation != DownloadConfirmationStatus.SkipDownloading)
        {
          if (podcastIndexes.Count >= newFeed.ConfirmDownloadThreshold)
          {
            IPodcastDownloadConfirmer podcastDownloadConfirmer = new PodcastDownloadConfirmer();
            downloadConfirmation = podcastDownloadConfirmer.ConfirmPodcastsForDownload(feed, newFeed, podcastIndexes);

            if (downloadConfirmation == DownloadConfirmationStatus.CancelScanning)
            {
              this.UpdateScanProgressMessage(podcastIndexes.Count + " podcasts found (Scan cancelled).");
              this.cancellationTokenSource.Cancel();
            }
          }
        }

        if (this.imageResolver != null)
        {
          foreach (var podcastIndex in podcastIndexes)
          {
            this.imageResolver.ResolvePodcastImage(newFeed.Podcasts[podcastIndex]);
          }
        }

        this.UpdateScanProgressMessage("Saving feed (" + podcastIndexes.Count + " podcasts found).");
        // Now update the feed to storage for real.
        newFeed = Feed.SetUpdatedDate(DateTime.Now, newFeed);
        this.feedCollection.UpdateFeedContent(newFeed);

        if (downloadConfirmation == DownloadConfirmationStatus.SkipDownloading)
        {
          return;
        }

        this.UpdateScanProgressMessage("Downloading " + podcastIndexes.Count + " podcasts ...");

        podcastIndexes.Reverse();
        List<JobViewModel> jobs = new List<JobViewModel>(podcastIndexes.Count);
        foreach (var podcastIndex in podcastIndexes)
        {
          jobs.Add(new JobViewModel(newFeed.Podcasts[podcastIndex], newFeed));
        }

        this.JobNavigation.SetPages(jobs);

        this.downloadManager = downloadManagerFactory.Create();
        var jobFinishedCount = 0;
        this.downloadManager.JobFinishedEvent = j =>
        {
          Application.Current.Dispatcher.Invoke(() =>
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

        this.UpdateScanProgressMessage("Done");

        Application.Current.Dispatcher.Invoke(() =>
        {
          this.FeedScanState = ScanStates.Completed;
        });
      }
      catch (OperationCanceledException oce)
      {
        this.HandleScanCancelled();
      }
      catch (Exception e)
      {
        this.HandleScanError(e);
      }
    }

    private void HandleScanCancelled()
    {
      throw new NotImplementedException();
    }

    private void HandleScanError(Exception e)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanFailedMessage = e.Message;
        this.FeedScanState = ScanStates.Failed;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("FeedScanFailedMessage"));
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
