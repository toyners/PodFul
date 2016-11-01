
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using Library;

  public static class PodcastComparisonListCreator
  {
    public static List<PodcastComparison> Create(Podcast[] oldPodcasts, Podcast[] newPodcasts)
    {
      var firstOldPodcast = oldPodcasts[0];

      // Find the first new podcast that matches the first old podcast
      Int32 firstMatchIndex = 0;
      for(; firstMatchIndex < newPodcasts.Length; firstMatchIndex++)
      {
        if (firstOldPodcast == newPodcasts[firstMatchIndex])
        {
          break;
        }
      }

      var lastNewPodcast = newPodcasts[newPodcasts.Length - 1];

      // Find the last new podcast that matches the last old podcast
      Int32 lastMatchIndex = oldPodcasts.Length;
      for (; lastMatchIndex >= 0; lastMatchIndex--)
      {
        if (lastNewPodcast.Equals(oldPodcasts[lastMatchIndex]))
        {
          break;
        }
      }

      throw new NotImplementedException();
    }
  }
}
