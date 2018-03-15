
namespace PodFul.WPF.Processing
{
  using System;

  /// <summary>
  /// Interface for setting handlers for download manager events.
  /// </summary>
  public interface IDownloadManager : ISimpleDownloadManager
  {
    event Action AllJobsFinishedEvent;

    event Action<DownloadJob> JobCompletedSuccessfullyEvent;

    event Action<DownloadJob> JobFinishedEvent;

    event Action<DownloadJob> JobStartedEvent;

    event Action<DownloadJob> JobQueuedEvent;
  }
}