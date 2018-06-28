
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using Library;

  public class PodcastPageNavigation : PageNavigation<PodcastPageViewModel, Podcast>
  {
    private FeedViewModel feedViewModel;
    public PodcastPageNavigation(FeedViewModel feedViewModel) => this.feedViewModel = feedViewModel;
   
    protected override PodcastPageViewModel CreatePage(IList<Podcast> items, Int32 firstIndex, Int32 lastIndex)
    {
      return new PodcastPageViewModel(this.feedViewModel, items, firstIndex, lastIndex);
    }
  }
}
