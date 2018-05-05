
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using ViewModel;

  public class NewDownloadManager : IDownloadManager
  {
    private List<JobViewModel> jobs;

    public void AddJobs(IEnumerable<JobViewModel> jobViewModels)
    {
      this.jobs = new List<JobViewModel>(jobViewModels);
    }

    public void StartWaitingJobs()
    {
      throw new NotImplementedException();
    }

    private void QueueJob(JobViewModel jobViewModel)
    {
      
    }
  }
}
