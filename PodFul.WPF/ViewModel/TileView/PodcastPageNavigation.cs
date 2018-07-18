
namespace PodFul.WPF.ViewModel.TileView
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using Jabberwocky.Toolkit.WPF;
  using Library;
  using PodFul.WPF.Processing.TileView;

  public class PodcastPageNavigation : NotifyPropertyChangedBase, IPageNavigation
  {
    #region Fields
    private FeedViewModel feedViewModel;
    private Int32 itemCountPerPage;
    private Int32 pageNumber;
    private ObservableCollection<PodcastPageViewModel> pages = new ObservableCollection<PodcastPageViewModel>();
    private IFileDownloadProxyFactory fileDownloadProxyFactory;
    #endregion

    #region Construction
    public PodcastPageNavigation(FeedViewModel feedViewModel, IFileDownloadProxyFactory fileDownloadProxyFactory)
    {
      this.feedViewModel = feedViewModel;
      this.fileDownloadProxyFactory = fileDownloadProxyFactory;
    }
    #endregion

    #region Properties
    public PodcastPageViewModel CurrentPage { get; private set; }

    public Int32 TotalPages { get { return this.pages.Count; } }

    public Boolean CanMoveBack { get { return this.pageNumber > 1; } }

    public Boolean CanMoveForward { get { return this.pageNumber < this.TotalPages; } }

    public PodcastViewModel this[Int32 index]
    {
      get
      {
        var pageIndex = index / this.itemCountPerPage;
        var podcastIndex = index % this.itemCountPerPage;
        return this.pages[pageIndex].PodcastViewModels[podcastIndex];
      }
    }

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
    #endregion

    #region Methods
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

    public void Reset()
    {
      this.pages.Clear();
      this.CurrentPage = null;
      this.pageNumber = 0;
    }

    public void SetPages(IList<Podcast> items, Int32 itemCountPerPage = 2)
    {
      this.itemCountPerPage = itemCountPerPage;
      for (var firstItemIndex = 0; firstItemIndex < items.Count; firstItemIndex += itemCountPerPage)
      {
        var lastItemIndex = firstItemIndex + itemCountPerPage - 1;
        if (lastItemIndex >= items.Count)
        {
          lastItemIndex = items.Count - 1;
        }

        PodcastPageViewModel page = new PodcastPageViewModel(this.feedViewModel, items, firstItemIndex, lastItemIndex, this.fileDownloadProxyFactory);
        this.pages.Add(page);
      }

      if (this.pages.Count > 0)
      {
        this.CurrentPage = this.pages[0];
        this.PageNumber = 1;
      }

      this.TryInvokePropertyChanged(new PropertyChangedEventArgs("TotalPages"));
    }
    #endregion
  }
}
