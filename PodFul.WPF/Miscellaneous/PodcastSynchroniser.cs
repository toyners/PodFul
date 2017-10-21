
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.IO;
  using PodFul.Library;

  public static class PodcastSynchroniser
  {
    public static Int32 Synchronise(Feed feed)
    {
      if (feed.Podcasts == null)
      {
        return 0;
      }

      var syncCount = 0;
      for (int i = 0; i < feed.Podcasts.Length; i++)
      {
        var podcast = feed.Podcasts[i];
        var podcastFilePath = Path.Combine(feed.Directory, podcast.FileDetails.FileName);
        var podcastFileInfo = new FileInfo(podcastFilePath);
        if (!podcastFileInfo.Exists)
        {
          continue;
        }

        podcast.SetFileDetails(podcastFileInfo.Length, podcastFileInfo.CreationTime);
        syncCount++;
      }

      return syncCount;
    }
  }
}
