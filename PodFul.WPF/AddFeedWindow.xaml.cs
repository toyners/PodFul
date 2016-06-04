
namespace PodFul.WPF
{
  using System;
  using System.IO;
  using System.Windows;
  using System.Windows.Controls;

  /// <summary>
  /// Interaction logic for AddFeedWindow.xaml
  /// </summary>
  public partial class AddFeedWindow : Window
  {
    public AddFeedWindow()
    {
      InitializeComponent();
    }

    public String FeedDirectory { get { return this.DirectoryPath.Text; } }

    public String FeedURL { get { return this.URL.Text; } }

    private void okButton_Click(Object sender, RoutedEventArgs e)
    {
      var directoryPath = this.DirectoryPath.Text;
      if (!directoryPath.EndsWith(@"\", StringComparison.Ordinal))
      {
        directoryPath += @"\";
      }

      if (!Directory.Exists(directoryPath))
      {
        var message = String.Format("Directory '{0}' does not exist. Create it now?", directoryPath);
        var createDirectory = MessageBox.Show(message, "Directory does not exist", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

        if (createDirectory == MessageBoxResult.Cancel)
        {
          return;
        }

        if (createDirectory == MessageBoxResult.Yes)
        {
          Directory.CreateDirectory(directoryPath);
        }
      }

      this.DialogResult = true;
    }

    private void textChanged(Object sender, TextChangedEventArgs e)
    {
      this.OkButton.IsEnabled = (this.DirectoryPath.Text.Length > 0 && this.URL.Text.Length > 0);
      e.Handled = true;
    }
  }
}
