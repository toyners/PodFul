
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using Library;

  public class PodcastViewModel
  {
    private FeedViewModel feedViewModel;
    private Podcast podcast;
    public PodcastViewModel(FeedViewModel feedViewModel, Podcast podcast)
    {
      this.feedViewModel = feedViewModel;
      this.podcast = podcast;
    }

    public String Description { get { return this.podcast.Description; } }
    public DateTime DownloadDate { get { return this.podcast.FileDetails.DownloadDate; } }
    public String FilePath { get { return this.podcast.FileDetails.FileName; } }
    public Int64 FileSize { get { return this.podcast.FileDetails.FileSize; } }
    public String ImageFileName { get { return this.podcast.FileDetails.ImageFileName; } }
    public DateTime PublishedDate { get { return this.podcast.PubDate; } }
    public String Title { get { return this.podcast.Title; } }
    public String URL { get { return this.podcast.URL; } }
  }
}
