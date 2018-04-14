
namespace PodFul.WPF.ViewModel
{
  using System;
  using Library;

  public class PodcastViewModel : TreeViewItemViewModel, IPodcastViewModel
  {
    public PodcastViewModel(Podcast podcast)
    {
      this.Title = podcast.Title;
      this.Description = podcast.Description;
      this.ImageFilePath = podcast.FileDetails.ImageFileName;
    }

    public String Description { get; private set; }
    public String ImageFilePath { get; private set; }
    public String Title { get; private set; }
  }
}
