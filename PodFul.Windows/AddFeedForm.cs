
namespace PodFul.Windows
{
  using System;
  using System.Windows.Forms;

  public partial class AddFeedForm : Form
  {
    public AddFeedForm()
    {
      InitializeComponent();
    }

    private void addFeed_Click(Object sender, EventArgs e)
    {
      if (!this.FeedDirectory.Text.EndsWith(@"\"))
      {
        this.FeedDirectory.Text += @"\";
      }

      this.Visible = false;
    }

    private void TextHasChanged(Object sender, EventArgs e)
    {
      this.addFeed.Enabled = (this.FeedDirectory.Text.Length > 0 && this.FeedURL.Text.Length > 0);
    }
  }
}
