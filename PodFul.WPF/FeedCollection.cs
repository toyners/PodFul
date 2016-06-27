
namespace PodFul.WPF
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Library;

  public class FeedCollection
  {
    private IFeedStorage feedStorage;

    public FeedCollection(String directoryPath)
    {
      this.feedStorage = new FeedFileStorage(directoryPath);
      this.feedStorage.Open();
      this.Feeds = new ObservableCollection<Feed>(this.feedStorage.Feeds);
    }

    public ObservableCollection<Feed> Feeds { get; private set; }

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
      this.Feeds[index] = feed;
    }

    public void UpdateFeed(Feed feed)
    {
      this.feedStorage.Update(feed);
    }
  }
}
