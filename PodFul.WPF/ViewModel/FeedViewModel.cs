
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.Generic;
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
      this.Children.Add(new FeedPodcastsExpandableViewModel(feed.Podcasts));
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

  public class FeedPodcastsExpandableViewModel : TreeViewItemViewModel
  {
    public FeedPodcastsExpandableViewModel(IList<Podcast> podcasts)
    {
      this.Pages = new ObservableCollection<PodcastPageViewModel>();
      this.Pages.Add(new PodcastPageViewModel(podcasts));
    }

    public ObservableCollection<PodcastPageViewModel> Pages { get; private set; }
  }

  public class PodcastPageViewModel
  {
    public PodcastPageViewModel(IList<Podcast> podcasts)
    {
      this.Podcasts = new ObservableCollection<PodcastViewModel>();
      foreach(var podcast in podcasts)
      {
        this.Podcasts.Add(new PodcastViewModel(podcast));
      }
    }

    public ObservableCollection<PodcastViewModel> Podcasts { get; private set; }
  }

  public class FeedSettingsViewModel : TreeViewItemViewModel
  {
    public FeedSettingsViewModel(Feed feed)
    {
      this.DoScan = feed.DoScan;
      this.CompleteDownloadsOnScan = feed.CompleteDownloadsOnScan;
    }

    public Boolean DoScan { get; set; }
    public Boolean CompleteDownloadsOnScan { get; set; }
  }
}
