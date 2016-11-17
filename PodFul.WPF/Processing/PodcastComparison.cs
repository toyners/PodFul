
namespace PodFul.WPF.Processing
{
  using System;
  using Library;

  public class PodcastComparison
  {
    #region Fields
    public const String NoMatch = "(no match)";

    private Data oldPodcastData;

    private Data newPodcastData;
    #endregion

    #region Construction
    private PodcastComparison(Podcast podcast, Int32 number, Boolean isNew)
    {
      if (isNew)
      {
        this.oldPodcastData = new Data();
        this.newPodcastData = new Data(number, podcast);
      }
      else
      {
        this.oldPodcastData = new Data(number, podcast);
        this.newPodcastData = new Data();
      }
    }

    private PodcastComparison(Podcast oldPodcast, Int32 oldNumber, Podcast newPodcast, Int32 newNumber)
    {
      this.newPodcastData = new Data(newNumber, newPodcast);
      this.oldPodcastData = new Data(oldNumber, oldPodcast);
    }
    #endregion

    #region Properties
    public Boolean HasNewPodcast { get { return this.newPodcastData != null; } }

    public Boolean HasNewPodcastOnly { get { return this.HasNewPodcast && this.oldPodcastData == null; } }

    public String NewFileSize { get { return this.newPodcastData.FileSize; } }

    public String NewPubDate { get { return this.newPodcastData.PubDate; } }

    public String NewTitle { get { return this.newPodcastData.Title; } }

    public String NewURL { get { return this.newPodcastData.URL; } }

    public String OldDownloadDate { get { return this.oldPodcastData.DownloadDate; } }

    public String OldFileSize { get { return this.oldPodcastData.FileSize; } }

    public String OldPubDate { get { return this.oldPodcastData.PubDate; } }

    public String OldTitle { get { return this.oldPodcastData.Title; } }

    public String OldURL { get { return this.oldPodcastData.URL; } }
    #endregion

    #region Methods
    public static PodcastComparison CreatePodcastComparisonWithNewPodcastOnly(Podcast podcast, Int32 number)
    {
      return new PodcastComparison(podcast, number, true);
    }

    public static PodcastComparison CreatePodcastComparisonWithCurrentPodcastOnly(Podcast podcast, Int32 number)
    {
      return new PodcastComparison(podcast, number, false);
    }

    public static PodcastComparison CreatePodcastComparisonWithBothPodcasts(Podcast oldPodcast, Int32 oldNumber, Podcast newPodcast, Int32 newNumber)
    {
      return new PodcastComparison(oldPodcast, oldNumber, newPodcast, newNumber);
    }
    #endregion

    #region Classes
    private class Data
    {
      #region Construction
      public Data()
      {
        this.Title = PodcastComparison.NoMatch;
      }

      public Data(Int32 number, Podcast podcast)
      {
        if (podcast.FileDetails.FileSize <= 0)
        {
          this.FileSize = "File size: (unknown)";
        }
        else
        {
          this.FileSize = "File size: " + podcast.FileDetails.FileSize.ToString();
        }

        this.DownloadDate = "Download date: " + podcast.FileDetails.DownloadDate.ToString();
        this.PubDate = "Publishing date: " + podcast.PubDate.ToString();
        this.Title = number + ". " + podcast.Title;
        this.URL = "URL: " + podcast.URL;
      }
      #endregion

      #region Properties
      public String DownloadDate { get; private set; }

      public String FileSize { get; private set; }

      public String PubDate { get; private set; }

      public String Title { get; private set; }

      public String URL { get; private set; }
      #endregion
    }
    #endregion
  }
}
