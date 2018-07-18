
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.Generic;
  using Library;
  using PodFul.WPF.Processing.TileView;

  public class PodcastPageViewModel
  {
    public PodcastPageViewModel(FeedViewModel feedViewModel, IList<Podcast> podcasts, Int32 firstPodcastIndex, Int32 lastPodcastIndex, IFileDownloadProxyFactory fileDownProxyFactory)
    {
      this.PodcastViewModels = new List<PodcastViewModel>(lastPodcastIndex - firstPodcastIndex + 1);
      while (firstPodcastIndex <= lastPodcastIndex)
      {
        this.PodcastViewModels.Add(new PodcastViewModel(feedViewModel, podcasts[firstPodcastIndex++], fileDownProxyFactory));
      }
    }

    public List<PodcastViewModel> PodcastViewModels { get; private set; }
  }
}
