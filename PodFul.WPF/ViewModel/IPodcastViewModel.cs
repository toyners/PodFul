
namespace PodFul.WPF.ViewModel
{
  using System;

  public interface IPodcastViewModel
  {
    String Title { get; }
    String Description { get; }
    String ImageFilePath { get; }
  }
}
