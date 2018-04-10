
namespace PodFul.WPF.ViewModel
{
  using System;
  using Library;

  public class FeedViewModel : IFeedViewModel
  {
    public String Description { get; private set; }
    public String Image { get; private set; }
    public String Title { get; private set; }

    public Boolean IsExpanded
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public Boolean IsSelected
    {
      get
      {
        throw new NotImplementedException();
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    public FeedViewModel(Feed feed)
    {
      this.Title = feed.Title;
      this.Description = feed.Description;
    }
  }
}
