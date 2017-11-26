
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
      var podcastComparisons = new List<PodcastComparison>();

      // Find the first match
      var foundFirstMatch = false;
      var firstCurrentMatchIndex = 0;
      var firstNewMatchIndex = 0;
      for (; firstCurrentMatchIndex < currentPodcasts.Length; firstCurrentMatchIndex++)
      {
        firstNewMatchIndex = 0;
        for (; firstNewMatchIndex < newPodcasts.Length; firstNewMatchIndex++)
        {
          if (currentPodcasts[firstCurrentMatchIndex] == newPodcasts[firstNewMatchIndex])
          {
            // found match
            foundFirstMatch = true;
            break;
          }
        }

        if (foundFirstMatch)
        {
          break;
        }
      }

      var currentIndex = 0;
      var newIndex = 0;
      if (!foundFirstMatch)
      {
        while (newIndex < newPodcasts.Length || currentIndex < currentPodcasts.Length)
        {
          if (newIndex < newPodcasts.Length)
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
            newIndex++;
          }

          if (currentIndex < currentPodcasts.Length)
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
            currentIndex++;
          }
        }

        return podcastComparisons;
      }

      // Place podcasts before the first match into the list in an interleaved style.
      while (currentIndex < firstCurrentMatchIndex || newIndex < firstNewMatchIndex)
      {
        if (newIndex < firstNewMatchIndex)
        {
          podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
          newIndex++;
        }

        if (currentIndex < firstCurrentMatchIndex)
        {
          podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
          currentIndex++;
        }
      }

      // Place first match
      podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithBothPodcasts(currentPodcasts[currentIndex], (currentIndex + 1), newPodcasts[newIndex], (newIndex + 1)));
      currentIndex++;
      newIndex++;

      // Advance along both lists. Where current and new does not match then add two comparison rows.
      while (currentIndex < currentPodcasts.Length || newIndex < newPodcasts.Length)
      {
        if (currentIndex < currentPodcasts.Length && newIndex < newPodcasts.Length)
        {
          if (currentPodcasts[currentIndex] == newPodcasts[newIndex])
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithBothPodcasts(currentPodcasts[currentIndex], (currentIndex + 1), newPodcasts[newIndex], (newIndex + 1)));
            currentIndex++;
            newIndex++;
          }
          else
          {
            // No match
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
            currentIndex++;
            newIndex++;
          }
        }
        else if (newIndex >= newPodcasts.Length)
        {
          // New index is exhausted
          podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
          currentIndex++;
        }
        else
        {
          // Current index is exhausted
          podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
          newIndex++;
        }
      }

      return podcastComparisons;
    }
  }
}
