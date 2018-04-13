
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.ComponentModel;
  using Library;

  public class FeedViewModel : BaseViewModel, IFeedViewModel, INotifyPropertyChanged
  {
    private Boolean isSelected;
    private Boolean isExpanded;

    public FeedViewModel(Feed feed)
    {
      this.ImageFilePath = feed.ImageFileName;
      this.Title = feed.Title;
      this.Description = feed.Description;
      this.Children = new ObservableCollection<BaseViewModel>();

      foreach (var podcast in feed.Podcasts)
      {
        this.Children.Add(new PodcastViewModel(podcast));
      }

      this.Children.Add(new FeedSettingsViewModel());
    }

    public String Description { get; private set; }
    public String ImageFilePath { get; private set; }
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

    public ObservableCollection<BaseViewModel> Children { get; private set; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(String propertyName)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  public class FeedSettingsViewModel : BaseViewModel
  {
    public Boolean DoScan { get; set; }
  }

  public class BaseViewModel
  {

  }
}
