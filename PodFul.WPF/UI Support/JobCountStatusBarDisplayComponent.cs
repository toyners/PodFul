
namespace PodFul.WPF.UI_Support
{
  using System;
  using System.Windows.Controls;
  using System.Windows.Media;

  internal class JobCountStatusBarDisplayComponent : IJobCountDisplayComponent
  {
    private TextBlock waitingCountTextblock;
    private TextBlock runningCountTextblock;
    private TextBlock completedCountTextblock;
    private TextBlock firstOptionalCountTextblock;
    private TextBlock secondOptionalCountTextblock;
    private Brush failedBrush;
    private Brush cancelledBrush;

    public JobCountStatusBarDisplayComponent(
      TextBlock waitingCountTextblock,
      TextBlock runningCountTextblock,
      TextBlock completedCountTextblock,
      TextBlock firstOptionalCountTextblock,
      TextBlock secondOptionalCountTextblock)
    {
      this.waitingCountTextblock = waitingCountTextblock;
      this.runningCountTextblock = runningCountTextblock;
      this.completedCountTextblock = completedCountTextblock;
      this.firstOptionalCountTextblock = firstOptionalCountTextblock;
      this.secondOptionalCountTextblock = secondOptionalCountTextblock;
      this.firstOptionalCountTextblock.Text = String.Empty;
      this.secondOptionalCountTextblock.Text = String.Empty;

      var brushConverter = new BrushConverter();
      this.failedBrush = brushConverter.ConvertFromString("Red") as SolidColorBrush;
      this.cancelledBrush = brushConverter.ConvertFromString("OrangeRed") as SolidColorBrush;
    }

    public void UpdateCounts(Int32 waitingJobCount, Int32 runningJobCount, Int32 completedJobCount, Int32 cancelledJobCount, Int32 failedJobCount)
    {
      this.waitingCountTextblock.Text = "Waiting: " + waitingJobCount;
      this.runningCountTextblock.Text = "Running: " + runningJobCount;
      this.completedCountTextblock.Text = "Completed: " + completedJobCount;

      if (cancelledJobCount > 0 && failedJobCount > 0)
      {
        this.firstOptionalCountTextblock.Foreground = this.cancelledBrush;
        this.firstOptionalCountTextblock.Text = "Cancelled: " + cancelledJobCount;
        this.secondOptionalCountTextblock.Foreground = this.failedBrush;
        this.secondOptionalCountTextblock.Text = "Failed: " + failedJobCount;
      }
      else if (cancelledJobCount > 0)
      {
        this.firstOptionalCountTextblock.Text = "Cancelled: " + cancelledJobCount;
      }
      else if (failedJobCount > 0)
      {
        this.firstOptionalCountTextblock.Foreground = this.failedBrush;
        this.firstOptionalCountTextblock.Text = "Cancelled: " + failedJobCount;
      }
    }
  }
}
