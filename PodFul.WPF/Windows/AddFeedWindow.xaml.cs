
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

    public String FeedDirectory { get; private set; }

    public String FeedURL { get { return this.URL.Text; } }

    private void okButton_Click(Object sender, RoutedEventArgs e)
    {
      this.FeedDirectory = this.DirectoryPath.Text;
      if (!this.FeedDirectory.EndsWith(@"\", StringComparison.Ordinal))
      {
        this.FeedDirectory += @"\";
      }

      if (!Directory.Exists(this.FeedDirectory))
      {
        var message = String.Format("Directory '{0}' does not exist. Create it now?", this.FeedDirectory);
        var createDirectory = MessageBox.Show(message, "Directory does not exist", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

        if (createDirectory == MessageBoxResult.Cancel)
        {
          return;
        }

        if (createDirectory == MessageBoxResult.Yes)
        {
          Directory.CreateDirectory(this.FeedDirectory);
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
