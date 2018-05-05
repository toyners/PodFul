
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using Jabberwocky.Toolkit.Object;
  using Logging;
  using ViewModel;

  public interface IDownloadManager
  {
    void AddJobs(IEnumerable<JobViewModel> jobViewModels);

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

  public class MockLogger : ILogger
  {
    public void Message(String message)
    {
      throw new NotImplementedException();
    }
  }
}
