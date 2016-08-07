
namespace PodFul.WPF
{
  using System;
  using System.Windows;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for PodcastPropertiesWindow.xaml
  /// </summary>
  public partial class PodcastPropertiesWindow : Window
  {
    public PodcastPropertiesWindow(Podcast podcast)
    {
      InitializeComponent();
      this.DataContext = podcast; 
    }

    private void CloseButton_Click(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}