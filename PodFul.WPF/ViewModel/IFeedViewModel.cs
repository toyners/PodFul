
namespace PodFul.WPF.ViewModel
{
  using System;

  public interface IFeedViewModel
  {
    String Image { get; }
    String Description { get; }
    String Title { get; }

    Boolean IsSelected { get; set; }
    Boolean IsExpanded { get; set; }
  }
}
