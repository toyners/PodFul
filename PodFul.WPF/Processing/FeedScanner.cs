﻿
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Threading;
  using System.Threading.Tasks;
  using System.Windows;
  using Jabberwocky.Toolkit.String;
  using Library;
  using Processing;

  /// <summary>
  /// Scans a list of feeds and collects a list of podcasts to be downloaded. Feeds with new podcasts are updated.
  /// </summary>
  public class FeedScanner
  {
    #region Fields
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private DownloadManager downloadManager;

    private FeedCollection feedCollection;

    private IFileDeliverer fileDeliverer;

    private IImageResolver imageResolver;

    private Queue<Int32> indexes;

    private ILogger logger;
    #endregion

    #region Construction
    public FeedScanner(
      FeedCollection feedCollection,
      Queue<Int32> feedIndexes,
      IImageResolver imageResolver,
      IFileDeliverer fileDeliverer,
      ILogger logger,
      DownloadManager downloadManager)
    {
      this.feedCollection = feedCollection;
      this.indexes = feedIndexes;
      this.imageResolver = imageResolver;
      this.fileDeliverer = fileDeliverer;
      this.logger = logger;
      this.downloadManager = downloadManager;
    }
    #endregion

    #region Properties
    public ObservableCollection<DownloadJob> Jobs { get { return this.downloadManager.Jobs; } }
    #endregion

    #region Events
    public Action<String> SetWindowTitleEvent;

    public Action<Boolean> SetCancelButtonStateEvent;
    #endregion

    #region Methods
    public void Cancel()
    {
      this.cancellationTokenSource.Cancel();
    }

    public void Process()
    {
      var feedTotal = (UInt32)this.indexes.Count;
      var title = "Scanning " + feedTotal + " feed".Pluralize(feedTotal);
      this.SetWindowTitleEvent?.Invoke(title);

      var cancelToken = this.cancellationTokenSource.Token;

      // Set this outside of the scanning task because there is no guarantee that the scanning task
      // will start before the downloading taks.
      var isScanning = true;

      Task scanningTask = Task.Factory.StartNew(() =>
      {
        this.SetCancelButtonStateEvent?.Invoke(true);
        var podcastIndexes = new List<Int32>();
        String scanReport = null;

        while (indexes.Count > 0)
        {
          Int32 feedIndex = indexes.Dequeue();

          title = "Scanning " + (feedTotal - indexes.Count) + " of " + feedTotal + " feed".Pluralize(feedTotal);
          this.SetWindowTitleEvent?.Invoke(title);

          if (this.cancellationTokenSource.IsCancellationRequested)
          {
            this.logger.Message("\r\nCANCELLED");
            break;
          }

          var feed = this.feedCollection.Feeds[feedIndex];

          this.logger.Message("Scanning \"" + feed.Title + "\".");

          Feed newFeed = null;
          try
          {
            newFeed = FeedFunctions.UpdateFeed(feed, this.imageResolver, cancelToken);

            if (this.cancellationTokenSource.IsCancellationRequested)
            {
              this.logger.Message("CANCELLED");
              break;
            }

            this.logger.Message("Comparing podcasts ... ", false);

            podcastIndexes.Clear();
            if (feed.Podcasts.Length == 0)
            {
              for (Int32 i = 0; i < newFeed.Podcasts.Length; i++)
              {
                podcastIndexes.Add(i);
              }
            }
            else
            {
              Int32 podcastIndex = 0;
              var firstPodcast = feed.Podcasts[0];
              while (podcastIndex < newFeed.Podcasts.Length && !newFeed.Podcasts[podcastIndex].Equals(firstPodcast))
              {
                podcastIndexes.Add(podcastIndex);
                podcastIndex++;
              }
            }

            Boolean downloadPodcasts = (podcastIndexes.Count > 0);
            if (podcastIndexes.Count > 5)
            {
              var text = String.Format("{0} new podcasts found during feed scan.\r\n\r\nYes to continue with downloading.\r\nNo to skip downloading (feed will still be updated).\r\nCancel to stop scanning (feed will not be updated).", podcastIndexes.Count);
              var continuingDownloading = MessageBox.Show(text, "Multiple podcasts found.", MessageBoxButton.YesNoCancel);
              if (continuingDownloading == MessageBoxResult.Cancel)
              {
                var feedReport = podcastIndexes.Count + " podcasts found";
                this.logger.Message(feedReport + " (Scan cancelled).\r\n");
                scanReport += feedReport + " for \"" + feed.Title + "\" (Scan cancelled).";
                break;
              }

              if (continuingDownloading == MessageBoxResult.No)
              {
                downloadPodcasts = false;
              }
            }

            String message = "Complete - ";
            if (podcastIndexes.Count == 0)
            {
              message += "No new podcasts found.";
            }
            else
            {
              newFeed = Feed.SetUpdatedDate(DateTime.Now, newFeed);
              var feedReport = podcastIndexes.Count + " podcast" +
                (podcastIndexes.Count != 1 ? "s" : String.Empty) + " found";
              var downloadingReport = (downloadPodcasts ? String.Empty : " (Downloading skipped)");
              message += feedReport + downloadingReport + ".";
              scanReport += feedReport + " for \"" + feed.Title + "\"" + downloadingReport + ".\r\n";
            }

            this.logger.Message(message);

            this.logger.Message(String.Format("Updating \"{0}\" ... ", feed.Title), false);

            this.FeedScanCompleted(feedIndex, newFeed);

            if (downloadPodcasts)
            {
              podcastIndexes.Reverse();

              foreach (var index in podcastIndexes)
              {
                var podcast = newFeed.Podcasts[index];
                var downloadJob = new DownloadJob(podcast, newFeed, this.feedCollection, this.fileDeliverer, this.imageResolver);
                this.downloadManager.AddJob(downloadJob);
              }
            }

            this.logger.Message(String.Empty);
          }
          catch (Exception exception)
          {
            var exceptionReport = String.Format("EXCEPTION thrown for \"{0}\": {1}\r\n", feed.Title, exception.Message);
            scanReport += exceptionReport;
            this.logger.Message(exceptionReport);
          }
        }

        // Display the final scan report.
        if (scanReport == null)
        {
          this.logger.Message("Nothing to report.");
        }
        else
        {
          this.logger.Message("Scan Report\r\n" + scanReport);
        }

        isScanning = false;
        this.SetCancelButtonStateEvent?.Invoke(false);

      }, cancelToken);

      Task downloadingTask = Task.Factory.StartNew(() =>
      {
        while (isScanning || downloadManager.GotIncompleteJobs)
        {
          downloadManager.StartDownloads();
        }
      }, cancelToken);
    }

    private void FeedScanCompleted(Int32 feedIndex, Feed feed)
    {
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.feedCollection.UpdateFeed(feedIndex, feed);
        this.logger.Message("Completed.");
      });
    }
    #endregion
  }
}
