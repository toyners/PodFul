
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using ViewModel;

  public class Scanner
  {
    private CancellationTokenSource cancellationTokenSource;

    public Action ScanCompletedEvent { get; set; }

    public void CancelScanning()
    {
      if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested)
      {
        this.cancellationTokenSource.Cancel(true);
      }
    }

    public void ScanFeeds(IList<FeedViewModel> feedViewModels, IDownloadManagerFactory downloadManagerFactory)
    {
      var maxTaskCount = 1;
      this.cancellationTokenSource = new CancellationTokenSource();
      var cancelToken = this.cancellationTokenSource.Token;

      var scanningTask = Task.Factory.StartNew(() =>
      {
        var feedViewModelQueue = new ConcurrentQueue<FeedViewModel>(feedViewModels);

        var objectLock = new Object();
        var scanTaskCount = 0;
        var completedScan = false;
        FeedViewModel feedViewModel;
        
        while (!completedScan)
        {
          cancelToken.ThrowIfCancellationRequested();
          if (scanTaskCount < maxTaskCount && feedViewModelQueue.Count > 0 && feedViewModelQueue.TryDequeue(out feedViewModel))
          {
            lock (objectLock)
            {
              scanTaskCount++;
            }

            Task.Factory.StartNew(() =>
            {
              feedViewModel.Scan(downloadManagerFactory);
              lock (objectLock)
              {
                scanTaskCount--;
              }
            });
          }

          Thread.Sleep(500);

          completedScan = (scanTaskCount == 0 && feedViewModelQueue.Count == 0);
        }
      });

      scanningTask.ContinueWith((t) =>
      {
        this.ScanCompletedEvent?.Invoke();
      });
    }
  }
}
