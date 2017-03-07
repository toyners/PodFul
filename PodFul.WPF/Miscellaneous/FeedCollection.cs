﻿
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

      this.ObservableFeeds = new ObservableCollection<Feed>(this.feedStorage.Feeds);
    }
    #endregion

    #region Properties
    public Feed this[Int32 index]
    {
      get
      {
        return this.ObservableFeeds[index];
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

      // Triggering changed event on the ObservableCollection by
      // removing and adding since reassigning does not work.
      var index = this.ObservableFeeds.IndexOf(feed);
      this.ObservableFeeds.RemoveAt(index);
      this.ObservableFeeds.Insert(index, feed);
    }
    #endregion
  }
}
