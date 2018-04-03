
using System.Windows;
using PodFul.WPF.Windows;

namespace PodFul.WPF
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    private void Application_Startup(System.Object sender, StartupEventArgs e)
    {
      var mainWindow = new MainWindow();
      mainWindow.ShowDialog();
    }
  }
}
