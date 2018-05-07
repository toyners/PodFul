
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

  public class PageViewModel<V> where V : new()
  {
    public PageViewModel(IList<V> items, Int32 firstIndex, Int32 lastIndex)
    {

    }
  }

  public interface INavigationPageFactory<T>
  {
    T CreatePage();
    void Reset();
  }

  public class PodcastPageFactory : INavigationPageFactory<PodcastPageViewModel2>
  {
    public PodcastPageViewModel2 CreatePage()
    {
      throw new NotImplementedException();
    }

    public void Reset()
    {
      throw new NotImplementedException();
    }
  }

  public class PageNavigation<T> : NotifyPropertyChangedBase where T : class
  {
    private Int32 pageNumber = 1;
    private T currentPage;
    private ObservableCollection<T> pages;

    public PageNavigation(INavigationPageFactory<T> navigationPageFactory)
    {
      this.pages = new ObservableCollection<T>();
      var page = navigationPageFactory.CreatePage();
      while (page != null)
      {
        this.pages.Add(page);
      }

      this.currentPage = this.pages[0];
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
    public PodcastPageNavigation(INavigationPageFactory<PodcastPageViewModel2> navigationPageFactory) : base(navigationPageFactory)
    {
    }
  }
}
