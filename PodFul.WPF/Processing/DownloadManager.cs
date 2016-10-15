﻿
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Threading.Tasks;
  using System.Windows;
  using Jabberwocky.Toolkit.Object;
  using Library;

  public class DownloadManager
  {
    #region Fields
    private ILogger logger;

    private Queue<DownloadJob> waitingJobs;

    private UInt32 concurrentDownloads = 1;

    private Int32 currentDownloads = 0;
    #endregion

    #region Construction
    public DownloadManager(ILogger logger, UInt32 concurrentDownloads)
    {
      logger.VerifyThatObjectIsNotNull("Parameter 'logger' is null.");

      this.logger = logger;
      this.concurrentDownloads = concurrentDownloads;

      this.waitingJobs = new Queue<DownloadJob>();

      this.Jobs = new ObservableCollection<DownloadJob>();
    }
    #endregion

    #region Properties
    public ObservableCollection<DownloadJob> Jobs { get; private set; }

    public Boolean GotIncompleteJobs { get { return this.waitingJobs.Count > 0 || this.currentDownloads > 0; } }
    #endregion

    #region Events
    public Action AllDownloadsCompletedEvent;
    #endregion

    #region Methods
    public void AddJob(DownloadJob job)
    {
      this.waitingJobs.Enqueue(job);

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.Jobs.Add(job);
      });
    }

    public void AddJobs(IEnumerable<DownloadJob> jobs)
    {
      foreach (var job in jobs)
      {
        this.waitingJobs.Enqueue(job);
        this.Jobs.Add(job);
      }
    }

    public void CancelAllDownloads()
    {
      foreach (var podcast in this.Jobs)
      {
        podcast.CancelDownload();
      }
    }

    public void CancelDownload(Object dataContext)
    {
      dataContext.VerifyThatObjectIsNotNull("Parameter 'dataContext' is null.");
      var podcast = (DownloadJob)dataContext;
      podcast.CancelDownload();
    }

    public void StartDownloads()
    {
      while (this.currentDownloads < this.concurrentDownloads)
      {
        StartDownload();
      }
    }

    private void StartDownload()
    {
      if (this.waitingJobs.Count == 0)
      {
        // No more podcasts queued so do not start another download.

        if (this.currentDownloads == 0)
        {
          // All current downloads have finished so nothing more to do.
          AllDownloadsCompletedEvent?.Invoke();
        }

        return;
      }

      this.currentDownloads++;

      Task task = null;
      var job = this.waitingJobs.Dequeue();
      job.InitialiseBeforeDownload();
      try
      {
        task = Task.Factory.StartNew(() =>
        {
          var downloader = new FileDownloader();
          downloader.Download(job.URL, job.FilePath, job.CancellationToken, job.ProgressEventHandler);
        }, job.CancellationToken);
      }
      catch (Exception e)
      {
        // Catch any exceptions regarding the setup of the task. May not be necessary
        this.ProcessException(e, job);
        return;
      }

      task.ContinueWith(t =>
      {
        Application.Current.Dispatcher.Invoke(() => { job.CancellationCanBeRequested = false; });

        if (t.Exception != null)
        {
          this.ProcessException(t.Exception, job);
        }
        else if (t.IsCanceled)
        {
          job.DownloadCanceled();
        }
        else
        {
          try
          {
            job.DownloadCompleted();
          }
          catch (Exception e)
          {
            this.ProcessException(e, job);
          }
        }

        this.currentDownloads--;

        this.StartDownload();
      });
    }

    private void ProcessException(Exception exception, DownloadJob podcast)
    {
      Exception e = exception;
      if (exception is AggregateException)
      {
        e = ((AggregateException)exception).Flatten();
      }

      if (e.InnerException != null)
      {
        e = e.InnerException;
      }

      this.logger.Exception(e.Message);

      Application.Current.Dispatcher.Invoke(() =>
      {
        podcast.Status = DownloadJob.StatusTypes.Failed;
        podcast.ExceptionMessage = e.Message;
      });
    }
    #endregion
  }
}
