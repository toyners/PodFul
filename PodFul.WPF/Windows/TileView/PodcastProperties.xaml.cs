
namespace PodFul.WPF.Windows
{
  using PodFul.WPF.ViewModel;
  using System;
  using System.Windows;

  /// <summary>
  /// Interaction logic for PodcastInfo.xaml
  /// </summary>
  public partial class PodcastProperties : Window
  {
    public PodcastProperties(PodcastViewModel podcastViewModel)
    {
      InitializeComponent();

      this.DataContext = podcastViewModel; 
    }

    private void CloseButton_Click(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
