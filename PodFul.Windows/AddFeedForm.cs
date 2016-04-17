
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
      this.Visible = false;
    }
  }
}
