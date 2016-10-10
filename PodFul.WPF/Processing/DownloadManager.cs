
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

    public Action AllDownloadsCompleted;

    public Action<DownloadJob> JobAdded;

    public Action JobsAdded;

    #region Methods
    public void AddJob(DownloadJob job)
    {
      this.waitingJobs.Enqueue(job);
      this.Jobs.Add(job);

      this.JobAdded?.Invoke(job);
    }

    public void AddJobs(IEnumerable<DownloadJob> jobs)
    {
      foreach (var job in jobs)
      {
        this.waitingJobs.Enqueue(job);
      }

      this.JobsAdded?.Invoke();
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
          AllDownloadsCompleted?.Invoke();
        }

        return;
      }

      this.currentDownloads++;

      Task task = null;
      var podcast = this.waitingJobs.Dequeue();
      podcast.InitialiseBeforeDownload();
      try
      {
        task = Task.Factory.StartNew(() =>
        {
          var downloader = new FileDownloader();
          downloader.Download(podcast.URL, podcast.FilePath, podcast.CancellationToken, podcast.ProgressEventHandler);
        }, podcast.CancellationToken);
      }
      catch (Exception e)
      {
        // Catch any exceptions regarding the setup of the task. May not be necessary
        this.ProcessException(e, podcast);
        return;
      }

      task.ContinueWith(t =>
      {
        Application.Current.Dispatcher.Invoke(() => { podcast.CancellationCanBeRequested = false; });

        if (t.Exception != null)
        {
          this.ProcessException(t.Exception, podcast);
        }
        else if (t.IsCanceled)
        {
          podcast.DownloadCanceled();
        }
        else
        {
          try
          {
            podcast.DownloadCompleted();
          }
          catch (Exception e)
          {
            this.ProcessException(e, podcast);
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
