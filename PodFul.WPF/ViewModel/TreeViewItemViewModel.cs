
namespace PodFul.WPF.ViewModel
{
  using System;
  using System.ComponentModel;

  public class TreeViewItemViewModel : INotifyPropertyChanged
  {
    private Boolean isSelected;
    private Boolean isExpanded;

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

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged(String propertyName)
    {
      this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
