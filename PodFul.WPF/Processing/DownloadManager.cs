﻿
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using Jabberwocky.Toolkit.Object;
  using Library;
  using Logging;

  public class DownloadManager
  {
    #region Fields
    private Boolean isCancelingAll;
    private UInt32 concurrentDownloads = 1;
    private Int32 currentDownloads;
    private ILogger logger;
    private ConcurrentQueue<DownloadJob> waitingJobs;
    #endregion

    #region Construction
    public DownloadManager(ILogger logger, UInt32 concurrentDownloads)
    {
      logger.VerifyThatObjectIsNotNull("Parameter 'exceptionLogger' is null.");
      this.logger = logger;
      this.concurrentDownloads = concurrentDownloads;

      this.waitingJobs = new ConcurrentQueue<DownloadJob>();

      this.Jobs = new ObservableCollection<DownloadJob>();
    }
    #endregion

    #region Properties
    public Boolean GotIncompleteJobs { get { return this.GotWaitingJobs || this.currentDownloads > 0; } }

    public Boolean GotWaitingJobs { get { return this.waitingJobs.Count > 0; } }

    public ObservableCollection<DownloadJob> Jobs { get; private set; }
    #endregion

    #region Events
    public event Action AllJobsFinishedEvent;

    public event Action<DownloadJob> JobCompletedSuccessfullyEvent;

    public event Action<DownloadJob> JobFinishedEvent;

    public event Action<DownloadJob> JobStartedEvent;

    public event Action<DownloadJob> JobQueuedEvent;
    #endregion

    #region Methods
    public void AddJob(DownloadJob job)
    {
      this.QueueJob(job);

      Application.Current.Dispatcher.Invoke(() =>
      {
        this.Jobs.Add(job);
      });
    }

    public void AddJobs(IEnumerable<DownloadJob> jobs)
    {
      foreach (var job in jobs)
      {
        this.QueueJob(job);
        this.Jobs.Add(job);
      }
    }

    public void CancelAllDownloads()
    {
      // Drain the waiting queue - processing queue will empty as the running jobs cancel out
      if (this.waitingJobs.IsEmpty)
      {
        return;
      }

      this.isCancelingAll = true; // Stop any further jobs being pulled from waiting queue

      while (!this.waitingJobs.IsEmpty)
      {
        DownloadJob job;
        if (!this.waitingJobs.TryDequeue(out job))
        {
          Thread.Sleep(50);
          continue;
        }

        job.DownloadCanceled();
        this.JobFinishedEvent?.Invoke(job);
      }
    }

    public void CancelDownload(Object dataContext)
    {
      dataContext.VerifyThatObjectIsNotNull("Parameter 'dataContext' is null.");
      var job = (DownloadJob)dataContext;
      job.CancelDownload();
    }

    /// <summary>
    /// Will start any waiting jobs up to the concurrent maximum
    /// </summary>
    public void StartDownloads()
    {
      while (this.GotWaitingJobs && this.currentDownloads < this.concurrentDownloads)
      {
        this.StartDownload();
      }
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

      this.logger.Message(e.Message);

      Application.Current.Dispatcher.Invoke(() =>
      {
        podcast.Status = DownloadJob.StatusTypes.Failed;
        podcast.ExceptionMessage = e.Message;
      });
    }

    private void QueueJob(DownloadJob job)
    {
      this.JobQueuedEvent?.Invoke(job);
      this.waitingJobs.Enqueue(job);
    }

    private void StartDownload()
    {
      if (this.isCancelingAll)
      {
        return;
      }

      if (this.waitingJobs.IsEmpty)
      {
        // No more podcasts queued so do not start another download.

        if (this.currentDownloads == 0)
        {
          // All current downloads have finished so nothing more to do.
          this.AllJobsFinishedEvent?.Invoke();
        }

        return;
      }

      this.currentDownloads++;

      DownloadJob job;
      while (!this.waitingJobs.TryDequeue(out job))
      {
        if (this.waitingJobs.IsEmpty)
        {
          return;
        }

        Thread.Sleep(50);
      }

      job.InitialiseBeforeDownload();

      Task downloadTask = null;
      try
      {
        downloadTask = Task.Factory.StartNew(() =>
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

      // Definition of 'job started' is when the thread has been created and is ready to run.
      this.JobStartedEvent?.Invoke(job);

      downloadTask.ContinueWith(task =>
      {
        Application.Current.Dispatcher.Invoke(() =>
        {
          job.CancellationCanBeRequested = false;
          // If this job had an unknown file size then the progress bar was marque.
          // Regardless, ensure that the marque effect is turned off.
          job.UseMarqueProgressStyle = false;
        });

        if (task.IsFaulted)
        {
          this.ProcessException(task.Exception, job);
        }
        else if (task.IsCanceled)
        {
          job.DownloadCanceled();
        }
        else
        {
          try
          {
            job.DownloadCompleted();
            this.JobCompletedSuccessfullyEvent?.Invoke(job);
          }
          catch (Exception e)
          {
            this.ProcessException(e, job);
          }
        }

        this.currentDownloads--;

        this.JobFinishedEvent?.Invoke(job);

        this.StartDownload();
      });
    }
    #endregion
  }
}
