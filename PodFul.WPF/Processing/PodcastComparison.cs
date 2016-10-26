
namespace PodFul.WPF.Processing
{
  using System;
  using Library;

  public class PodcastComparison
  {
    #region Fields
    private Data oldPodcastData;

    private Data newPodcastData;
    #endregion

    #region Construction
    public PodcastComparison(Podcast oldPodcast, Int32 oldNumber, Podcast newPodcast, Int32 newNumber)
    {
      this.newPodcastData = new Data(newNumber, newPodcast);
      this.oldPodcastData = new Data(oldNumber, oldPodcast);
    }
    #endregion

    #region Properties
    public String OldFileSize { get { return this.oldPodcastData.FileSize; } }

    public String OldNumber { get { return this.oldPodcastData.Number; } }

    public String OldPubDate { get { return this.oldPodcastData.PubDate; } }

    public String OldTitle { get { return this.oldPodcastData.Title; } }

    public String OldURL { get { return this.oldPodcastData.URL; } }

    public String NewFileSize { get { return this.newPodcastData.FileSize; } }

    public String NewNumber { get { return this.newPodcastData.Number; } }

    public String NewPubDate { get { return this.newPodcastData.PubDate; } }

    public String NewTitle { get { return this.newPodcastData.Title; } }

    public String NewURL { get { return this.newPodcastData.URL; } }
    #endregion

    #region Classes
    private class Data
    {
      #region Construction
      public Data(Int32 number, Podcast podcast)
      {
        if (podcast == null)
        {
          this.Title = "(no match)";
        }
        else
        {
          if (podcast.FileDetails.FileSize == -1)
          {
            this.FileSize = "File size: (unknown)";
          }
          else
          {
            this.FileSize = "File size: " + podcast.FileDetails.FileSize.ToString();
          }

          this.Number = number.ToString();
          this.DownloadDate = "Download date: " + podcast.FileDetails.DownloadDate.ToString();
          this.PubDate = "Publishing date: " + podcast.PubDate.ToString();
          this.Title = podcast.Title;
          this.URL = "URL: " + podcast.URL;
        }
      }
      #endregion

      #region Properties
      public String DownloadDate { get; private set; }

      public String FileSize { get; private set; }

      public String Number { get; private set; }

      public String PubDate { get; private set; }

      public String Title { get; private set; }

      public String URL { get; private set; }
      #endregion
    }
    #endregion
  }
}
