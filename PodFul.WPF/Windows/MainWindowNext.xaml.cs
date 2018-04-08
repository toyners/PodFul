using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows;
using PodFul.Library;
using PodFul.WPF.Logging;
using PodFul.WPF.Processing;
using PodFul.WPF.ViewModel;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for MainWindowNext.xaml
  /// </summary>
  public partial class MainWindowNext : Window
  {
    private IFeedCollectionViewModel feedCollectionViewModel;

    public MainWindowNext(IFeedCollectionViewModel feedCollectionViewModel)
    {
      InitializeComponent();

      this.feedCollectionViewModel = feedCollectionViewModel;
      this.FeedTree.ItemsSource = this.feedCollectionViewModel.Feeds;
    }

    private void AddFeedButtonClick(Object sender, RoutedEventArgs e)
    {
      // Use dialog to get the feed directory and url.
      AddFeedToken addFeedToken;
      if (!this.TryGetFeedCreationData(out addFeedToken))
      {
        return;
      }

      // Open dialog to show progress of adding new feed to the feed processor.
      var addFeedProgressWindow = new AddFeedProgressWindow(this.feedCollectionViewModel, addFeedToken);
      addFeedProgressWindow.Owner = this;
      addFeedProgressWindow.ShowDialog();
      Feed feed = addFeedProgressWindow.Feed;
      if (feed == null)
      {
        // Cancelled or Faulted - nothing more to be done.
        return;
      }

      //this.feedCollectionViewModel.AddFeed(directory, url);
    }

    private Boolean TryGetFeedCreationData(out AddFeedToken addFeedToken)
    {
      addFeedToken = default(AddFeedToken);

      var addFeedWindow = new AddFeedWindow();
      addFeedWindow.Owner = this;
      var dialogResult = addFeedWindow.ShowDialog();
      if (!dialogResult.HasValue || !dialogResult.Value)
      {
        return false;
      }

      addFeedToken = new AddFeedToken(addFeedWindow.FeedDirectory, addFeedWindow.FeedURL, "");
      return true;
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

    Feed AddFeed(String directory, String url, String defaultPodcastImageFilePath, CancellationToken cancelToken);
    void RemoveFeed(Feed feed);
    void RemoveFeed(Int32 index);
    void ScanFeeds(IList<Int32> indexes);
  }

  public class FeedProcessor : IFeedProcessor
  {
    private ILogController logController;

    public IList<Feed> Feeds { get; private set; }

    public Action FinishedAddingFeed { get; set; }

    public Action StartedAddingFeed { get; set; }

    public Feed AddFeed(String directory, String url, String defaultPodcastImageFilePath, CancellationToken cancelToken)
    {
      try
      {
        var feedFilePath = Path.Combine(directory, "download.rss");
        var feed = FeedFunctions.CreateFeed(url, feedFilePath, directory, defaultPodcastImageFilePath, cancelToken);
        this.logController.Message(MainWindow.InfoKey, "'" + feed.Title + "' added. Podcasts stored in '" + directory + "'");
        return feed;
      }
      catch (OperationCanceledException oce)
      {
        this.logController.Message(MainWindow.InfoKey, "Adding feed from '" + url + "' was cancelled.");
      }
      catch (AggregateException ae)
      {
        var flattenedMessage = String.Empty;
        var flattenedException = ae.Flatten();
        foreach (var exception in flattenedException.InnerExceptions)
        {
          flattenedMessage += exception.Message + " ";
        }

        throw new Exception(flattenedMessage);
      }
      catch (Exception e)
      {
        this.logController.Message(MainWindow.ExceptionKey, "Trying to create new feed: " + e.Message);
        throw e;
      }

      return null;
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

  public struct AddFeedToken
  {
    public readonly String Directory;
    public readonly String Url;
    public readonly String DefaultPodcastImageFilePath;

    public AddFeedToken(String directory, String url, String defaultPodcastImageFilePath)
    {
      this.Directory = directory;
      this.Url = url;
      this.DefaultPodcastImageFilePath = defaultPodcastImageFilePath;
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
      this.Title = feed.Title;
      this.Description = feed.Description;
    }
  }

  public interface IPodcastViewModel
  {
    String Title { get; }
    String Description { get; }
    String Image { get; }
  }
}
