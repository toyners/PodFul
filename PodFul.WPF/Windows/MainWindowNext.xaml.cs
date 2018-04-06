using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using PodFul.Library;
using PodFul.WPF.Logging;
using PodFul.WPF.Processing;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for MainWindowNext.xaml
  /// </summary>
  public partial class MainWindowNext : Window
  {
    private IFeedProcessor feedProcessor;
    private IFeedCollectionViewModel feedCollectionViewModel;
    private ObservableCollection<Feed> ObservableFeeds;

    public MainWindowNext(IFeedProcessor feedProcessor)
    {
      InitializeComponent();

      this.feedCollectionViewModel = new FeedCollectionViewModel(feedProcessor.Feeds);

      this.FeedTree.ItemsSource = this.feedCollectionViewModel.Feeds;
    }

    private void AddFeedButtonClick(Object sender, RoutedEventArgs e)
    {
      // Use dialog to get the feed directory and url.

      // Open dialog to show progress of adding new feed to the feed processor.

      var feed = this.feedProcessor.AddFeed("Title", "Description for Title");
      this.ObservableFeeds.Add(feed);
    }

    private void FullScanButtonClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void RemoveFeedClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }

    private void SettingsButtonClick(Object sender, RoutedEventArgs e)
    {
      throw new NotImplementedException();
    }
  }

  public interface IFeedProcessor
  {
    IList<Feed> Feeds { get; }
    Action StartedAddingFeed { get; set; }
    Action FinishedAddingFeed { get; set; }

    Feed AddFeed(String directory, String url);
    void RemoveFeed(Feed feed);
    void RemoveFeed(Int32 index);
    void ScanFeeds(IList<Int32> indexes);
  }

  public class TestFeedProcessor : IFeedProcessor
  {
    public IList<Feed> Feeds { get; private set; }

    public Action FinishedAddingFeed { get; set; }

    public Action StartedAddingFeed { get; set; }

    public Feed AddFeed(String title, String description)
    {
      this.StartedAddingFeed?.Invoke();

      var newFeed = new Feed(title, description, "", "", "", "", "",
        new Podcast[0], DateTime.MinValue, DateTime.MinValue, true, true, true, 3u);

      this.FinishedAddingFeed?.Invoke();

      return newFeed;
    }

    public void RemoveFeed(Int32 index)
    {
      throw new NotImplementedException();
    }

    public void RemoveFeed(Feed feed)
    {
      throw new NotImplementedException();
    }

    public void ScanFeeds(IList<Int32> indexes)
    {
      throw new NotImplementedException();
    }
  }

  public class FeedProcessor : IFeedProcessor
  {
    public IList<Feed> Feeds { get; private set; }

    public Action FinishedAddingFeed { get; set; }

    public Action StartedAddingFeed { get; set; }

    public Feed AddFeed(String directory, String url)
    {
      throw new NotImplementedException();
    }

    public void RemoveFeed(Int32 index)
    {
      throw new NotImplementedException();
    }

    public void RemoveFeed(Feed feed)
    {
      throw new NotImplementedException();
    }

    public void ScanFeeds(IList<Int32> indexes)
    {
      throw new NotImplementedException();
    }
  }

  public interface IFeedCollectionViewModel
  {
    ObservableCollection<IFeedViewModel> Feeds { get; }

    void Add(Feed feed);

    void Remove(Feed feed);
  }

  public class FeedCollectionViewModel : IFeedCollectionViewModel
  {
    public ObservableCollection<IFeedViewModel> Feeds { get; private set; }

    public FeedCollectionViewModel(IEnumerable<Feed> feeds)
    {
      this.Feeds = new ObservableCollection<IFeedViewModel>();
      foreach (var feed in feeds)
      {
        this.Feeds.Add(new FeedViewModel(feed));  
      }
    }

    public void Add(Feed feed)
    {
      throw new NotImplementedException();
    }

    public void Remove(Feed feed)
    {
      throw new NotImplementedException();
    }
  }

  public interface IFeedViewModel
  {
    String Image { get; }
    String Description { get; }
    String Title { get; }

    Boolean IsSelected { get; set; }
    Boolean IsExpanded { get; set; }
  }

  public class FeedViewModel : IFeedViewModel
  {
    public String Description { get; private set; }
    public String Image { get; private set; }
    public String Title { get; private set; }

    public Boolean IsExpanded
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public Boolean IsSelected
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public FeedViewModel(Feed feed)
    {
      throw new NotImplementedException();
    }
  }

  public interface IPodcastViewModel
  {
    String Title { get; }
    String Description { get; }
    String Image { get; }
  }
}
