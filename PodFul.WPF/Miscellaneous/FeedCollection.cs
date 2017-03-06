
namespace PodFul.WPF.Miscellaneous
{
  using Library;
  using Jabberwocky.Toolkit.Object;

  /// <summary>
  /// Thin container that combines the feed storage with the observable collection used as the source
  /// for the GUI feed list.
  /// </summary>
  public class FeedCollection : IFeedCollection
  {
    #region Fields
    private IFeedStorage feedStorage;
    #endregion

    #region Construction
    public FeedCollection(IFeedStorage feedStorage)
    {
      feedStorage.VerifyThatObjectIsNotNull("Parameter 'feedStorage' is null.");
      this.feedStorage = feedStorage;
      if (!this.feedStorage.IsOpen)
      {
        this.feedStorage.Open();
      }
    }
    #endregion

    #region Methods
    public void AddFeed(Feed feed)
    {
      this.feedStorage.Add(feed);
    }

    public void RemoveFeed(Feed feed)
    {
      this.feedStorage.Remove(feed);
    }

    /// <summary>
    /// Update the feed in storage.
    /// </summary>
    /// <param name="feed">Feed to be updated in storage</param>
    public void UpdateFeed(Feed feed)
    {
      this.feedStorage.Update(feed);
    }
    #endregion
  }
}
