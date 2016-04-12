
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Data;
  using System.Drawing;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private void AddFeed_Click(Object sender, EventArgs e)
    {
      var form = new AddFeedForm();
      if (form.ShowDialog() == DialogResult.Abort)
      {
        return;
      }

      var url = form.URLText.Text;
      var feed = ChannelFunctions.DownloadRSSFeed(url);
      //feed.Directory = form.DirectoryText.Text;
    }
  }
}
