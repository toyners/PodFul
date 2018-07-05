
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using PodFul.Library;
  using PodFul.WPF.Testbed.ViewModel;

  public class NewDownloadManager : INewDownloadManager
  {
    private CancellationTokenSource cancellationTokenSource;
    private IList<PodcastViewModel> podcastViewModels;

    public Int32 Count { get; private set; }

    public Action<Int32> DownloadProgressEventHandler { get; set; }
    public event Action DownloadCompletedEvent;
    public Action<PodcastViewModel> DownloadStartingEvent { get; set; }

    public void AddJobs(IList<PodcastViewModel> podcastViewModels)
    {
      this.podcastViewModels = new List<PodcastViewModel>(podcastViewModels);
      this.Count = this.podcastViewModels.Count;
    }

    public void CancelJobs()
    {
      if (this.cancellationTokenSource != null && !this.cancellationTokenSource.IsCancellationRequested)
      {
        this.cancellationTokenSource.Cancel();
      }
    }

    public void CompleteJobs()
    {
      this.cancellationTokenSource = new CancellationTokenSource();
      var cancelToken = this.cancellationTokenSource.Token;
      var fileDownloader = new FileDownloader();

      foreach (var podcastViewModel in this.podcastViewModels)
      {
        this.DownloadStartingEvent?.Invoke(podcastViewModel);
        podcastViewModel.Download(fileDownloader, cancelToken, this.DownloadProgressEventHandler);
        this.DownloadCompletedEvent?.Invoke();
      }
    }

    public void DownloadPodcast(PodcastViewModel podcastViewModel)
    {
      this.cancellationTokenSource = new CancellationTokenSource();
      var cancelToken = this.cancellationTokenSource.Token;
      var fileDownloader = new FileDownloader();

      this.DownloadStartingEvent?.Invoke(podcastViewModel);
      podcastViewModel.Download(fileDownloader, cancelToken);
      this.DownloadCompletedEvent?.Invoke();
    }
  }
}
