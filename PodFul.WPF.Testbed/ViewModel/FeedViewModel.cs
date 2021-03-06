﻿
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Threading;
  using System.Windows;
  using Jabberwocky.Toolkit.String;
  using Jabberwocky.Toolkit.WPF;
  using Library;
  using Miscellaneous;
  using WPF.Processing;

  public class FeedViewModel : NotifyPropertyChangedBase
  {
    #region Fields
    private static PropertyChangedEventArgs FeedScanProgressMessageArgs = new PropertyChangedEventArgs("FeedScanProgressMessage");
    private CancellationTokenSource cancellationTokenSource = null;
    private PodcastViewModel currentDownload;
    private String downloadCount;
    private Feed feed;
    private readonly IFeedCollection feedCollection;
    private readonly Int32 feedIndex;
    private String feedStatusMessage;
    private readonly IFileDownloadProxyFactory fileDownloadProxyFactory;
    private readonly IImageResolver imageResolver;
    private ProcessingStatus scanState;
    #endregion

    #region Construction
    public FeedViewModel(Int32 index, IFeedCollection feedCollection, IImageResolver imageResolver, IFileDownloadProxyFactory fileDownloadProxyFactory)
    {
      this.feedCollection = feedCollection;
      this.feedIndex = index;
      this.feed = this.feedCollection[this.feedIndex];
      this.fileDownloadProxyFactory = fileDownloadProxyFactory;
      this.imageResolver = imageResolver;

      this.PodcastNavigation = new PodcastPageNavigation(this, this.fileDownloadProxyFactory);
      this.PodcastNavigation.SetPages(this.feed.Podcasts, 3);
    }
    #endregion

    #region Properties
    public Boolean CompleteDownloadsOnScan
    {
      get { return this.feed.CompleteDownloadsOnScan; }
      set
      {
        this.feed = Feed.SetCompleteDownloadsOnScanFlag(value, this.feed);
        this.feedCollection.UpdateFeedContent(this.feed);
        this.feedCollection[this.feedIndex] = this.feed;
      }
    }
    public UInt32 ConfirmDownloadThreshold
    {
      get { return this.feed.ConfirmDownloadThreshold; }
      set
      {
        this.feed = Feed.SetConfirmDownloadThreshold(value, this.feed);
        this.feedCollection.UpdateFeedContent(this.feed);
        this.feedCollection[this.feedIndex] = this.feed;
      }
    }
    public DateTime CreatedDate { get { return this.feed.CreationDateTime; } }
    public PodcastViewModel CurrentDownload
    {
      get { return this.currentDownload; }
      private set
      {
        this.SetField(ref this.currentDownload, value);
      }
    }
    public Boolean DeliverDownloadsOnScan
    {
      get { return this.feed.DeliverDownloadsOnScan; }
      set
      {
        this.feed = Feed.SetDeliverDownloadsOnScanFlag(value, this.feed);
        this.feedCollection.UpdateFeedContent(this.feed);
        this.feedCollection[this.feedIndex] = this.feed;
      }
    }
    public Boolean DoScan
    {
      get { return this.feed.DoScan; }
      set
      {
        this.feed = Feed.SetDoScanFlag(value, this.feed);
        this.feedCollection.UpdateFeedContent(this.feed);
        this.feedCollection[this.feedIndex] = this.feed;
      }
    }
    public String DownloadCount
    {
      get { return this.downloadCount; }
      private set { this.SetField(ref this.downloadCount, value); }
    }
    public String Title { get { return this.feed.Title; } }
    public String Description { get { return this.feed.Description; } }
    public String FeedDirectoryPath
    {
      get { return this.feed.Directory; }
      set
      {
        this.feed = Feed.SetDirectory(value, this.feed);
        this.feedCollection.UpdateFeedContent(this.feed);
        this.feedCollection[this.feedIndex] = this.feed;
      }
    }
    public String FeedImage { get { return this.feed.ImageFileName; } }
    public String FeedStatusMessage
    {
      get { return this.feedStatusMessage; }
      set { this.SetField(ref this.feedStatusMessage, value); }
    }
    public String FeedURL { get { return this.feed.URL; } }
    public PodcastPageNavigation PodcastNavigation { get; set; }
    public String FeedScanProgressMessage { get; private set; }
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
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("ScanRunning"));
      }
    }
    public Int32 PodcastCount { get { return this.feed.Podcasts.Length; } }
    public Boolean ScanFinished { get { return this.FeedScanState == ProcessingStatus.Cancelled || this.FeedScanState == ProcessingStatus.Completed || this.FeedScanState == ProcessingStatus.Failed; } }
    public Boolean ScanRunning { get { return this.FeedScanState == ProcessingStatus.Waiting || this.FeedScanState == ProcessingStatus.Scanning || this.FeedScanState == ProcessingStatus.Downloading; } }
    public DateTime UpdatedDate { get { return this.feed.UpdatedDateTime; } }
    public String WebsiteURL { get { return this.feed.Website; } }
    #endregion

    #region Methods
    /// <summary>
    /// Cancel scan including all current and future downloads.
    /// </summary>
    public void CancelScan()
    {
      if (this.cancellationTokenSource == null || 
        this.cancellationTokenSource.IsCancellationRequested ||
        this.FeedScanState == ProcessingStatus.Cancelled)
      {
        return;
      }

      this.cancellationTokenSource.Cancel(true);
    }

    public void InitialiseForScan()
    {
      this.FeedScanState = ProcessingStatus.Waiting;
      this.cancellationTokenSource = new CancellationTokenSource();
    }

    public void Reset()
    {
      this.FeedStatusMessage = String.Empty;
      this.FeedScanState = ProcessingStatus.Idle;
    }

    public void Scan()
    {
      try
      {
        this.FeedScanState = ProcessingStatus.Scanning;

        //this.UpdateScanProgressMessage("Processing ...");
        
        var cancelToken = this.cancellationTokenSource.Token;
        var feedFilePath = Path.Combine(this.feed.Directory, "download.rss");
        var newFeed = FeedFunctions.UpdateFeed(this.feed, feedFilePath, cancelToken);

        // Creating the new feed may have taken a while - check for cancellation before processing podcasts.
        cancelToken.ThrowIfCancellationRequested();

        //this.UpdateScanProgressMessage("Searching for new podcasts ...");
        var podcastIndexes = this.BuildNewPodcastIndexList(this.feed, newFeed);

        if (podcastIndexes.Count == 0)
        {
          this.FeedStatusMessage = "No new podcasts found";
          newFeed = Feed.SetUpdatedDate(DateTime.Now, newFeed);
          this.feedCollection.UpdateFeedContent(newFeed);

          this.FeedScanState = ProcessingStatus.Completed;
          return;
        }

        var downloadConfirmation = (!newFeed.CompleteDownloadsOnScan ? DownloadConfirmationStatus.SkipDownloading : DownloadConfirmationStatus.ContinueDownloading);
        if (downloadConfirmation != DownloadConfirmationStatus.SkipDownloading)
        {
          if (podcastIndexes.Count >= newFeed.ConfirmDownloadThreshold)
          {
            IPodcastDownloadConfirmer podcastDownloadConfirmer = new PodcastDownloadConfirmer();
            downloadConfirmation = podcastDownloadConfirmer.ConfirmPodcastsForDownload(this.feed, newFeed, podcastIndexes);

            if (downloadConfirmation == DownloadConfirmationStatus.CancelScanning)
            {
              this.FeedStatusMessage = podcastIndexes.Count + " podcast".Pluralize((uint)podcastIndexes.Count) + " found [CANCELLED]";
              this.FeedScanState = ProcessingStatus.Cancelled;
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

        // Update the feed in storage.
        //this.UpdateScanProgressMessage("Saving feed ... ");
        newFeed = Feed.SetUpdatedDate(DateTime.Now, newFeed);
        this.feedCollection.UpdateFeedContent(newFeed);

        if (downloadConfirmation == DownloadConfirmationStatus.SkipDownloading)
        {
          this.FeedStatusMessage = (podcastIndexes.Count + " podcasts found [SKIPPED]");
          this.FeedScanState = ProcessingStatus.Completed;
          return;
        }

        // Update the reference for feed in the view model.
        this.feed = newFeed;
        this.PodcastNavigation.Reset();
        this.PodcastNavigation.SetPages(this.feed.Podcasts, 3);

        this.FeedScanState = ProcessingStatus.Downloading;
        //this.UpdateScanProgressMessage("Downloading " + podcastIndexes.Count + " podcasts ...");

        var fileDownloader = new FileDownloader();
        var number = 1;
        for (var index = podcastIndexes.Count - 1; index >= 0; index--)
        {
          this.DownloadCount = $"[{number++}/{podcastIndexes.Count}]";
          this.CurrentDownload = this.PodcastNavigation[podcastIndexes[index]];
          this.CurrentDownload.ScanDownload(fileDownloader, cancelToken);
        }

        //this.UpdateScanProgressMessage(podcastIndexes.Count + " podcast".Pluralize((uint)podcastIndexes.Count) + " downloaded");

        this.FeedScanState = ProcessingStatus.Completed;
      }
      catch (OperationCanceledException)
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

    private void DownloadManager_DownloadCompletedEvent(Podcast obj)
    {
      throw new NotImplementedException();
    }

    private void HandleScanCancelled()
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanState = ProcessingStatus.Cancelled;
        this.FeedStatusMessage = "Cancelled";
      });
    }

    private void HandleScanError(Exception e)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanState = ProcessingStatus.Failed;
        this.FeedStatusMessage = e.Message;
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

    /*private void UpdateScanProgressMessage(String progressMessage)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanProgressMessage = progressMessage;
        this.TryInvokePropertyChanged(FeedViewModel.FeedScanProgressMessageArgs);
      });
    }*/
    #endregion
  }
}
