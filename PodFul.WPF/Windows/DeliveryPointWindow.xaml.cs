
namespace PodFul.WPF
{
  using System;
  using System.Windows;
  using Miscellaneous;

  /// <summary>
  /// Interaction logic for DeliveryPointWindow.xaml
  /// </summary>
  public partial class DeliveryPointWindow : Window
  {
    private Settings.SettingsData.DeliveryPointData.Types type;

    public DeliveryPointWindow(Settings.SettingsData.DeliveryPointData.Types type, String title)
    {
      InitializeComponent();

      this.type = type;
      this.Title = title;

      if (this.type == Settings.SettingsData.DeliveryPointData.Types.Directory)
      {
        this.FullPathTitle.Content = "Directory Path:";
      }
    }

    public DeliveryPointWindow(Settings.SettingsData.DeliveryPointData.Types type, String title, String fullPath) : this(type, title)
    {
      this.FullPath.Text = fullPath;
    }

    private void SelectButtonClick(Object sender, RoutedEventArgs e)
    {
      var executableFileDialog = new System.Windows.Forms.OpenFileDialog();
      executableFileDialog.AddExtension = true;
      executableFileDialog.CheckFileExists = true;
      executableFileDialog.DefaultExt = "exe";
      executableFileDialog.Filter = "Executable Files (*.exe)|*.exe";
      executableFileDialog.Title = "Select Winamp Executable File";

      if (executableFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
      {
        return;
      }

      this.FullPath.Text = executableFileDialog.FileName;
    }

    private void FullPathTextChanged(Object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      this.OKButton.IsEnabled = (this.FullPath.Text.Length > 0);
    }

    private void WindowClosing(Object sender, System.ComponentModel.CancelEventArgs e)
    {

    }

    private void OKButtonClick(Object sender, RoutedEventArgs e)
    {

    }
  }
}
