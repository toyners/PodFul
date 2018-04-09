using System;
using System.Windows;
using PodFul.Library;
using PodFul.WPF.Miscellaneous;
using PodFul.WPF.ViewModel;

namespace PodFul.WPF.Windows
{
  /// <summary>
  /// Interaction logic for MainWindowNext.xaml
  /// </summary>
  public partial class MainWindowNext : Window
  {
    private Settings settings;
    private IFeedCollectionViewModel feedCollectionViewModel;

    public MainWindowNext(Settings settings, IFeedCollectionViewModel feedCollectionViewModel)
    {
      InitializeComponent();

      this.settings = settings;
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
