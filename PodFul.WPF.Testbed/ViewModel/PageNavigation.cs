﻿
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using Jabberwocky.Toolkit.WPF;
  using Library;

  public interface IPageNavigation
  {
    void MoveToNextPage();
    void MoveToFirstPage();
    void MoveToPreviousPage();
    void MoveToLastPage();
  }

  public class PageNavigation<T, U> : NotifyPropertyChangedBase, IPageNavigation where T : class where U : class
  {
    private Int32 pageNumber;
    private T currentPage;
    private ObservableCollection<T> pages = new ObservableCollection<T>();

    public T CurrentPage { get { return this.currentPage; } }

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

        this.pageNumber = value;
        this.currentPage = this.pages[this.pageNumber - 1];
        this.TryInvokePropertyChanged(new[]
        {
           new PropertyChangedEventArgs("CurrentPage"),
           new PropertyChangedEventArgs("PageNumber"),
           new PropertyChangedEventArgs("CanMoveBack"),
           new PropertyChangedEventArgs("CanMoveForward"),
        });
      }
    }

    public virtual void SetPages(IList<U> items, Int32 itemCountPerPage)
    {
      this.pages.Clear();

      for (var firstItemIndex = 0; firstItemIndex < items.Count; firstItemIndex += itemCountPerPage)
      {
        var lastItemIndex = firstItemIndex + itemCountPerPage - 1;
        if (lastItemIndex >= items.Count)
        {
          lastItemIndex = items.Count - 1;
        }

        var args = new Object[] { items, firstItemIndex, lastItemIndex };
        T page = Activator.CreateInstance(typeof(T), args) as T;
        this.pages.Add(page);
      }

      this.currentPage = this.pages[0];
      this.PageNumber = 1;
      this.TryInvokePropertyChanged(new PropertyChangedEventArgs("TotalPages"));
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
  }

  public class PodcastPageNavigation : PageNavigation<PodcastPageViewModel2, Podcast>
  {
  }

  public class JobPageNavigation : PageNavigation<JobPageViewModel, JobViewModel>
  {
    public Boolean HasJobs { get; private set; }

    public override void SetPages(IList<JobViewModel> items, Int32 itemCountPerPage)
    {
      base.SetPages(items, itemCountPerPage);

      if (this.TotalPages > 0)
      {
        this.HasJobs = true;
        this.TryInvokePropertyChanged(new PropertyChangedEventArgs("HasJobs"));
      }
    }
  }
}