
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
    #region Fields
    private static PropertyChangedEventArgs FeedScanProgressMessageArgs = new PropertyChangedEventArgs("FeedScanProgressMessage");
    private CancellationTokenSource cancellationTokenSource = null;
    private Processing.IDownloadManager downloadManager;
    private Feed feed;
    private IFeedCollection feedCollection;
    private IImageResolver imageResolver;
    private ProcessingStatus scanState;
    #endregion

    #region Construction
    public FeedViewModel(Feed feed, IFeedCollection feedCollection)
    {
      this.feed = feed;
      this.PodcastNavigation = new PodcastPageNavigation();
      this.PodcastNavigation.SetPages(this.feed.Podcasts);
      this.JobNavigation = new JobPageNavigation();
      this.feedCollection = feedCollection;
    }
    #endregion

    #region Properties
    public String Title { get { return this.feed.Title; } }
    public String Description { get { return this.feed.Description; } }
    public PodcastPageNavigation PodcastNavigation { get; set; }
    public JobPageNavigation JobNavigation { get; set; }
    public String FeedScanProgressMessage { get; private set; }
    public String FeedScanFailedMessage { get; private set; }
    public ProcessingStatus FeedScanState
    {
      get { return this.scanState; }
      private set
      {
        if (this.scanState == value)
        {
          return;
        }

        this.SetField(ref this.scanState, value, "FeedScanState");
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("ScanFinished"));
      }
    }
    public Boolean ScanFinished { get { return this.FeedScanState == ProcessingStatus.Cancelled || this.FeedScanState == ProcessingStatus.Completed || this.FeedScanState == ProcessingStatus.Failed; } }
    #endregion

    #region Methods
    /// <summary>
    /// Cancel scan including all current and future downloads.
    /// </summary>
    public void CancelScan()
    {
      if (this.cancellationTokenSource == null || this.cancellationTokenSource.IsCancellationRequested)
      {
        return;
      }

      this.cancellationTokenSource.Cancel(true);
      this.downloadManager?.CancelJobs();
    }

    public void InitialiseForScan()
    {
      this.FeedScanState = ProcessingStatus.Waiting;
      this.cancellationTokenSource = new CancellationTokenSource();
    }

    public void Reset()
    {
      this.JobNavigation.Reset();
      this.UpdateScanProgressMessage(String.Empty);
      this.FeedScanState = ProcessingStatus.Idle;
    }

    public void Scan(IDownloadManagerFactory downloadManagerFactory)
    {
      try
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
          this.FeedScanState = ProcessingStatus.Running;
        });

        this.UpdateScanProgressMessage("Processing ...");
        
        var cancelToken = this.cancellationTokenSource.Token;
        var feedFilePath = Path.Combine(this.feed.Directory, "download.rss");
        var newFeed = FeedFunctions.UpdateFeed(feed, feedFilePath, cancelToken);

        // Creating the new feed may have taken a while - check for cancellation before processing podcasts.
        cancelToken.ThrowIfCancellationRequested();

        this.UpdateScanProgressMessage("Searching for new podcasts ...");
        var podcastIndexes = this.BuildNewPodcastIndexList(feed, newFeed);

        if (podcastIndexes.Count == 0)
        {
          this.UpdateScanProgressMessage("No podcasts found.");
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
              this.UpdateScanProgressMessage(podcastIndexes.Count + " podcasts found [CANCELLED].");
              this.cancellationTokenSource.Cancel();
              return;
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

        this.UpdateScanProgressMessage("Saving feed ... ");
        // Now update the feed to storage for real.
        newFeed = Feed.SetUpdatedDate(DateTime.Now, newFeed);
        this.feedCollection.UpdateFeedContent(newFeed);

        if (downloadConfirmation == DownloadConfirmationStatus.SkipDownloading)
        {
          this.UpdateScanProgressMessage(podcastIndexes.Count + " podcasts found [SKIPPED].");
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
        var lastIndex = podcastIndexes.Count - 1;
        this.downloadManager.JobFinishedEvent = j =>
        {
          if (this.imageResolver != null)
          {
            var podcast = newFeed.Podcasts[podcastIndexes[lastIndex - jobFinishedCount]];
            this.imageResolver.ResolvePodcastImage(podcast);
          }

          this.feedCollection.UpdateFeedContent(this.feed);
          jobFinishedCount++;
          if (jobFinishedCount % 2 == 0 && this.JobNavigation.CanMoveForward)
          {
            Application.Current.Dispatcher.Invoke(() =>
            {
              this.JobNavigation.PageNumber += 1;
            });
          }
        };

        downloadManager.AddJobs(jobs);
        downloadManager.StartWaitingJobs();

        this.UpdateScanProgressMessage("Done");

        Application.Current.Dispatcher.Invoke(() =>
        {
          this.FeedScanState = ProcessingStatus.Completed;
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
      finally
      {
        this.cancellationTokenSource = null;
      }
    }

    private void HandleScanCancelled()
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.UpdateScanProgressMessage("Cancelled");
        this.FeedScanState = ProcessingStatus.Cancelled;
      });
    }

    private void HandleScanError(Exception e)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanFailedMessage = e.Message;
        this.FeedScanState = ProcessingStatus.Failed;
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
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanProgressMessage = progressMessage;
        this.TryInvokePropertyChanged(FeedViewModel.FeedScanProgressMessageArgs);
      });
    }
    #endregion
  }
}
