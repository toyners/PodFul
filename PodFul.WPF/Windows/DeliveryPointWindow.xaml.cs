
namespace PodFul.WPF
{
  using System;
  using System.IO;
  using System.Windows;
  using Jabberwocky.Toolkit.Path;
  using Miscellaneous;

  /// <summary>
  /// Interaction logic for DeliveryPointWindow.xaml
  /// </summary>
  public partial class DeliveryPointWindow : Window
  {
    #region Fields
    private Settings.SettingsData.DeliveryPointData.Types type;
    #endregion

    #region Construction
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
    #endregion

    #region Methods
    private Boolean ConfirmDirectoryPath()
    {
      if (Directory.Exists(this.FullPath.Text))
      {
        return true;
      }

      var message = "Directory does not exist. Create it now?\r\n\r\n(Warning: Not creating the directory now may cause file creation issues later on).";
      var result = MessageBox.Show(message, "Create Directory?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes);

      if (result == MessageBoxResult.Cancel)
      {
        return false;
      }

      if (result == MessageBoxResult.Yes)
      {
        Directory.CreateDirectory(this.FullPath.Text);
      }

      return true;
    }

    private void FullPathTextChanged(Object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
      this.OKButton.IsEnabled = (this.FullPath.Text.Length > 0);
    }

    private void OKButtonClick(Object sender, RoutedEventArgs e)
    {
      if (this.type == Settings.SettingsData.DeliveryPointData.Types.Directory)
      {
        this.FullPath.Text = PathOperations.CompleteDirectoryPath(this.FullPath.Text);

        if (!this.ConfirmDirectoryPath())
        {
          return;
        }
      }
      else if (this.type == Settings.SettingsData.DeliveryPointData.Types.Winamp && !File.Exists(this.FullPath.Text))
      {
        MessageBox.Show("Winamp executable file does not exist. Please check the file selection.", "File does not Exist", MessageBoxButton.OK, MessageBoxImage.Error);
        return;
      }

      this.DialogResult = true;
      this.Close();
    }

    private void SelectButtonClick(Object sender, RoutedEventArgs e)
    {
      if (this.type == Settings.SettingsData.DeliveryPointData.Types.Winamp)
      {
        this.SelectFile();
      }
      else
      {
        this.SelectDirectory();
      }
    }

    private void SelectDirectory()
    {
      var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
      if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
      {
        return;
      }

      this.FullPath.Text = folderBrowserDialog.SelectedPath;
    }

    private void SelectFile()
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

    private void WindowClosing(Object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (!this.DialogResult.HasValue)
      {
        this.DialogResult = false;
      }
    }
    #endregion
  }
}
