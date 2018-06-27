
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.Toolkit.Object;
  using Logging;
  using PodFul.Library;
  using PodFul.WPF.Testbed.ViewModel;

  public interface INewDownloadManager
  {
    Int32 Count { get; }
    Action<Podcast> DownloadStartingEvent { get; set; }
    Action<Int32> DownloadProgressEventHandler { get; set; }
    event Action<Podcast> DownloadCompletedEvent;

    void AddJobs(IList<Int32> podcastIndexes, Feed feed);

    void DownloadPodcast(PodcastViewModel podcastViewModel);

    void CancelJobs();

    void DownloadPodcasts();
  }

  public interface IDownloadManagerFactory
  {
    INewDownloadManager Create();
  }

  public class DownloadManagerFactory : IDownloadManagerFactory
  {
    private ILogger logger;

    public DownloadManagerFactory(ILogger logger)
    {
      logger.VerifyThatObjectIsNotNull("Parameter 'logger' is null.");
      this.logger = logger;
    }

    public INewDownloadManager Create()
    {
      return new NewDownloadManager();
    }
  }
}
