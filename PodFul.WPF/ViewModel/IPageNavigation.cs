
namespace PodFul.WPF.ViewModel
{
  public interface IPageNavigation
  {
    void MoveToNextPage();
    void MoveToFirstPage();
    void MoveToPreviousPage();
    void MoveToLastPage();
  }
}
