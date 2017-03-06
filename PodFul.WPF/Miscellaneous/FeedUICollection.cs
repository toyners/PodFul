
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Collections.ObjectModel;
  using Jabberwocky.Toolkit.Object;
  using Library;

  public class FeedUICollection : IFeedCollection
  {
    private IFeedCollection feedCollection;

    public FeedUICollection(IFeedCollection feedCollection)
    {
      feedCollection.VerifyThatObjectIsNotNull("Parameter 'feedCollection' is null.");
      this.feedCollection = feedCollection;
      this.Feeds = new ObservableCollection<Feed>();
    }

    #region Properties
    public ObservableCollection<Feed> Feeds { get; private set; }
    #endregion

    public void AddFeed(Feed feed)
    {
      this.Feeds.Add(feed);
      this.feedCollection.AddFeed(feed);
    }

    public void RemoveFeed(Feed feed)
    {
      this.Feeds.Remove(feed);
      this.feedCollection.RemoveFeed(feed);
    }

    public void UpdateFeed(Feed feed)
    {
      // Triggering changed event on the ObservableCollection by
      // removing and adding since reassigning does not work.
      var index = this.Feeds.IndexOf(feed);
      this.Feeds.RemoveAt(index);
      this.Feeds.Insert(index, feed);

      this.feedCollection.UpdateFeed(feed);
    }
  }
}
