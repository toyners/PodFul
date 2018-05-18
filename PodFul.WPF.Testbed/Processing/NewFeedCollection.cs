
namespace PodFul.WPF.Testbed.Processing
{
  using Miscellaneous;
  using Library;
  using System.Windows;

  public class NewFeedCollection : FeedCollection
  {
    public NewFeedCollection(IFeedStorage feedStorage) : base(feedStorage)
    {
    }

    public override void UpdateFeedContent(Feed feed)
    {
      base.UpdateFeedContent(feed);

      // Operations on Observable collection always need to be done on the main thread so
      // invoke it.
      var index = this.ObservableFeeds.IndexOf(feed);
      Application.Current.Dispatcher.Invoke(() =>
      {
        base[index] = feed;
      });
    }
  }
}
