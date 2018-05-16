
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using Library;

  public class PodcastPageViewModel
  {
    public PodcastPageViewModel(IList<Podcast> podcasts, Int32 firstPodcastIndex, Int32 lastPodcastIndex)
    {
      this.Podcasts = new List<PodcastViewModel>(lastPodcastIndex - firstPodcastIndex + 1);
      while (firstPodcastIndex <= lastPodcastIndex)
      {
        this.Podcasts.Add(new PodcastViewModel(podcasts[firstPodcastIndex++]));
      }
    }

    public List<PodcastViewModel> Podcasts { get; private set; }
  }
}
