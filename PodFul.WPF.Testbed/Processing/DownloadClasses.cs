
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.Toolkit.Object;
  using Logging;
  using PodFul.Library;
  using ViewModel;

  public interface INewDownloadManager
  {
    Action<DownloadManagerViewModel> JobQueuedEvent { get; set; }
    Action<DownloadManagerViewModel> JobFinishedEvent { get; set; }
    Action<Int32> ProgressEventHandler { get; set; }
    Action<Podcast, String> DownloadCompletedEvent { get; set; }

    void AddJobs(IList<DownloadManagerViewModel> jobViewModels);
    void AddJobs(IList<Int32> podcastIndexes, Feed feed);

    void CancelJobs();

    void StartWaitingJobs();
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
