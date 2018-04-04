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
    private ObservableCollection<Feed> ObservableFeeds;

    public MainWindowNext(IFeedProcessor feedProcessor)
    {
      InitializeComponent();

      this.feedProcessor = feedProcessor;
      if (this.feedProcessor.Feeds == null || this.feedProcessor.Feeds.Count == 0)
      {
        this.ObservableFeeds = new ObservableCollection<Feed>();
      }
      else
      {
        this.ObservableFeeds = new ObservableCollection<Feed>(this.feedProcessor.Feeds);
      }

      this.FeedTree.ItemsSource = this.ObservableFeeds;
    }

    private void AddFeedButtonClick(Object sender, RoutedEventArgs e)
    {
      // Use dialog to get the feed directory and url.

      // Open dialog to show progress of adding new feed to the feed processor.

      throw new NotImplementedException();
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

    public Action FinishedAddingFeed
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

    public Action StartedAddingFeed
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

    public Feed AddFeed(String directory, String url)
    {
      this.StartedAddingFeed?.Invoke();

      var newFeed = new Feed(directory, "Description for " + directory, "", "", url, "", "",
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
}
