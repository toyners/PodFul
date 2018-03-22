
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using FileDelivery;
  using Jabberwocky.Toolkit.Object;
  using Library;
  using Logging;

  public class DownloadManager : IDownloadManager
  {
    #region Fields
    private UInt32 concurrentDownloads = 1;
    private Int32 currentDownloads;
    private ILogger logger;
    private ConcurrentQueue<DownloadJob> waitingJobs;
    private Boolean treatJobsWithMissingFileNamesAsFailed;
    #endregion

    #region Construction
    public static IDownloadManager Create(ILogger combinedLogger, UInt32 concurrentDownloadCount, Func<DownloadJob, Boolean> jobNeedsLocationEventHandler, IFileDeliverer fileDeliverer = null)
    {
      Boolean treatJobsWithMissingFileNamesAsFailed = (jobNeedsLocationEventHandler == null);
      var downloadManager = new DownloadManager(combinedLogger, concurrentDownloadCount, treatJobsWithMissingFileNamesAsFailed);

      if (fileDeliverer != null)
      {
        downloadManager.JobCompletedSuccessfullyEvent += job =>
        {
          if (job.DoDeliverFile)
          {
            fileDeliverer.DeliverFileToDeliveryPoints(job.FilePath, job.Name);
          }
        };
      }

      if (!treatJobsWithMissingFileNamesAsFailed)
      {
        downloadManager.JobNeedsLocationEvent = jobNeedsLocationEventHandler;
      }

      return downloadManager;
    } 

    public DownloadManager(ILogger logger, UInt32 concurrentDownloads, Boolean treatJobsWithMissingFileNamesAsFailed)
    {
      logger.VerifyThatObjectIsNotNull("Parameter 'exceptionLogger' is null.");
      this.logger = logger;
      this.concurrentDownloads = concurrentDownloads;

      this.waitingJobs = new ConcurrentQueue<DownloadJob>();

      this.Jobs = new ObservableCollection<DownloadJob>();

      this.treatJobsWithMissingFileNamesAsFailed = treatJobsWithMissingFileNamesAsFailed;
    }
    #endregion

    #region Properties
    public IList<DownloadJob> FailedJobs { get; private set; }

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
    public event Func<DownloadJob, Boolean> JobNeedsLocationEvent;
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

    public void CancelAllJobs()
    {
      // Drain the waiting queue
      while (!this.waitingJobs.IsEmpty)
      {
        DownloadJob job;
        if (!this.waitingJobs.TryDequeue(out job))
        {
          Thread.Sleep(50);
          continue;
        }
      }

      // Cancel any running jobs
      foreach (var job in this.Jobs)
      {
        if (job.Status == DownloadJob.StatusTypes.Waiting || job.Status == DownloadJob.StatusTypes.Running)
        {
          this.CancelJob(job);
        }
      }
    }

    public void CancelJob(DownloadJob job)
    {
      job.VerifyThatObjectIsNotNull("Parameter 'job' is null.");

      var jobNotStarted = (job.Status == DownloadJob.StatusTypes.Waiting);

      job.CancelDownload();

      if (jobNotStarted)
      {
        this.JobFinishedEvent?.Invoke(job);
      }
    }

    /// <summary>
    /// Will start any waiting jobs up to the concurrent maximum
    /// </summary>
    public void StartWaitingJobs()
    {
      while (this.GotWaitingJobs && this.currentDownloads < this.concurrentDownloads)
      {
        this.StartDownload();
      }
    }

    private void AddJobToFailedJobsList(DownloadJob job)
    {
      if (this.FailedJobs == null)
      {
        this.FailedJobs = new List<DownloadJob>();
      }

      this.FailedJobs.Add(job);
    }

    private DownloadJob GetNextJobToProcess()
    {
      DownloadJob job = null;
      while (!this.waitingJobs.TryDequeue(out job))
      {
        if (this.waitingJobs.IsEmpty)
        {
          return null;
        }

        Thread.Sleep(50);
      }

      return job;
    }

    private void ProcessException(Exception exception, DownloadJob job)
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

      this.AddJobToFailedJobsList(job);

      Application.Current.Dispatcher.Invoke(() =>
      {
        job.Status = DownloadJob.StatusTypes.Failed;
        job.ExceptionMessage = e.Message;
        job.CancellationVisibility = Visibility.Hidden;
      });
    }

    private void QueueJob(DownloadJob job)
    {
      this.JobQueuedEvent?.Invoke(job);
      this.waitingJobs.Enqueue(job);
    }

    private void StartDownload()
    {
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

      DownloadJob job = this.GetNextJobToProcess();
      if (job == null)
      {
        // Waiting queue is empty.
        return;
      }

      if (job.Status == DownloadJob.StatusTypes.NoLocation)
      {
        if (this.treatJobsWithMissingFileNamesAsFailed)
        {
          job.Status = DownloadJob.StatusTypes.Failed;
        }
        else if (this.JobNeedsLocationEvent != null)
        {
          Application.Current.Dispatcher.Invoke(() =>
          {
            var continueDownload = this.JobNeedsLocationEvent.Invoke(job);
            job.Status = (continueDownload ? DownloadJob.StatusTypes.Waiting : DownloadJob.StatusTypes.Cancelled);
          });
        }
      }

      if (job.Status != DownloadJob.StatusTypes.Waiting)
      {
        // Job is cancelled or failed.
        if (job.Status == DownloadJob.StatusTypes.Failed)
        {
          this.AddJobToFailedJobsList(job);
        }

        return;
      }

      this.currentDownloads++;

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
          // If this job had an unknown file size then the progress bar was marque.
          // Regardless, ensure that the marque effect is turned off.
          job.UseMarqueProgressStyle = false;
        });

        if (task.IsFaulted)
        {
          this.ProcessException(task.Exception, job);
          this.FailedJobs.Add(job);
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
