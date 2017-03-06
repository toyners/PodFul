
namespace PodFul.WPF.Miscellaneous
{
  using Library;

  public interface IFeedCollection
  {
    void AddFeed(Feed feed);

    void RemoveFeed(Feed feed);

    void UpdateFeed(Feed feed);
  }
}
