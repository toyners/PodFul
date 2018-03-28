
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;

  /// <summary>
  /// Interface for managing download jobs
  /// </summary>
  public interface IDownloadFunctionality
  {
    #region Properties
    /// <summary>
    /// List of all failed jobs in the download manager.
    /// </summary>
    IList<DownloadJob> FailedJobs { get; }

    /// <summary>
    /// True it the download manager has failed jobs. Otherwise false.
    /// </summary>
    Boolean GotFailedJobs { get; }

    /// <summary>
    /// True if the download manager has running jobs or waiting jobs. Otherwise false.
    /// </summary>
    Boolean GotIncompleteJobs { get; }

    /// <summary>
    /// Observable collection of all the jobs in the download manager.
    /// </summary>
    ObservableCollection<DownloadJob> Jobs { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Add a job to the download manager.
    /// </summary>
    /// <param name="job">Job to add.</param>
    void AddJob(DownloadJob job);

    /// <summary>
    /// Add a collection of jobs to the download manager.
    /// </summary>
    /// <param name="jobs"></param>
    void AddJobs(IEnumerable<DownloadJob> jobs);

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
    #endregion
  }
}
