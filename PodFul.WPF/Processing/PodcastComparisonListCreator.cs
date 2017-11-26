
namespace PodFul.WPF.Processing
{
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
    /// <param name="newPodcasts">List of new podcasts.</param>
    /// <returns>List of podcast comparison objects.</returns>
    public static List<PodcastComparison> Create(Podcast[] currentPodcasts, Podcast[] newPodcasts)
    {
      return Create2(currentPodcasts, newPodcasts);
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
        }
        else
        {
          // Podcasts not equal so add a one-side comparison with a new podcast.
          list.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
          list.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
        }

        currentIndex++;
        newIndex++;
      }

      return list;
    }

    private static List<PodcastComparison> Create2(Podcast[] currentPodcasts, Podcast[] newPodcasts)
    {
      var currentIndex = currentPodcasts.Length - 1;
      var newIndex = newPodcasts.Length - 1;
      var list = new List<PodcastComparison>();

      while (currentIndex > -1 || newIndex > -1)
      {
        if (currentIndex == -1)
        {
          list.Insert(0, PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
          newIndex--;
          continue;
        }

        if (newIndex == -1)
        {
          list.Insert(0, PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
          currentIndex--;
          continue;
        }

        if (currentPodcasts[currentIndex] == newPodcasts[newIndex])
        {
          list.Insert(0, PodcastComparison.CreatePodcastComparisonWithBothPodcasts(currentPodcasts[currentIndex], (currentIndex + 1), newPodcasts[newIndex], (newIndex + 1)));
        }
        else
        {
          list.Insert(0, PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
          list.Insert(0, PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
        }

        currentIndex--;
        newIndex--;
      }

      return list;
    }

    private static List<PodcastComparison> Create3(Podcast[] currentPodcasts, Podcast[] newPodcasts)
    {
      if (currentPodcasts.Length == newPodcasts.Length)
      {
        // Either
      }
      else
      {

      }

      throw new System.NotImplementedException();
    }
  }
}
