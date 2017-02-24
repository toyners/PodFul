
namespace PodFul.WPF.UI_Support
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  internal class JobCountStatusBarDisplayComponent : IJobCountDisplayComponent
  {
    private TextBlock waitingCountTextblock;
    private TextBlock runningCountTextblock;
    private TextBlock completedCountTextblock;
    private TextBlock firstOptionalCountTextblock;
    private TextBlock secondOptionalCountTextblock;
    private DownloadJobCounter cancelledJobCounter;
    private DownloadJobCounter failedJobCounter;

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
    }

    public void UpdateCounts(Int32 waitingJobCount, Int32 runningJobCount, Int32 completedJobCount, Int32 cancelledJobCount, Int32 failedJobCount)
    {
      if (cancelledJobCount > 0)
      {
        this.ProcessCancelCount(cancelledJobCount);
      }

      if (failedJobCount > 0)
      {
        this.ProcessFailedCount(failedJobCount);
      }

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.waitingCountTextblock.Text = "Waiting: " + waitingJobCount;
        this.runningCountTextblock.Text = "Running: " + runningJobCount;
        this.completedCountTextblock.Text = "Completed: " + completedJobCount;

        if (this.cancelledJobCounter != null)
        {
          this.cancelledJobCounter.UpdateTextCount();
        }

        if (this.failedJobCounter != null)
        {
          this.failedJobCounter.UpdateTextCount();
        }
      });
    }

    private void ProcessCancelCount(Int32 cancelledJobCount)
    {
      if (this.cancelledJobCounter == null)
      {
        Brush cancelledJobCounterBrush = new SolidColorBrush();
        this.cancelledJobCounter = new DownloadJobCounter("Cancelled: ", cancelledJobCounterBrush);

        if (this.failedJobCounter != null)
        {
          this.failedJobCounter.TextControl = this.secondOptionalCountTextblock;
        }

        this.cancelledJobCounter.TextControl = this.firstOptionalCountTextblock;
      }

      this.cancelledJobCounter.Count = cancelledJobCount;
    }

    private void ProcessFailedCount(Int32 failedJobCount)
    {
      if (this.failedJobCounter == null)
      {
        Brush failedJobCounterBrush = new SolidColorBrush();
        this.failedJobCounter = new DownloadJobCounter("Failed: ", failedJobCounterBrush);
        this.failedJobCounter.TextControl = (this.cancelledJobCounter != null ? this.secondOptionalCountTextblock : this.firstOptionalCountTextblock);
      }

      this.failedJobCounter.Count = failedJobCount;
    }

    private class DownloadJobCounter
    {
      private String text;
      private Brush brush;
      private TextBlock textBlock;

      public DownloadJobCounter(String text, Brush brush)
      {
        this.text = text;
        this.brush = brush;
      }

      public TextBlock TextControl
      {
        set
        {
          this.textBlock = value;
          this.textBlock.Foreground = this.brush;
        }
      }

      public Int32 Count { get; set; }

      public void UpdateTextCount()
      {
        this.textBlock.Text = this.text + this.Count;
      }
    }
  }
}
