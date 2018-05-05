
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using Library;
  using ViewModel;

  public class NewDownloadManager : IDownloadManager
  {
    private Queue<JobViewModel> jobs;

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

    public void StartWaitingJobs()
    {
      while (this.jobs.Count > 0)
      {
        var job = this.jobs.Dequeue();
        job.Download();
      }
    }
  }
}
