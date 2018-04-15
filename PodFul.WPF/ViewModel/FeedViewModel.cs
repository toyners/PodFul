
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
      this.Children.Add(new FeedPodcastsExpandableViewModel(feed.Podcasts, 1));
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
    private Int32 pageIndex;
    private ObservableCollection<PodcastPageViewModel> currentPage = new ObservableCollection<PodcastPageViewModel>();
    public FeedPodcastsExpandableViewModel()
    { }
    public FeedPodcastsExpandableViewModel(IList<Podcast> podcasts, Int32 count)
    {
      this.Pages = new ObservableCollection<PodcastPageViewModel>();
      for (var index = 0; index < podcasts.Count; index+=count)
      {
        this.Pages.Add(new PodcastPageViewModel(podcasts, index, (index + count - 1)));
      }

      this.currentPage.Add(this.Pages[0]);
    }

    public ObservableCollection<PodcastPageViewModel> Pages { get; private set; }

    public ObservableCollection<PodcastPageViewModel> CurrentPage { get { return this.currentPage; } }

    public void Next()
    {

    }
  }

  public class PodcastPageViewModel
  {
    public PodcastPageViewModel(IList<Podcast> podcasts, Int32 first, Int32 last)
    {
      this.Podcasts = new ObservableCollection<PodcastViewModel>();
      while (first <= last)
      {
        this.Podcasts.Add(new PodcastViewModel(podcasts[first++]));
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
