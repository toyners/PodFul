﻿
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using Library;

  public class FeedDownload : IFeedProcessor
  {
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private FeedCollection feedCollection;
    private Feed feed;
    private Queue<Int32> podcastIndexes;
    private IFileDeliverer fileDeliverer;
    private ILogger log;

    private Int64 fileSize;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;
    private Boolean fileSizeNotKnown;

    public Action<String> SetWindowTitleEvent;

    public Action<Boolean> SetCancelButtonStateEvent;

    public Action<String, Boolean> InitialiseProgressEvent;

    public Action ResetProgressEvent;

    public Action<String, Int32> SetProgressEvent;

    public FeedDownload(
      FeedCollection feedCollection,
      Feed feed,
      Queue<Int32> podcastIndexes,
      IFileDeliverer fileDeliverer,
      ILogger log)
    {
      this.feedCollection = feedCollection;
      this.feed = feed;
      this.podcastIndexes = podcastIndexes;
      this.fileDeliverer = fileDeliverer;
      this.log = log;
    }

    public void Cancel()
    {
      this.cancellationTokenSource.Cancel();
    }

    public void Process()
    {
      var title = "Downloading " + this.podcastIndexes.Count + " podcast" + (this.podcastIndexes.Count != 1 ? "s" : String.Empty);
      this.SetWindowTitleEvent?.Invoke(title);

      var cancelToken = this.cancellationTokenSource.Token;

      var podcastDownload = this.InitialisePodcastDownload(cancelToken);

      Task task = Task.Factory.StartNew(() =>
      {
        this.SetCancelButtonStateEvent?.Invoke(true);
        if (podcastDownload.Download(this.feed.Directory, this.feed.Podcasts, this.podcastIndexes))
        {
          feedCollection.UpdateFeed(this.feed);
        }

        this.SetCancelButtonStateEvent?.Invoke(false);

      }, cancelToken);
    }

    private PodcastDownload InitialisePodcastDownload(CancellationToken cancelToken)
    {
      var podcastDownload = new PodcastDownload(cancelToken, this.UpdateProgessEventHandler);

      podcastDownload.OnBeforeDownload += (podcast) =>
      {
        this.fileSize = podcast.FileSize;
        this.percentageStepSize = this.fileSize / 100;
        this.downloadedSize = 0;
        this.InitialiseProgress(podcast.FileSize);
        this.log.Message(String.Format("Downloading \"{0}\" ... ", podcast.Title), false);
      };

      podcastDownload.OnSuccessfulDownload += (podcast, filePath) =>
      {
        this.log.Message("Completed.");
        this.log.Message(String.Empty); //Blank line to break up text flow
        this.fileDeliverer.Deliver(podcast, filePath);
      };

      podcastDownload.OnException += (podcast, exception) =>
      {
        Exception e = exception;
        if (exception is AggregateException)
        {
          e = ((AggregateException)exception).Flatten();
        }

        if (e.InnerException != null)
        {
          e = e.InnerException;
        }

        this.log.Exception(e.Message);
      };

      podcastDownload.OnFinish += () => this.ResetProgressEvent?.Invoke();

      return podcastDownload;
    }

    private void InitialiseProgress(Int64 expectedFileSize = -1)
    {
      this.fileSizeNotKnown = (expectedFileSize <= 0);
      String progressSize;
      if (expectedFileSize > 0)
      {
        progressSize = "0.0%";
      }
      else if (expectedFileSize == 0)
      {
        progressSize = "0.0Mb";
      }
      else
      {
        progressSize = String.Empty;
      }

      this.InitialiseProgressEvent?.Invoke(progressSize, this.fileSizeNotKnown);
    }

    private void UpdateProgessEventHandler(Int32 bytesWrittenToFile)
    {
      this.downloadedSize += bytesWrittenToFile;

      Int64 value = 100;
      if (this.downloadedSize < this.fileSize)
      {
        value = this.downloadedSize / this.percentageStepSize;
      }

      String text;
      if (this.fileSizeNotKnown)
      {
        var downloadedSizeInMb = this.downloadedSize / 1048576.0;
        text = downloadedSizeInMb.ToString("0.0") + "Mb";
      }
      else 
      {
        if (value == 100)
        {
          text = "100%";
        }
        else
        {
          var percentageValue = (Double)this.downloadedSize / this.percentageStepSize;
          text = percentageValue.ToString("0.0") + "%";
        }
      }

      this.SetProgressEvent?.Invoke(text, (Int32)value);
    }
  }
}
