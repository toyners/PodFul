
namespace PodFul.WPF.UI_Support
{
  using System;

  internal class JobCountDisplayManager : IJobCountDisplayComponent
  {
    private IJobCountDisplayComponent[] jobCountDisplayComponents;

    public JobCountDisplayManager(params IJobCountDisplayComponent[] jobCountDisplayComponents)
    {
      this.jobCountDisplayComponents = jobCountDisplayComponents;
    }

    public void UpdateCounts(Int32 waitingJobCount, Int32 runningJobCount, Int32 completedJobCount, Int32 cancelledJobCount, Int32 failedJobCount)
    {
      foreach (var jobCountDisplayComponent in this.jobCountDisplayComponents)
      {
        jobCountDisplayComponent.UpdateCounts(waitingJobCount, runningJobCount, completedJobCount, cancelledJobCount, failedJobCount);
      }
    }
  }
}
