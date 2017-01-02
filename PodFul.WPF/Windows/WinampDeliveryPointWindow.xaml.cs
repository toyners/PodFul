
namespace PodFul.WPF
{
  using System;
  using System.Windows;

  /// <summary>
  /// Interaction logic for WinampDeliveryPointWindow.xaml
  /// </summary>
  public partial class WinampDeliveryPointWindow : Window
  {
    public WinampDeliveryPointWindow()
    {
      InitializeComponent();
    }

    private void ExecutableSelectionButtonClick(Object sender, RoutedEventArgs e)
    {
      var executableFileDialog = new System.Windows.Forms.OpenFileDialog();
      executableFileDialog.AddExtension = true;
      executableFileDialog.CheckFileExists = true;
      executableFileDialog.DefaultExt = "exe";
      executableFileDialog.Filter = "Executable Files (*.exe)|*.exe";
      executableFileDialog.Title = "Select Winamp Executable";

      if (executableFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
      {
        return;
      }

      this.FilePath.Text = executableFileDialog.FileName;
    }
  }
}
