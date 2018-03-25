
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Collections.Generic;
  using Library;
  using Processing;

  public static class JobCollectionFactory
  {
    public static IList<DownloadJob> FilterJobsByIndex(IList<DownloadJob> jobs, List<Int32> indexes)
    {
      var filteredJobs = new List<DownloadJob>(indexes.Count);
      foreach (var index in indexes)
      {
        filteredJobs.Add(jobs[index]);
      }

      return filteredJobs;
    }

    public static IList<DownloadJob> CreateJobsFromSelectedIndexesOfFeed(Feed feed, IEnumerable<Int32> indexes, IFeedCollection feedCollection, IImageResolver imageResolver)
    {
      List<DownloadJob> jobs = new List<DownloadJob>();
      foreach (var index in indexes)
      {
        var podcast = feed.Podcasts[index];
        var downloadJob = new DownloadJob(podcast, feed, feedCollection, imageResolver);

        jobs.Add(downloadJob);
      }

      return jobs;
    }
  }
}
