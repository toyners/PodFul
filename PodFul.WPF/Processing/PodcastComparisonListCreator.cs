﻿
namespace PodFul.WPF.Processing
{
  using System;
  using System.Collections.Generic;
  using Library;

  public static class PodcastComparisonListCreator
  {
    public static List<PodcastComparison> Create(Podcast[] oldPodcasts, Podcast[] newPodcasts)
    {
      var oldIndex = 0;
      var newIndex = 0;
      var list = new List<PodcastComparison>();

      while (oldIndex < oldPodcasts.Length && newIndex < newPodcasts.Length)
      {
        if (newIndex == newPodcasts.Length)
        {
          list.Add(new PodcastComparison(oldPodcasts[oldIndex], (oldIndex + 1), false));
          oldIndex++;
          continue;
        }

        if (oldIndex == oldPodcasts.Length)
        {
          list.Add(new PodcastComparison(newPodcasts[newIndex], (newIndex + 1), true));
          newIndex++;
          continue;
        }

        if (oldPodcasts[oldIndex] == newPodcasts[newIndex])
        {
          list.Add(new PodcastComparison(oldPodcasts[oldIndex], (oldIndex + 1), newPodcasts[newIndex], (newIndex + 1)));
          oldIndex++;
          newIndex++;
        }
        else
        {
          list.Add(new PodcastComparison(newPodcasts[newIndex], (newIndex + 1), true));
          newIndex++;
        }
      }

      return list;
    }
  }
}
