
namespace PodFul.WPF.UI_Support
{
  using System;
  using Processing;

  internal class JobCountDisplayManager : IJobCountDisplayManager
  {
    private IJobCountDisplayComponent[] jobCountDisplayComponents;
    private Int32 waitingJobsCount;
    private Int32 runningJobsCount;
    private Int32 completedJobsCount;
    private Int32 cancelledJobsCount;
    private Int32 failedJobsCount;

    public JobCountDisplayManager(params IJobCountDisplayComponent[] jobCountDisplayComponents)
    {
      this.jobCountDisplayComponents = jobCountDisplayComponents;
    }

    public void DisplayCounts()
    {
      foreach (var jobCountDisplayComponent in this.jobCountDisplayComponents)
      {
        jobCountDisplayComponent.UpdateCounts(this.waitingJobsCount, this.runningJobsCount, this.completedJobsCount, this.cancelledJobsCount, this.failedJobsCount);
      }
    }

    public void UpdateCounts(DownloadJob job)
    {
      switch (job.Status)
      {
        case DownloadJob.StatusTypes.Waiting: this.waitingJobsCount++; break;
        case DownloadJob.StatusTypes.Running: this.runningJobsCount++; break;
        case DownloadJob.StatusTypes.Completed: this.completedJobsCount++; break;
        case DownloadJob.StatusTypes.Canceled: this.cancelledJobsCount++; break;
        case DownloadJob.StatusTypes.Failed: this.failedJobsCount++; break;
      }

      this.DisplayCounts();
    }
  }
}
