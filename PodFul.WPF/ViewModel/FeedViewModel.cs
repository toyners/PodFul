
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using Library;

  public class FeedViewModel : TreeViewItemViewModel, IFeedViewModel
  {
    public FeedViewModel(Feed feed)
    {
      this.ImageFilePath = feed.ImageFileName;
      this.Title = feed.Title;
      this.Description = feed.Description;
      this.Children = new ObservableCollection<TreeViewItemViewModel>();

      foreach (var podcast in feed.Podcasts)
      {
        this.Children.Add(new PodcastViewModel(podcast));
      }

      this.Children.Add(new FeedSettingsExpandableViewModel(feed));
    }

    public String Description { get; private set; }
    public String ImageFilePath { get; private set; }
    public String Title { get; private set; }

    public ObservableCollection<TreeViewItemViewModel> Children { get; private set; }
  }

  public class FeedSettingsExpandableViewModel : TreeViewItemViewModel
  {
    public FeedSettingsExpandableViewModel(Feed feed)
    {
      this.Children = new ObservableCollection<TreeViewItemViewModel>();
      this.Children.Add(new FeedSettingsViewModel(feed));
    }

    public ObservableCollection<TreeViewItemViewModel> Children { get; private set; }
  }

  public class FeedSettingsViewModel : TreeViewItemViewModel
  {
    public FeedSettingsViewModel(Feed feed)
    {
      this.DoScan = feed.DoScan;
    }

    public Boolean DoScan { get; set; }
  }
}
