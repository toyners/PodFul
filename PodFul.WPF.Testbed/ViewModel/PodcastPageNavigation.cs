
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using Jabberwocky.Toolkit.WPF;
  using Library;

  public class PodcastPageNavigation : NotifyPropertyChangedBase, IPageNavigation
  {
    private FeedViewModel feedViewModel;
    public PodcastPageNavigation(FeedViewModel feedViewModel) => this.feedViewModel = feedViewModel;

    private Int32 pageNumber;
    private ObservableCollection<PodcastPageViewModel> pages = new ObservableCollection<PodcastPageViewModel>();

    public PodcastPageViewModel CurrentPage { get; private set; }

    public Int32 TotalPages { get { return this.pages.Count; } }

    public Boolean CanMoveBack { get { return this.pageNumber > 1; } }

    public Boolean CanMoveForward { get { return this.pageNumber < this.TotalPages; } }

    public Int32 PageNumber
    {
      get { return this.pageNumber; }
      set
      {
        if (this.pageNumber == value)
        {
          return;
        }

        if (value < 1 || value > this.pages.Count)
        {
          return;
        }

        this.pageNumber = value;
        this.CurrentPage = this.pages[this.pageNumber - 1];
        this.TryInvokePropertyChanged(new[]
        {
           new PropertyChangedEventArgs("CurrentPage"),
           new PropertyChangedEventArgs("PageNumber"),
           new PropertyChangedEventArgs("CanMoveBack"),
           new PropertyChangedEventArgs("CanMoveForward"),
        });
      }
    }

    public void MoveToNextPage()
    {
      this.PageNumber++;
    }

    public void MoveToFirstPage()
    {
      this.PageNumber = 1;
    }

    public void MoveToPreviousPage()
    {
      this.PageNumber--;
    }

    public void MoveToLastPage()
    {
      this.PageNumber = this.pages.Count;
    }

    public virtual void Reset()
    {
      this.pages.Clear();
      this.CurrentPage = null;
      this.pageNumber = 0;
    }

    public virtual void SetPages(IList<Podcast> items, Int32 itemCountPerPage = 2)
    {
      for (var firstItemIndex = 0; firstItemIndex < items.Count; firstItemIndex += itemCountPerPage)
      {
        var lastItemIndex = firstItemIndex + itemCountPerPage - 1;
        if (lastItemIndex >= items.Count)
        {
          lastItemIndex = items.Count - 1;
        }

        PodcastPageViewModel page = this.CreatePage(items, firstItemIndex, lastItemIndex);
        this.pages.Add(page);
      }

      if (this.pages.Count > 0)
      {
        this.CurrentPage = this.pages[0];
        this.PageNumber = 1;
      }

      this.TryInvokePropertyChanged(new PropertyChangedEventArgs("TotalPages"));
    }

    private PodcastPageViewModel CreatePage(IList<Podcast> items, Int32 firstIndex, Int32 lastIndex)
    {
      return new PodcastPageViewModel(this.feedViewModel, items, firstIndex, lastIndex);
    }
  }
}
