
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Collections.Generic;
  using Processing;

  public static class JobFilter
  {
    public static IEnumerable<DownloadJob> FilterJobsByIndex(IList<DownloadJob> jobs, List<Int32> indexes)
    {
      var filteredJobs = new List<DownloadJob>(indexes.Count);
      foreach (var index in indexes)
      {
        filteredJobs.Add(jobs[index]);
      }

      return filteredJobs;
    }
  }
}
