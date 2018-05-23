﻿
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using ViewModel;

  public class NewDownloadManager : IDownloadManager
  {
    private Queue<JobViewModel> jobs;
    private CancellationTokenSource cancellationTokenSource;

    public Action<JobViewModel> JobFinishedEvent { get; set; }
    public Action<JobViewModel> JobQueuedEvent { get; set; }

    public void AddJobs(IList<JobViewModel> jobViewModels)
    {
      if (this.JobQueuedEvent == null)
      {
        this.jobs = new Queue<JobViewModel>(jobViewModels);
        return;
      }

      this.jobs = new Queue<JobViewModel>(jobViewModels.Count);
      foreach (var jobViewModel in jobViewModels)
      {
        this.jobs.Enqueue(jobViewModel);
        this.JobQueuedEvent.Invoke(jobViewModel);
      }
    }

    public void CancelJobs()
    {
      if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested)
      {
        this.cancellationTokenSource.Cancel();
      }
    }

    public void StartWaitingJobs()
    {
      this.cancellationTokenSource = new CancellationTokenSource();

      while (this.jobs.Count > 0)
      {
        var job = this.jobs.Dequeue();
        if (job.Status == DownloadJobStatus.Waiting)
        {
          job.Download();
          this.JobFinishedEvent?.Invoke(job);
        }
      }
    }
  }
}
