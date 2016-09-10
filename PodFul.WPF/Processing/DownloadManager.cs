﻿
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Threading.Tasks;
  using System.Windows;
  using Library;

  public class DownloadManager
  {
    #region Fields
    private Feed feed;
    private FeedCollection feedCollection; //TODO: Is this needed? Maybe only the feed
    private IFileDeliverer fileDeliverer;
    private ILogger logger;
    private IImageResolver imageResolver;

    private Queue<PodcastMonitor> podcasts;

    private Int32 threadCount = 1;

    private Int32 currentDownloads = 0;
    #endregion

    #region Construction
    public DownloadManager(FeedCollection feedCollection, Feed feed, List<Int32> podcastIndexes, IImageResolver imageResolver, IFileDeliverer fileDeliverer, ILogger logger)
    {
      this.feedCollection = feedCollection;
      this.feed = feed;
      this.imageResolver = imageResolver;
      this.fileDeliverer = fileDeliverer;
      this.logger = logger;

      this.podcasts = new Queue<PodcastMonitor>();
      this.Podcasts = new ObservableCollection<PodcastMonitor>();
      foreach (var index in podcastIndexes)
      {
        var podcast = feed.Podcasts[index];
        var podcastMonitor = new PodcastMonitor(podcast, podcast.FileDetails.FileSize, feed.Directory);

        this.podcasts.Enqueue(podcastMonitor);
        this.Podcasts.Add(podcastMonitor);
      }
    }
    #endregion

    #region Properties
    public ObservableCollection<PodcastMonitor> Podcasts { get; private set; } //TODO - no adding or removing so could just use a list<T> instead
    #endregion

    public Action AllDownloadsCompleted;

    #region Methods
    public void CancelAllDownloads()
    {
      foreach (var podcast in this.Podcasts)
      {
        podcast.CancelDownload();
      }
    }

    public void CancelDownload(Object dataContext)
    {
      var podcast = (PodcastMonitor)dataContext;
      podcast.CancelDownload();
    }

    public void StartDownloads()
    {
      for (Int32 i = 0; i < this.threadCount; i++)
      {
        StartDownload();
      }
    }

    private void StartDownload()
    {
      if (this.podcasts.Count == 0 && this.currentDownloads == 0)
      {
        AllDownloadsCompleted?.Invoke();
        return;
      }

      this.currentDownloads++;

      Task task = null;
      var podcast = this.podcasts.Dequeue();
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
            //TODO: Turn on podcast delivery -> podcast.DeliverPodcastFile(this.fileDeliverer, fileInfo.FullName);
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
