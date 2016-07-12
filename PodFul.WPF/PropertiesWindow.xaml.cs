
namespace PodFul.WPF
{
  using System.Windows;
  using PodFul.Library;

  /// <summary>
  /// Interaction logic for Properties.xaml
  /// </summary>
  public partial class PropertiesWindow : Window
  {
    public PropertiesWindow(Feed feed)
    {
      InitializeComponent();
      this.DataContext = feed;
    }

    private void CloseButton_Click(System.Object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
