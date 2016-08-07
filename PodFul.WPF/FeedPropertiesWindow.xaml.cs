
namespace PodFul.WPF
{
  using System;
  using System.Windows;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for FeedPropertiesWindow.xaml
  /// </summary>
  public partial class FeedPropertiesWindow : Window
  {
    public FeedPropertiesWindow(Feed feed)
    {
      InitializeComponent();
      this.DataContext = feed;
    }

    private void CloseButton_Click(Object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
