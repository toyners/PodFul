
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using Library;

  public class FeedViewModel : IFeedViewModel, INotifyPropertyChanged
  {
    private Boolean isSelected;
    private Boolean isExpanded;

    public String Description { get; private set; }
    public String Image { get; private set; }
    public String Title { get; private set; }

    public Boolean IsExpanded
    {
      get { return this.isExpanded; }
      set
      {
        if (this.isExpanded == value)
        {
          return;
        }

        this.isExpanded = value;
        this.OnPropertyChanged("IsExpanded");
      }
    }

    public Boolean IsSelected
    {
      get { return this.isSelected; }
      set
      {
        if (this.isSelected == value)
        {
          return;
        }

        this.isSelected = value;
        this.OnPropertyChanged("IsSelected");
      }
    }

    public ObservableCollection<IPodcastViewModel> Podcasts { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public FeedViewModel(Feed feed)
    {
      this.Title = feed.Title;
      this.Description = feed.Description;
    }

    private void OnPropertyChanged(String propertyName)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
