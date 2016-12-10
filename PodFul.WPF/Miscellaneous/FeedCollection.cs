
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Collections.ObjectModel;
  using Library;
  using Jabberwocky.Toolkit.Object;

  /// <summary>
  /// Thin container that combines the feed storage with the observable collection used as the source
  /// for the GUI feed list.
  /// </summary>
  public class FeedCollection
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

      this.Feeds = new ObservableCollection<Feed>(this.feedStorage.Feeds);
    }
    #endregion

    #region Properties
    public ObservableCollection<Feed> Feeds { get; private set; }
    #endregion

    #region Methods
    public void AddFeed(Feed feed)
    {
      this.feedStorage.Add(feed);
      this.Feeds.Add(feed);
    }

    public void RemoveFeed(Feed feed)
    {
      this.feedStorage.Remove(feed);
      this.Feeds.Remove(feed);
    }

    /// <summary>
    /// Update the feed in storage and in the collection.
    /// </summary>
    /// <param name="index">Index of the feed in the collection.</param>
    /// <param name="feed">Feed to be updated in storage and in collection.</param>
    public void UpdateFeed(Int32 index, Feed feed)
    {
      this.feedStorage.Update(feed);

      // Triggering changed event on the ObservableCollection by
      // removing and adding since reassigning does not work.
      this.Feeds.RemoveAt(index); 
      this.Feeds.Insert(index, feed);
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
