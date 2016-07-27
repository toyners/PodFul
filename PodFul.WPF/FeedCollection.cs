
namespace PodFul.WPF
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

    public void UpdateFeed(Int32 index, Feed feed)
    {
      this.feedStorage.Update(feed);

      // Triggering changed event on the ObservableCollection by
      // removing and adding since reassigning does not work.
      this.Feeds.RemoveAt(index); 
      this.Feeds.Insert(index, feed);
    }

    public void UpdateFeed(Feed feed)
    {
      this.feedStorage.Update(feed);
    }
    #endregion
  }
}
