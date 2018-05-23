
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
    public Action ScanCompletedEvent { get; set; }

    public void ScanFeeds(IList<FeedViewModel> feedViewModels, IDownloadManagerFactory downloadManagerFactory)
    {
      var maxTaskCount = 1;
      var scanningTask = Task.Factory.StartNew(() =>
      {
        var feedViewModelQueue = new ConcurrentQueue<FeedViewModel>(feedViewModels);

        var objectLock = new Object();
        var scanTaskCount = 0;
        var completedScan = false;
        FeedViewModel feedViewModel;

        while (!completedScan)
        {
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
