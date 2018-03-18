
namespace PodFul.WPF.Processing
{
  using System;

  /// <summary>
  /// Interface for managing download events.
  /// </summary>
  public interface IDownloadEvents
  {
    /// <summary>
    /// Event raised when all jobs have finished (completed, failed or canceled).
    /// </summary>
    event Action AllJobsFinishedEvent;

    /// <summary>
    /// Event raised when job finished successfully.
    /// </summary>
    event Action<DownloadJob> JobCompletedSuccessfullyEvent;

    /// <summary>
    /// Event raised when job has finished (completed, failed or canceled).
    /// </summary>
    event Action<DownloadJob> JobFinishedEvent;

    event Func<DownloadJob, Boolean> JobNeedsLocationEvent;

    /// <summary>
    /// Event raised when job moves from waiting to running.
    /// </summary>
    event Action<DownloadJob> JobStartedEvent;

    /// <summary>
    /// Event raised when job is queued on download manager (i.e. added).
    /// </summary>
    event Action<DownloadJob> JobQueuedEvent;
  }
}