
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using Library;

  public class PodcastViewModel
  {
    public PodcastViewModel(Podcast podcast)
    {
      this.Title = podcast.Title;
    }

    public String Title { get; private set; }
  }
}
