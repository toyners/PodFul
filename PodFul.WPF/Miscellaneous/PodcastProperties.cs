
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.IO;
  using Jabberwocky.Toolkit.Object;
  using Library;

  public class PodcastProperties
  {
    #region Construction
    public PodcastProperties(Podcast podcast, String feedDirectory)
    {
      feedDirectory.VerifyThatObjectIsNotNull("Parameter 'feedDirectory' is null.");
      this.Title = podcast.Title;
      this.Description = podcast.Description;
      this.DownloadedDate = new DownloadDateTimeConverter().ConvertToString(podcast.FileDetails.DownloadDate);
      this.ImageFileName = podcast.FileDetails.ImageFileName;
      this.PublishedDate = podcast.PubDate;
      this.URL = podcast.URL;

      if (podcast.FileDetails.DownloadDate == Miscellaneous.NoDateTime)
      {
        this.FilePath = "<No Download>";
      }
      else
      {
        this.FilePath = Path.Combine(feedDirectory, podcast.FileDetails.FileName);
        if (!File.Exists(this.FilePath))
        {
          this.FilePath = "<No File>";
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
