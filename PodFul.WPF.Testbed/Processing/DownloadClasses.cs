
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
    Action<PodcastViewModel> DownloadStartingEvent { get; set; }
    Action<Int32> DownloadProgressEventHandler { get; set; }
    event Action DownloadCompletedEvent;

    void AddJobs(IList<Int32> podcastIndexes, Feed feed);
    void AddJobs(IList<PodcastViewModel> podcastViewModels);

    void CancelJobs();

    void CompleteJobs();
    void DownloadPodcast(PodcastViewModel podcastViewModel);
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
