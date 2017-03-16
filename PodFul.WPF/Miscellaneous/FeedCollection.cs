
namespace PodFul.WPF.Miscellaneous
{
  using Library;
  using Jabberwocky.Toolkit.Object;
  using System;
  using System.Collections.ObjectModel;

  /// <summary>
  /// Thin container that wraps feed storage.
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

      var feeds = this.feedStorage.Feeds;
      if (feeds == null || feeds.Length == 0)
      {
        throw new Exception("Feed Storage contains no feeds");
      }

      this.ObservableFeeds = new ObservableCollection<Feed>(this.feedStorage.Feeds);
    }
    #endregion

    #region Properties
    public Feed this[Int32 index]
    {
      get
      {
        if (index  < 0 || index >= this.ObservableFeeds.Count)
        {
          throw new Exception(String.Format("Cannot get feed: Index ({0}) is outside range (0..{1})", index, this.ObservableFeeds.Count - 1));
        }

        return this.ObservableFeeds[index];
      }

      set
      {
        if (index < 0 || index >= this.ObservableFeeds.Count)
        {
          throw new Exception(String.Format("Cannot set feed: Index ({0}) is outside range (0..{1})", index, this.ObservableFeeds.Count - 1));
        }

        // Triggering changed event on the ObservableCollection by
        // removing and adding since reassigning does not work.
        this.ObservableFeeds.RemoveAt(index);
        this.ObservableFeeds.Insert(index, value);
      }
    }

    public Int32 Count
    {
      get
      {
        return this.ObservableFeeds.Count;
      }
    }

    public ObservableCollection<Feed> ObservableFeeds { get; private set; }
    #endregion

    #region Methods
    /// <summary>
    /// Add feed to collection and to storage.
    /// </summary>
    /// <param name="feed">Feed to add.</param>
    public void AddFeed(Feed feed)
    {
      this.feedStorage.Add(feed);
      this.ObservableFeeds.Add(feed);
    }

    /// <summary>
    /// Remove feed from collection and from storage.
    /// </summary>
    /// <param name="feed">Feed to be removed.</param>
    public void RemoveFeed(Feed feed)
    {
      this.feedStorage.Remove(feed);
      var index = this.ObservableFeeds.IndexOf(feed);
      this.ObservableFeeds.RemoveAt(index);
    }

    /// <summary>
    /// Update the feed in storage.
    /// </summary>
    /// <param name="feed">Feed to be updated in storage</param>
    public void UpdateFeedContent(Feed feed)
    {
      this.feedStorage.Update(feed);
    }
    #endregion
  }
}
