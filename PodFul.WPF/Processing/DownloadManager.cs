
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
    private UInt32 concurrentDownloads = 1;

    private Int32 currentDownloads;

    private ILogger logger;

    private Queue<DownloadJob> waitingJobs;
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
    public Int32 CancelledJobsCount { get; private set; }

    public Int32 CompletedJobsCount { get; private set; }

    public Int32 FailedJobsCount { get; private set; }

    public Boolean GotIncompleteJobs { get { return this.GotWaitingJobs || this.ProcessingJobsCount > 0; } }

    public Boolean GotWaitingJobs { get { return this.WaitingJobsCount > 0; } }

    public ObservableCollection<DownloadJob> Jobs { get; private set; }

    public Int32 ProcessingJobsCount { get { return this.currentDownloads; } }

    public Int32 WaitingJobsCount { get { return this.waitingJobs.Count; } }
    #endregion

    #region Events
    public event Action AllDownloadsCompletedEvent;

    public event Action<DownloadJob> JobFinishedEvent;

    public event Action<DownloadJob> JobStartedEvent;
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

    /// <summary>
    /// Will start any waiting jobs up to the concurrent maximum
    /// </summary>
    public void StartDownloads()
    {
      while (this.GotWaitingJobs && this.currentDownloads < this.concurrentDownloads)
      {
        StartDownload();
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

      this.logger.Exception(e.Message);

      Application.Current.Dispatcher.Invoke(() =>
      {
        podcast.Status = DownloadJob.StatusTypes.Failed;
        podcast.ExceptionMessage = e.Message;
      });
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

      var job = this.waitingJobs.Dequeue();
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
          job.FileSizeNotKnown = false;
        });

        if (task.IsFaulted)
        {
          this.ProcessException(task.Exception, job);
          this.FailedJobsCount++;
        }
        else if (task.IsCanceled)
        {
          job.DownloadCanceled();
          this.CancelledJobsCount++;
        }
        else
        {
          try
          {
            job.DownloadCompleted();
            this.CompletedJobsCount++;
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
