
namespace PodFul.WPF.UI_Support
{
  using System;
  using System.Windows;

  internal class JobCountWindowTitleDisplayComponent : IJobCountDisplayComponent
  {
    private Window window;

    public JobCountWindowTitleDisplayComponent(Window window)
    {
      this.window = window;
    }

    public void UpdateCounts(Int32 waitingJobCount, Int32 runningJobCount, Int32 completedJobCount, Int32 cancelledJobCount, Int32 failedJobCount)
    {
      var title = String.Format("Downloading Podcasts: {0} waiting, {1} running, {2} completed", waitingJobCount, runningJobCount, completedJobCount);
      if (cancelledJobCount > 0)
      {
        title += ", " + cancelledJobCount + " cancelled";
      }

      if (failedJobCount > 0)
      {
        title += ", " + failedJobCount + " failed";
      }

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.window.Title = title;
      });
    }
  }
}
