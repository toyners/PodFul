
namespace PodFul.WPF.Processing
{
  using System;

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
  }
}
