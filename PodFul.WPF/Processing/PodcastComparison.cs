
namespace PodFul.WPF.Processing
{
  using System;
  using Library;

  public class PodcastComparison
  {
    #region Fields
    private Podcast oldPodcast;

    private Podcast newPodcast;
    #endregion

    #region Construction
    public PodcastComparison(Podcast oldPodcast, Podcast newPodcast)
    {
      this.newPodcast = newPodcast;
      this.oldPodcast = oldPodcast;
    }
    #endregion

    #region Properties
    public String OldTitle { get { return this.oldPodcast.Title; } }

    public String NewTitle { get { return this.newPodcast.Title; } }
    #endregion
  }
}
