
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Threading.Tasks;
  using System.Windows;
  using Jabberwocky.Toolkit.Object;
  using Library;

  public class DownloadManager
  {
    #region Fields
    private Feed feed;
    private FeedCollection feedCollection; //TODO: Is this needed? Maybe only the feed
    private IFileDeliverer fileDeliverer;
    private ILogger logger;
    private IImageResolver imageResolver;

    private Queue<PodcastMonitor> waitingJobs;

    private UInt32 concurrentDownloads = 1;

    private Int32 currentDownloads = 0;
    #endregion

    #region Construction
    public DownloadManager(FeedCollection feedCollection, Feed feed, List<Int32> podcastIndexes, IImageResolver imageResolver, IFileDeliverer fileDeliverer, ILogger logger, UInt32 concurrentDownloads)
    {
      feedCollection.VerifyThatObjectIsNotNull("Parameter 'feedCollection' is null.");
      feed.VerifyThatObjectIsNotNull("Parameter 'feed' is null.");
      podcastIndexes.VerifyThatObjectIsNotNull("Parameter 'podcastIndexes' is null.");
      imageResolver.VerifyThatObjectIsNotNull("Parameter 'imageResolver' is null.");
      logger.VerifyThatObjectIsNotNull("Parameter 'logger' is null.");

      this.feedCollection = feedCollection;
      this.feed = feed;
      this.imageResolver = imageResolver;
      this.fileDeliverer = fileDeliverer;
      this.logger = logger;
      this.concurrentDownloads = concurrentDownloads;

      this.waitingJobs = new Queue<PodcastMonitor>();

      this.Jobs = new ObservableCollection<PodcastMonitor>();

      this.LoadPodcastMonitors(podcastIndexes);
    }
    #endregion

    #region Properties
    public ObservableCollection<PodcastMonitor> Jobs { get; private set; }

    public Boolean GotIncompleteJobs { get { return this.waitingJobs.Count > 0; } }
    #endregion

    public Action AllDownloadsCompleted;

    public Action<PodcastMonitor> JobAdded; 

    #region Methods
    public void AddJob(PodcastMonitor job)
    {
      this.waitingJobs.Enqueue(job);
      this.Jobs.Add(job);

      this.JobAdded?.Invoke(job);
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
      var podcast = (PodcastMonitor)dataContext;
      podcast.CancelDownload();
    }

    public void StartDownloads()
    {
      while (this.currentDownloads < this.concurrentDownloads)
      {
        StartDownload();
      }
    }

    private void LoadPodcastMonitors(List<Int32> podcastIndexes)
    {
      foreach (var index in podcastIndexes)
      {
        var podcast = feed.Podcasts[index];
        var podcastMonitor = new PodcastMonitor(podcast, podcast.FileDetails.FileSize, feed.Directory);

        this.waitingJobs.Enqueue(podcastMonitor);
        this.Jobs.Add(podcastMonitor);
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
          if (File.Exists(podcast.FilePath))
          {
            File.Delete(podcast.FilePath);
          }

          podcast.DownloadCanceled();
        }
        else
        {
          var fileInfo = new FileInfo(podcast.FilePath);
          if (!fileInfo.Exists)
          {
            this.ProcessException(new FileNotFoundException(String.Format("Podcast file '{0}' is missing.", podcast.FilePath)), podcast);
          }
          else
          {
            podcast.SetPodcastFileDetails(this.imageResolver, fileInfo.Length);
            if (this.fileDeliverer != null)
            {
              podcast.DeliverPodcastFile(this.fileDeliverer, fileInfo.FullName);
            }

            podcast.DownloadCompleted();

            this.feedCollection.UpdateFeed(feed);
          }
        }

        this.currentDownloads--;

        this.StartDownload();
      });
    }

    private void ProcessException(Exception exception, PodcastMonitor podcast)
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
        podcast.Status = PodcastMonitor.StatusTypes.Failed;
        podcast.ExceptionMessage = e.Message;
      });
    }
    #endregion
  }
}
