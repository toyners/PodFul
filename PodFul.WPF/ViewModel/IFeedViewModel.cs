
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;

  public interface IFeedViewModel
  {
    String Image { get; }
    String Description { get; }
    String Title { get; }

    Boolean IsSelected { get; set; }
    Boolean IsExpanded { get; set; }

    ObservableCollection<IPodcastViewModel> Podcasts { get; }
  }
}
