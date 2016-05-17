
namespace PodFul.Winforms
{
  using System;
  using System.IO;
  using System.Windows.Forms;

  public partial class AddFeedForm : Form
  {
    public AddFeedForm()
    {
      InitializeComponent();
    }

    private void addFeed_Click(Object sender, EventArgs e)
    {
      if (!this.FeedDirectory.Text.EndsWith(@"\", StringComparison.Ordinal))
      {
        this.FeedDirectory.Text += @"\";
      }

      if (!Directory.Exists(this.FeedDirectory.Text))
      {
        var message = String.Format("Directory '{0}' does not exist. Create it now?", this.FeedDirectory.Text);
        if (MessageBox.Show(message, "Missing Directory", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
        {
          this.DialogResult = DialogResult.Cancel;
          return;
        }

        Directory.CreateDirectory(this.FeedDirectory.Text);
      }

      this.Visible = false;
    }

    private void TextHasChanged(Object sender, EventArgs e)
    {
      this.addFeed.Enabled = (this.FeedDirectory.Text.Length > 0 && this.FeedURL.Text.Length > 0);
    }

    private void chooseDirectory_Click(Object sender, EventArgs e)
    {
      var dialog = new FolderBrowserDialog();
      dialog.ShowNewFolderButton = true;

      if (dialog.ShowDialog() == DialogResult.Cancel)
      {
        return;
      }

      this.FeedDirectory.Text = dialog.SelectedPath;
    }
  }
}
