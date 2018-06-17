
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using PodFul.Library;
  using ViewModel;

  public class NewDownloadManager : IDownloadManager
  {
    private CancellationTokenSource cancellationTokenSource;
    private DownloadManagerViewModel currentJob;
    private Queue<DownloadManagerViewModel> jobs;

    public Action<DownloadManagerViewModel> JobFinishedEvent { get; set; }
    public Action<DownloadManagerViewModel> JobQueuedEvent { get; set; }

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
