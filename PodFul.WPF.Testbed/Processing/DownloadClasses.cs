
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.Toolkit.Object;
  using Logging;
  using ViewModel;

  public interface IDownloadManager
  {
    Action<JobViewModel> JobQueuedEvent { get; set; }
    Action<JobViewModel> JobFinishedEvent { get; set; }

    void AddJobs(IList<JobViewModel> jobViewModels);

    void CancelJobs();

    void StartWaitingJobs();
  }

  public interface IDownloadManagerFactory
  {
    IDownloadManager Create();
  }

  public class DownloadManagerFactory : IDownloadManagerFactory
  {
    private ILogger logger;

    public DownloadManagerFactory(ILogger logger)
    {
      logger.VerifyThatObjectIsNotNull("Parameter 'logger' is null.");
      this.logger = logger;
    }

    public IDownloadManager Create()
    {
      return new NewDownloadManager();
    }
  }
}
