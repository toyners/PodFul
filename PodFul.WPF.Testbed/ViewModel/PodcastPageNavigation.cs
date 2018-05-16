
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using Library;

  public class PodcastPageNavigation : PageNavigation<PodcastPageViewModel, Podcast>
  {
    protected override PodcastPageViewModel CreatePage(IList<Podcast> items, Int32 firstIndex, Int32 lastIndex)
    {
      return new PodcastPageViewModel(items, firstIndex, lastIndex);
    }
  }
}
