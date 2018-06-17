
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using PodFul.Library;
  using ViewModel;

  public class NewDownloadManager : INewDownloadManager
  {
    private CancellationTokenSource cancellationTokenSource;
    private DownloadManagerViewModel currentJob;
    private Queue<DownloadManagerViewModel> jobs;

    public Action<DownloadManagerViewModel> JobFinishedEvent { get; set; }
    public Action<DownloadManagerViewModel> JobQueuedEvent { get; set; }
    public Action<Int32> ProgressEventHandler { get; set; }
    public Action<Podcast, String> DownloadCompletedEvent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void AddJobs(IList<DownloadManagerViewModel> jobViewModels)
    {
      if (this.JobQueuedEvent == null)
      {
        this.jobs = new Queue<DownloadManagerViewModel>(jobViewModels);
        return;
      }

      this.jobs = new Queue<DownloadManagerViewModel>(jobViewModels.Count);
      foreach (var jobViewModel in jobViewModels)
      {
        this.jobs.Enqueue(jobViewModel);
        this.JobQueuedEvent.Invoke(jobViewModel);
      }
    }

    private Queue<Int32> podcastIndexes;
    private Feed feed;

    public void AddJobs(IList<Int32> podcastIndexes, Feed feed)
    {

    }

    public void CancelJobs()
    {
      if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested)
      {
        this.cancellationTokenSource.Cancel();
        this.currentJob?.CancelDownload();
      }
    }

    public void DownloadPodcasts()
    {
      this.cancellationTokenSource = new CancellationTokenSource();
      var cancelToken = this.cancellationTokenSource.Token;

      while (this.podcastIndexes.Count == 0)
      {
        var podcastIndex = this.podcastIndexes.Dequeue();
        var podcast = this.feed.Podcasts[podcastIndex];
        var fileDownloader = new FileDownloader();
        var filePath = Path.Combine(this.feed.Directory, podcast.FileDetails.FileName);
        fileDownloader.Download(podcast.URL, filePath, cancelToken, this.ProgressEventHandler);

        // Maybe I should do this?
        // this.DownloadCompletedEvent?.Invoke(podcast, filePath);

        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
          throw new FileNotFoundException(String.Format("Podcast file '{0}' is missing.", filePath));
        }

        podcast.SetFileDetails(fileInfo.Length, DateTime.Now);
      }
    }

    public void StartWaitingJobs()
    {
      try
      {
        this.cancellationTokenSource = new CancellationTokenSource();
        var cancelToken = this.cancellationTokenSource.Token;

        while (this.jobs.Count > 0 && !cancelToken.IsCancellationRequested)
        {
          this.currentJob = this.jobs.Dequeue();
          if (this.currentJob.Status == ProcessingStatus.Waiting)
          {
            this.currentJob.Download();
            this.JobFinishedEvent?.Invoke(this.currentJob);
          }
        }
      }
      catch (OperationCanceledException)
      {
        this.currentJob?.CancelDownload();
        this.JobFinishedEvent?.Invoke(this.currentJob);
      }
      finally
      {
        this.currentJob = null;
      }
    }
  }
}
