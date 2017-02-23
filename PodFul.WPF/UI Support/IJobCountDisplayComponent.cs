
namespace PodFul.WPF.UI_Support
{
  using System;

  internal interface IJobCountDisplayComponent
  {
    void UpdateCounts(Int32 waitingJobCount, Int32 runningJobCount, Int32 completedJobCount, Int32 cancelledJobCount, Int32 failedJobCount);
  }
}
