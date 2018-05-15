
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using Library;

  public class FeedViewModelBad : TreeViewItemViewModelBad, IFeedViewModel
  {
    public FeedViewModelBad(Feed feed)
    {
      this.ImageFilePath = feed.ImageFileName;
      this.Title = feed.Title;
      this.Description = feed.Description;
      this.Children = new ObservableCollection<TreeViewItemViewModelBad>();
      this.Children.Add(new FeedPodcastsExpandableViewModel(feed.Podcasts, 1));
      this.Children.Add(new FeedSettingsExpandableViewModel(feed));
    }

    public String Description { get; private set; }
    public String ImageFilePath { get; private set; }
    public String Title { get; private set; }

    public ObservableCollection<TreeViewItemViewModelBad> Children { get; private set; }
  }

  public class FeedSettingsExpandableViewModel : TreeViewItemViewModelBad
  {
    public FeedSettingsExpandableViewModel(Feed feed)
    {
      this.Children = new ObservableCollection<TreeViewItemViewModelBad>();
      this.Children.Add(new FeedSettingsViewModel(feed));
    }

    public ObservableCollection<TreeViewItemViewModelBad> Children { get; private set; }
  }

  public class FeedPodcastsExpandableViewModel : TreeViewItemViewModelBad
  {
    public FeedPodcastsExpandableViewModel(IList<Podcast> podcasts, Int32 count)
    {
      this.Navigation = new ObservableCollection<PodcastPageNavigationViewModel>();
      this.Navigation.Add(new PodcastPageNavigationViewModel(podcasts, count));
    }

    public ObservableCollection<PodcastPageNavigationViewModel> Navigation { get; private set; }
  }

  public class PodcastPageNavigationViewModel : INotifyPropertyChanged
  {
    private List<PodcastPageViewModel> pages;
    private Int32 pageNumber = 1;

    public event PropertyChangedEventHandler PropertyChanged;

    public PodcastPageNavigationViewModel(IList<Podcast> podcasts, Int32 count)
    {
      this.pages = new List<PodcastPageViewModel>();
      for (var index = 0; index < podcasts.Count; index += count)
      {
        this.pages.Add(new PodcastPageViewModel(podcasts, index, (index + count - 1)));
      }

      this.CurrentPage = new ObservableCollection<PodcastPageViewModel>();
      this.CurrentPage.Add(this.pages[0]);
    }

    public Boolean CanMoveBack { get { return this.PageNumber > 1; } }
    public Boolean CanMoveForward { get { return this.PageNumber < this.TotalPages; } }
    public ObservableCollection<PodcastPageViewModel> CurrentPage { get; private set; }
    public Int32 PageNumber
    {
      get { return this.pageNumber; }
      set
      {
        if (this.pageNumber == value)
        {
          return;
        }

        this.pageNumber = value;
        this.CurrentPage[0] = this.pages[this.pageNumber - 1];
        if (this.PropertyChanged != null)
        {
          this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PageNumber"));
          this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanMoveBack"));
          this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanMoveForward"));
          this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanMoveBack"));
          this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanMoveForward"));
        }
      }
    }
    public Int32 TotalPages { get { return this.pages.Count; } }

    public void MoveBack()
    {
      this.PageNumber--;
    }

    public void MoveForward()
    {
      this.PageNumber++;
    }

    public void MoveToFirstPage()
    {
      this.PageNumber = 1;
    }

    public void MoveToLastPage()
    {
      this.PageNumber = this.TotalPages;
    }
  }

  public class PodcastPageViewModel
  {
    public PodcastPageViewModel(IList<Podcast> podcasts, Int32 first, Int32 last)
    {
      this.Podcasts = new ObservableCollection<PodcastViewModelBad>();
      while (first <= last)
      {
        this.Podcasts.Add(new PodcastViewModelBad(podcasts[first++]));
      }
    }

    public ObservableCollection<PodcastViewModelBad> Podcasts { get; private set; }
  }

  public class FeedSettingsViewModel : TreeViewItemViewModelBad
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
