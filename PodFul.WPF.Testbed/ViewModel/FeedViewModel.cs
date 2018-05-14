
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Threading;
  using Jabberwocky.Toolkit.WPF;
  using Library;
  using Processing;

  public class FeedViewModel : NotifyPropertyChangedBase
  {
    private Feed feed;
    private ScanStates scanState;

    public enum ScanStates
    {
      Idle,
      Waiting,
      Running,
      Completed
    }

    public FeedViewModel(Feed feed)
    {
      this.feed = feed;
      this.PodcastNavigation = new PodcastPageNavigation();
      this.PodcastNavigation.SetPages(this.feed.Podcasts, 2);
      this.JobNavigation = new JobPageNavigation();
    }

    public String Title { get { return this.feed.Title; } }
    public String Description { get { return this.feed.Description; } }
    public PodcastPageNavigation PodcastNavigation { get; set; }
    public JobPageNavigation JobNavigation { get; set; }
    public String FeedScanProgressMessage { get; private set; }
    public ScanStates FeedScanState
    {
      get { return this.scanState; }
      private set
      {
        if (this.scanState == value)
        {
          return;
        }

        this.SetField(ref this.scanState, value, "FeedScanState");
      }
    }

    public void InitialiseForScan()
    {
      this.FeedScanState = ScanStates.Waiting;
    }

    public void Reset()
    {
      this.JobNavigation.Reset();
      this.UpdateScanProgressMessage(String.Empty);
      this.FeedScanState = ScanStates.Idle;
    }

    public void Scan(IDownloadManagerFactory downloadManagerFactory)
    {
      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanState = ScanStates.Running;
      });

      this.UpdateScanProgressMessage("Scanning feed");
      Thread.Sleep(1000);

      this.UpdateScanProgressMessage("Searching for new podcasts ... ");
      Thread.Sleep(1000);

      var jobs = new List<JobViewModel>();
      foreach (var podcast in this.feed.Podcasts)
      {
        jobs.Add(new JobViewModel(podcast, feed));
      }

      this.UpdateScanProgressMessage("Updating feed (" + jobs.Count + " podcasts found).");
      Thread.Sleep(1000);

      this.UpdateScanProgressMessage("Downloading " + jobs.Count + " podcasts");
      this.JobNavigation.SetPages(jobs, 2);

      var downloadManager = downloadManagerFactory.Create();
      var jobFinishedCount = 0;
      downloadManager.JobFinishedEvent = j =>
      {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
          jobFinishedCount++;
          if (jobFinishedCount % 2 == 0 && this.JobNavigation.CanMoveForward)
          {
            this.JobNavigation.PageNumber += 1;
          }
        });
      };

      downloadManager.AddJobs(jobs);
      downloadManager.StartWaitingJobs();

      this.UpdateScanProgressMessage("Done");

      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanState = ScanStates.Completed;
      });
    }

    private void UpdateScanProgressMessage(String progressMessage)
    {
      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanProgressMessage = progressMessage;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("FeedScanProgressMessage"));
      });
    }
  }
}
