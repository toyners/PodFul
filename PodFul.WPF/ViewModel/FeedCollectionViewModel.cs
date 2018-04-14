
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.Threading;
  using System.Windows;
  using Jabberwocky.Toolkit.Object;
  using Logging;
  using PodFul.WPF.Windows;
  using Processing;

  public class FeedCollectionViewModel : IFeedCollectionViewModel
  {
    private enum ImageEvents
    {
      Completed,
      Skip,
      Start,
    }

    #region Fields
    private IFeedProcessor feedProcessor;
    private IImageResolver imageResolver;
    private ILogController logController;
    private Int32 imageDownloadCount;
    private Int32 imageDownloadTotal;
    #endregion

    #region Properties
    public ObservableCollection<TreeViewItemViewModel> Feeds { get; private set; }

    public Action<Int32, String> CompletedImageDownloadNotificationEvent { get; set; }

    public Action<Int32, String> SkippedImageDownloadNotificationEvent { get; set; }

    public Action<Int32, String> StartImageDownloadNotificationEvent { get; set; }

    public Action<Int32> TotalImageDownloadsRequiredEvent { get; set; }
    #endregion

    #region Construction
    public FeedCollectionViewModel(IFeedProcessor feedProcessor, ILogController logController, IImageResolver imageResolver)
    {
      feedProcessor.VerifyThatObjectIsNotNull("Parameter 'feedProcessor' is null.");
      logController.VerifyThatObjectIsNotNull("Parameter 'logController' is null.");

      this.feedProcessor = feedProcessor;
      this.logController = logController;
      this.imageResolver = imageResolver;
      this.Feeds = new ObservableCollection<TreeViewItemViewModel>();

      if (feedProcessor.Feeds != null && feedProcessor.Feeds.Count > 0)
      {
        foreach (var feed in feedProcessor.Feeds)
        {
          this.Feeds.Add(new FeedViewModel(feed));
        }
      }

      if (this.imageResolver != null)
      {
        this.imageResolver.CompletedDownloadNotificationEvent = this.CompletedImageDownloadNotificationEventHandler;
        this.imageResolver.SkippedDownloadNotificationEvent = this.SkippedImageDownloadNotificationEventHandler;
        this.imageResolver.StartDownloadNotificationEvent = this.StartImageDownloadNotificationEventHandler;
        this.imageResolver.TotalDownloadsRequiredEvent = this.TotalImageDownloadsRequiredEventHandler;
      }
    }
    #endregion

    #region Methods
    public void AddFeed(AddFeedToken addFeedToken, CancellationToken cancelToken)
    {
      this.imageDownloadCount = 0;
      var feed = this.feedProcessor.AddFeed(addFeedToken.Directory, addFeedToken.Url, addFeedToken.DefaultPodcastImageFilePath, cancelToken);
      if (this.imageResolver != null)
      {
        this.imageResolver.ResolvePodcastImagesForFeed(feed, cancelToken);
      }

      // Need to add the feed on the UI thread since the feed collection is tied to the UI.
      Application.Current.Dispatcher.Invoke(() =>
      {
        this.Feeds.Add(new FeedViewModel(feed));
      });
    }

    public void RemoveFeed(Int32 index)
    {
      throw new NotImplementedException();
    }

    private void CompletedImageDownloadNotificationEventHandler(Int32 number, String imageFilePath)
    {
      this.logController.GetLogger<FileLogger>(LoggerKeys.InfoKey).Message("[" + imageDownloadCount + " of " + imageDownloadTotal + "]: Completed downloading of \"" + imageFilePath + "\"");
      this.CompletedImageDownloadNotificationEvent?.Invoke(number, imageFilePath);
    }

    private void SkippedImageDownloadNotificationEventHandler(Int32 number, String imageFilePath)
    {
      this.logController.GetLogger<FileLogger>(LoggerKeys.InfoKey).Message("[" + imageDownloadCount + " of " + imageDownloadTotal + "]: Skipped downloading of \"" + imageFilePath + "\"");
      this.SkippedImageDownloadNotificationEvent?.Invoke(number, imageFilePath);
    }

    private void StartImageDownloadNotificationEventHandler(Int32 number, String filePath)
    {
      this.imageDownloadCount++;
      this.StartImageDownloadNotificationEvent?.Invoke(number, filePath);
    }

    private void TotalImageDownloadsRequiredEventHandler(Int32 total)
    {
      this.imageDownloadTotal = total;
      this.TotalImageDownloadsRequiredEvent?.Invoke(total);
    }
    #endregion
  }
}
