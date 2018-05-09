﻿
namespace PodFul.WPF.Testbed.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using Jabberwocky.Toolkit.WPF;
  using Library;

  public class PageViewModel<V> where V : new()
  {
    public PageViewModel(IList<V> items, Int32 firstIndex, Int32 lastIndex)
    {

    }
  }

  public interface INavigationPageFactory<T>
  {
    T GetPage();
    void Reset();
  }

  public class PodcastPageFactory : INavigationPageFactory<PodcastPageViewModel2>
  {
    private List<PodcastPageViewModel2> pages;
    private Int32 pageIndex;

    public PodcastPageFactory(IList<Podcast> podcasts, Int32 podcastCountPerPage)
    {
      this.pages = new List<PodcastPageViewModel2>();
      for (var firstPodcastIndex = 0; firstPodcastIndex < podcasts.Count; firstPodcastIndex += podcastCountPerPage)
      {
        var lastPodcastIndex = firstPodcastIndex + podcastCountPerPage - 1;
        if (lastPodcastIndex >= podcasts.Count)
        {
          lastPodcastIndex = podcasts.Count - 1;
        }

        this.pages.Add(new PodcastPageViewModel2(podcasts, firstPodcastIndex, lastPodcastIndex));
      }

      this.Reset();
    }

    public PodcastPageViewModel2 GetPage()
    {
      if (this.pageIndex == this.pages.Count)
      {
        return null;
      }

      return this.pages[this.pageIndex++];
    }

    public void Reset()
    {
      this.pageIndex = 0;
    }
  }

  public class JobPageFactory : INavigationPageFactory<JobPageViewModel>
  {
    private List<JobPageViewModel> pages;
    private Int32 pageIndex;

    public JobPageFactory(IList<JobViewModel> jobs, Int32 jobCountPerPage)
    {
      this.pages = new List<JobPageViewModel>();
      for (var firstJobIndex = 0; firstJobIndex < jobs.Count; firstJobIndex += jobCountPerPage)
      {
        var lastJobIndex = firstJobIndex + jobCountPerPage - 1;
        if (lastJobIndex >= jobs.Count)
        {
          lastJobIndex = jobs.Count - 1;
        }

        this.pages.Add(new JobPageViewModel(jobs, firstJobIndex, lastJobIndex));
      }

      this.Reset();
    }

    public JobPageViewModel GetPage()
    {
      if (this.pageIndex == this.pages.Count)
      {
        return null;
      }

      return this.pages[this.pageIndex++];
    }

    public void Reset()
    {
      this.pageIndex = 0;
    }
  }

  public interface IPageNavigation
  {
    void MoveToNextPage();
    void MoveToFirstPage();
    void MoveToPreviousPage();
    void MoveToLastPage();
  }

  public class PageNavigation<T> : NotifyPropertyChangedBase, IPageNavigation where T : class
  {
    private Int32 pageNumber;
    private T currentPage;
    private ObservableCollection<T> pages;

    public PageNavigation()
    {
      this.pages = new ObservableCollection<T>();
    }

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

    public virtual void SetPages(INavigationPageFactory<T> navigationPageFactory)
    {
      this.pages.Clear();
      T page = null;
      while ((page = navigationPageFactory.GetPage()) != null)
      {
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

  public class PodcastPageNavigation : PageNavigation<PodcastPageViewModel2>
  {
  }

  public class JobPageNavigation : PageNavigation<JobPageViewModel>
  {
    private Boolean hasJobs;

    public Boolean HasJobs
    {
      get { return this.hasJobs; }
      private set
      {
        this.SetField(ref this.hasJobs, value, "HasJobs");
      }
    }

    public override void SetPages(INavigationPageFactory<JobPageViewModel> navigationPageFactory)
    {
      base.SetPages(navigationPageFactory);

      if (this.TotalPages > 0)
      {
        this.HasJobs = true;
      }
    }
  }
}
