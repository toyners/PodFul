using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PodFul.Library;

namespace PodFul.WPF
{
  /// <summary>
  /// Interaction logic for ProcessingWindow.xaml
  /// </summary>
  public partial class ProcessingWindow : Window
  {
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private TaskScheduler mainTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
    private Int64 fileSize;
    private Int64 downloadedSize;
    private Int64 percentageStepSize;
    private Boolean fileSizeNotKnown;

    public ProcessingWindow(IFeedStorage feedStorage, Queue<Int32> feedIndexes)
    {
      InitializeComponent();
    }

    public ProcessingWindow(IFeedStorage feedStorage, Feed feed, Queue<Int32> podcastIndexes)
    {
      InitializeComponent();
    }

    private void Cancel_Click(Object sender, RoutedEventArgs e)
    {

    }

    private void UpdateProgessEventHandler(Int32 bytesWrittenToFile)
    {
      if (this.fileSizeNotKnown)
      {
        return;
      }

      new Task(() =>
      {
        this.downloadedSize += bytesWrittenToFile;
        if (this.downloadedSize > this.fileSize)
        {
          this.Progress.Value = 100;
          return;
        }

        Int64 steps = this.downloadedSize / this.percentageStepSize;
        if (steps > this.Progress.Value)
        {
          this.Progress.Value = (Int32)steps;
        }
      }).Start(this.mainTaskScheduler);
    }

    private void ResetProgressBar(Int64 expectedFileSize = -1)
    {
      new Task(() =>
      {
        this.Progress.Value = 0;
        this.Progress.IsIndeterminate = (expectedFileSize == 0);
      }).Start(this.mainTaskScheduler);
    }
  }
}
