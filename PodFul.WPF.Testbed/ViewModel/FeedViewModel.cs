﻿
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

    public enum ScanStates
    {
      Idle,
      Running,
      Completed
    }

    public FeedViewModel(Feed feed)
    {
      this.feed = feed;
      this.PodcastNavigation = new PodcastPageNavigation();
      this.PodcastNavigation.SetPages(this.feed.Podcasts, 2, (podcasts, firstIndex, lastIndex) => { return new PodcastPageViewModel2(podcasts, firstIndex, lastIndex); });
      this.JobNavigation = new JobPageNavigation();
    }

    public String Title { get { return this.feed.Title; } }
    public String Description { get { return this.feed.Description; } }
    public PodcastPageNavigation PodcastNavigation { get; set; }
    public JobPageNavigation JobNavigation { get; set; }
    public String FeedScanProgressMessage { get; private set; }
    public ScanStates FeedScanState { get; private set; }

    public void Reset()
    {
      this.FeedScanState = ScanStates.Idle;
    }

    public void Scan(IDownloadManagerFactory downloadManagerFactory)
    {
      System.Windows.Application.Current.Dispatcher.Invoke(() =>
      {
        this.FeedScanState = ScanStates.Running;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("FeedScanState"));
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

      this.UpdateScanProgressMessage(jobs.Count + " podcasts found.");

      this.UpdateScanProgressMessage("Updating feed");
      Thread.Sleep(1000);

      this.UpdateScanProgressMessage("Downloading podcasts");
      this.JobNavigation.SetPages(jobs, 2, (j, f, l) => { return new JobPageViewModel(j, f, l); });

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
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("FeedScanState"));
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
