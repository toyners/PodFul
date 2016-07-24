
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using Library;

  public class FeedProcessor
  {
    protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    protected FeedCollection feedCollection;
    protected Queue<Int32> indexes;
    protected IFileDeliverer fileDeliverer;
    protected ILogger log;

    private Int64 fileSize;
    private Boolean fileSizeNotKnown;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;

    public Action<String> SetWindowTitleEvent;

    public Action<Boolean> SetCancelButtonStateEvent;

    public Action<String, String, String, Boolean> InitialiseProgressEvent;

    public Action ResetProgressEvent;

    public Action<String, String, Int32> SetProgressEvent;

    protected FeedProcessor(
      FeedCollection feedCollection,
      Queue<Int32> indexes,
      IFileDeliverer fileDeliverer,
      ILogger log)
    {
      this.feedCollection = feedCollection;
      this.indexes = indexes;
      this.fileDeliverer = fileDeliverer;
      this.log = log;
    }

    public void Cancel()
    {
      this.cancellationTokenSource.Cancel();
    }

    protected void InitialiseProgress(Int64 expectedFileSize = -1)
    {
      this.fileSizeNotKnown = (expectedFileSize <= 0);
      String progressMajorSize;
      String progressMinorSize;
      String progressUnit;
      if (expectedFileSize > 0)
      {
        progressMajorSize = "0";
        progressMinorSize = ".0";
        progressUnit = "%";
      }
      else if (expectedFileSize == 0)
      {
        progressMajorSize = "0";
        progressMinorSize = ".0";
        progressUnit = "MB";
      }
      else
      {
        progressMajorSize = progressMinorSize = progressUnit = String.Empty;
      }

      this.InitialiseProgressEvent?.Invoke(progressMajorSize, progressMinorSize, progressUnit, this.fileSizeNotKnown);
    }

    protected PodcastDownload InitialisePodcastDownload(CancellationToken cancelToken)
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

      podcastDownload.OnSuccessfulDownload += OnSuccessfulDownload;

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

    protected virtual void OnSuccessfulDownload(Podcast podcast, String filePath)
    {
      this.log.Message("Completed.");
      this.fileDeliverer.Deliver(podcast, filePath);
    }

    private void UpdateProgessEventHandler(Int32 bytesWrittenToFile)
    {
      this.downloadedSize += bytesWrittenToFile;

      Int64 value = 100;
      if (this.downloadedSize < this.fileSize)
      {
        value = this.downloadedSize / this.percentageStepSize;
      }

      String size;
      String majorSize;
      String minorSize;
      if (this.fileSizeNotKnown)
      {
        var downloadedSizeInMb = this.downloadedSize / 1048576.0;
        this.GetMajorMinorComponentsOfSize(downloadedSizeInMb, out majorSize, out minorSize);
      }
      else
      {
        if (value == 100)
        {
          majorSize = "100";
          minorSize = ".0";
        }
        else
        {
          var percentageValue = (Double)this.downloadedSize / this.percentageStepSize;
          this.GetMajorMinorComponentsOfSize(percentageValue, out majorSize, out minorSize);
        }
      }

      this.SetProgressEvent?.Invoke(majorSize, minorSize, (Int32)value);
    }

    private void GetMajorMinorComponentsOfSize(Double value, out String majorSize, out String minorSize)
    {
      var size = value.ToString("0.0");
      Int32 index = size.IndexOf('.');
      majorSize = size.Substring(0, index);
      minorSize = size.Substring(index);
    }
  }
}
