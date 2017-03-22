
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.IO;
  using Library;

  public class PodcastProperties
  {
    #region Construction
    public PodcastProperties(Podcast podcast, String feedDirectory)
    {
      this.Title = podcast.Title;
      this.Description = podcast.Description;
      this.DownloadedDate = new DownloadDateTimeConverter().ConvertToString(podcast.FileDetails.DownloadDate);
      this.ImageFileName = podcast.FileDetails.ImageFileName;
      this.PublishedDate = podcast.PubDate;
      this.URL = podcast.URL;

      if (podcast.FileDetails.DownloadDate == FeedFunctions.NoDateTime)
      {
        this.FilePath = "No download";
      }
      else
      {
        this.FilePath = Path.Combine(feedDirectory, podcast.FileName);
        if (!File.Exists(this.FilePath))
        {
          this.FilePath = "No file";
        }
      }

      this.FileSize = (new FileSizeConverter()).ConvertToString(podcast.FileDetails.FileSize) + "  (" + podcast.FileDetails.FileSize + " bytes)";
    }
    #endregion

    #region Properties
    public String Description { get; private set; }

    public String DownloadedDate { get; private set; }

    public String FilePath { get; private set; }

    public String FileSize { get; private set; }

    public String ImageFileName { get; private set; }

    public DateTime PublishedDate { get; private set; }

    public String Title { get; private set; }

    public String URL { get; private set; }
    #endregion
  }
}
