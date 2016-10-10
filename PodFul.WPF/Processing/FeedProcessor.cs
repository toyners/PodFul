
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using Library;

  public abstract class FeedProcessor
  {
    protected CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    protected FeedCollection feedCollection;
    protected Queue<Int32> indexes;
    protected IImageResolver imageResolver;
    protected IFileDeliverer fileDeliverer;
    protected ILogger log;

    private Int64 fileSize;
    private Boolean fileSizeNotKnown;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;

    public Action<String> SetWindowTitleEvent;

    public Action<Boolean> SetCancelButtonStateEvent;

    protected FeedProcessor(
      FeedCollection feedCollection,
      Queue<Int32> indexes,
      IImageResolver imageResolver,
      IFileDeliverer fileDeliverer,
      ILogger log)
    {
      this.feedCollection = feedCollection;
      this.indexes = indexes;
      this.imageResolver = imageResolver;
      this.fileDeliverer = fileDeliverer;
      this.log = log;
    }

    public void Cancel()
    {
      this.cancellationTokenSource.Cancel();
    }

    public abstract void Process();

    protected PodcastDownload InitialisePodcastDownload(CancellationToken cancelToken)
    {
      var podcastDownload = new PodcastDownload(cancelToken, this.UpdateProgessEventHandler, this.imageResolver);

      podcastDownload.OnBeforeDownload += (podcast) =>
      {
        this.fileSize = podcast.FileDetails.FileSize;
        this.percentageStepSize = this.fileSize / 100;
        this.downloadedSize = 0;
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
