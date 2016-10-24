
namespace PodFul.WPF.Processing
{
  using System;
  using Library;

  public class PodcastComparison
  {
    #region Fields
    private Data oldPodcast;

    private Data newPodcast;
    #endregion

    #region Construction
    public PodcastComparison(Podcast oldPodcast, Podcast newPodcast)
    {
      this.newPodcast = new Data(newPodcast);
      this.oldPodcast = new Data(oldPodcast);
    }
    #endregion

    #region Properties
    public String OldTitle { get { return this.oldPodcast.Title; } }

    public String NewTitle { get { return this.newPodcast.Title; } }
    #endregion

    #region Classes
    private class Data
    {
      #region Construction
      public Data(Podcast podcast)
      {
        if (podcast == null)
        {
          this.Title = String.Empty;
        }
        else
        {
          this.Title = podcast.Title;
        }
      }
      #endregion

      #region Properties
      public String Title { get; private set; }
      #endregion
    }
    #endregion
  }
}
