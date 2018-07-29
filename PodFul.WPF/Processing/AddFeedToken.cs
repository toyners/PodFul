
namespace PodFul.WPF.Processing
{
  using System;
  using System.IO;

  public struct AddFeedToken
  {
    public readonly String Directory;
    public readonly String Url;
    public readonly String DefaultPodcastImageFilePath;

    public AddFeedToken(String directory, String url, String defaultPodcastImageFilePath)
    {
      this.Directory = directory;
      this.Url = url;
      this.DefaultPodcastImageFilePath = defaultPodcastImageFilePath;
    }

    public String DownloadPath { get { return Path.Combine(this.Directory, "download.rss"); } }
  }
}
