
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using Library;

  public class PodcastPageNavigation : PageNavigation<PodcastPageViewModel2, Podcast>
  {
    protected override PodcastPageViewModel2 CreatePage(IList<Podcast> items, Int32 firstIndex, Int32 lastIndex)
    {
      return new PodcastPageViewModel2(items, firstIndex, lastIndex);
    }
  }
}
