
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
      return Create3(currentPodcasts, newPodcasts);
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
        var podcastComparisons = new List<PodcastComparison>();

        // Find the first match
        var foundFirstMatch = false;
        var currentIndex = 0;
        var newIndex = 0;
        for (; currentIndex < currentPodcasts.Length; currentIndex++)
        {
          newIndex = 0;
          for (; newIndex < newPodcasts.Length; newIndex++)
          {
            if (currentPodcasts[currentIndex] == newPodcasts[newIndex])
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

        if (!foundFirstMatch)
        {
          // No matches
          for (var index = 0; index < currentPodcasts.Length; index++)
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[index], (index + 1)));
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[index], (index + 1)));
          }

          return podcastComparisons;
        }

        // All new podcasts before are not matched in the current list. Put them in now
        var ci = 0;
        var ni = 0;
        while (ci < currentIndex || ni < newIndex)
        {
          if (ni < newIndex)
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[ni], (ni + 1)));
            ni++;
          }

          if (ci < currentIndex)
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[ci], (ci + 1)));
            ci++;
          }
        }

        // Found first match between lists - use that as an anchor point for further comparison checked
        podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithBothPodcasts(currentPodcasts[currentIndex], (currentIndex + 1), newPodcasts[newIndex], (newIndex + 1)));
        currentIndex++;
        newIndex++;

        // Now walk along both lists. Where current and new does not match then add new only and advance along new list
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
              // No match - add new comparison only and current comparison only
              podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
              podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
              currentIndex++;
              newIndex++;
            }
          }
          else if (newIndex >= newPodcasts.Length)
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
            currentIndex++;
          }
          else
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
            newIndex++;
          }
        }

        return podcastComparisons;
      }

      if (currentPodcasts.Length < newPodcasts.Length)
      {
        var podcastComparisons = new List<PodcastComparison>();

        // Find the first match
        var foundFirstMatch = false;
        var currentIndex = 0;
        var newIndex = 0;
        for (; currentIndex < currentPodcasts.Length; currentIndex++)
        {
          newIndex = 0;
          for (; newIndex < newPodcasts.Length; newIndex++)
          {
            if (currentPodcasts[currentIndex] == newPodcasts[newIndex])
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

        if (!foundFirstMatch)
        {
          // No matches
          newIndex = 0;
          currentIndex = 0;
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

        // All new podcasts before are not matched in the current list. Put them in now
        var ci = 0;
        var ni = 0;
        while (ci < currentIndex || ni < newIndex)
        {
          if (ni < newIndex)
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[ni], (ni + 1)));
            ni++;
          }

          if (ci < currentIndex)
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[ci], (ci + 1)));
            ci++;
          }
        }

        // Found first match between lists - use that as an anchor point for further comparison checked
        podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithBothPodcasts(currentPodcasts[currentIndex], (currentIndex + 1), newPodcasts[newIndex], (newIndex + 1)));
        currentIndex++;
        newIndex++;

        // Now walk along both lists. Where current and new does not match then add new only and advance along new list
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
              // No match - add new comparison only and current comparison only
              podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
              podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
              currentIndex++;
              newIndex++;
            }
          }
          else if (newIndex >= newPodcasts.Length)
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithCurrentPodcastOnly(currentPodcasts[currentIndex], (currentIndex + 1)));
            currentIndex++;
          }
          else
          {
            podcastComparisons.Add(PodcastComparison.CreatePodcastComparisonWithNewPodcastOnly(newPodcasts[newIndex], (newIndex + 1)));
            newIndex++;
          }
        }

        return podcastComparisons;
      }
      
      throw new System.NotImplementedException();
    }
  }
}
