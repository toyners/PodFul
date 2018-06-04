﻿
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using Library;

  public class PodcastViewModel
  {
    private Podcast podcast;
    public PodcastViewModel(Podcast podcast)
    {
      this.podcast = podcast;
    }

    public String Description { get { return this.podcast.Description; } }
    public DateTime DownloadDate { get { return this.podcast.FileDetails.DownloadDate; } }
    public String FilePath { get { return this.podcast.FileDetails.FileName; } }
    public Int64 FileSize { get { return this.podcast.FileDetails.FileSize; } }
    public DateTime PublishedDate { get { return this.podcast.PubDate; } }
    public String Title { get { return this.podcast.Title; } }
    public String URL { get { return this.podcast.URL; } }
  }
}
