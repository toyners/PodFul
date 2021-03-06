﻿
namespace PodFul.WPF.Processing.TileView
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Threading;
  using System.Threading.Tasks;
  using PodFul.WPF.ViewModel.TileView;

  public class Scanner
  {
    private CancellationTokenSource cancellationTokenSource;

    public Action ScanCompletedEvent { get; set; }

    public void CancelScan()
    {
      if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested)
      {
        this.cancellationTokenSource.Cancel(true);
      }
    }

    public void ScanFeeds(IList<FeedViewModel> feedViewModels)
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
          if (scanTaskCount < maxTaskCount && 
            feedViewModelQueue.Count > 0 && 
            feedViewModelQueue.TryDequeue(out feedViewModel) &&
            feedViewModel.FeedScanState == ProcessingStatus.Waiting)
          {
            if (cancelToken.IsCancellationRequested)
            {
              feedViewModel.CancelScan();
              continue;
            }

            lock (objectLock)
            {
              scanTaskCount++;
            }

            Task.Factory.StartNew(() =>
            {
              feedViewModel.Scan();
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
