
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;

  public interface IFeedViewModel
  {
    String ImageFilePath { get; }
    String Description { get; }
    String Title { get; }

    Boolean IsSelected { get; set; }
    Boolean IsExpanded { get; set; }

    ObservableCollection<TreeViewItemViewModel> Children { get; }
  }
}
