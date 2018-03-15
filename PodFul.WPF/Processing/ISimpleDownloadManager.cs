
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.ObjectModel;

  /// <summary>
  /// Simple interface for managing download jobs
  /// </summary>
  public interface ISimpleDownloadManager
  {
    /// <summary>
    /// True if the download manager has running jobs or waiting jobs. Otherwise false.
    /// </summary>
    Boolean GotIncompleteJobs { get; }

    /// <summary>
    /// Observable collection of all the jobs in the download manager.
    /// </summary>
    ObservableCollection<DownloadJob> Jobs { get; }

    /// <summary>
    /// Add a job to the download manager.
    /// </summary>
    /// <param name="job">Job to add.</param>
    void AddJob(DownloadJob job);

    /// <summary>
    /// Cancel all running jobs and waiting jobs.
    /// </summary>
    void CancelAllJobs();

    /// <summary>
    /// Cancel a job.
    /// </summary>
    /// <param name="job">Job to cancel.</param>
    void CancelJob(DownloadJob job);

    /// <summary>
    /// Start any waiting jobs.
    /// </summary>
    void StartWaitingJobs();
  }
}
