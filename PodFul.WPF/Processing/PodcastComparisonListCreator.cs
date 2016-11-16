
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using Library;

  /// <summary>
  /// Creates the list of podcast comparison objects.
  /// </summary>
  public static class PodcastComparisonListCreator
  {
    /// <summary>
    /// Create a list of podcast comparison objects.
    /// </summary>
    /// <param name="currentPodcasts">List of current podcasts.</param>
    /// <param name="newPodcasts">List of new podcasts</param>
    /// <returns></returns>
    public static List<PodcastComparison> Create(Podcast[] currentPodcasts, Podcast[] newPodcasts)
    {
      var currentIndex = 0;
      var newIndex = 0;
      var list = new List<PodcastComparison>();

      while (currentIndex < currentPodcasts.Length || newIndex < newPodcasts.Length)
      {
        if (newIndex == newPodcasts.Length)
        {
          // No more new podcasts so add a one-side comparison with a current podcast.
          list.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
          currentIndex++;
          continue;
        }

        if (currentIndex == currentPodcasts.Length)
        {
          // No more current podcasts so add a one-side comparison with a new podcast.
          list.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
          newIndex++;
          continue;
        }

        if (currentPodcasts[currentIndex] == newPodcasts[newIndex])
        {
          // Podcasts are equal so build comparison with both podcasts.
          list.Add(PodcastComparison.CreatePodcastComparisonWithBothPodcasts(currentPodcasts[currentIndex], (currentIndex + 1), newPodcasts[newIndex], (newIndex + 1)));
          currentIndex++;
          newIndex++;
        }
        else
        {
          // Podcasts not equal so add a one-side comparison with a new podcast.
          list.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
          newIndex++;
        }
      }

      return list;
    }
  }
}
