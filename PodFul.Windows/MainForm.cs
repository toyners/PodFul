
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class MainForm : Form
  {
    private List<Channel> channels;

    public MainForm()
    {
      InitializeComponent();

      this.channels = new List<Channel>();
    }

    private void AddFeed_Click(Object sender, EventArgs e)
    {
      var form = new AddFeedForm();
      if (form.ShowDialog() == DialogResult.Abort)
      {
        return;
      }

      var feed = ChannelFunctions.CreateChannel(form.FeedURL.Text, form.FeedDirectory.Text);
      this.channels.Add(feed);
    }

    private void removeFeed_Click(Object sender, EventArgs e)
    {

    }

    private void feeds_SelectedIndexChanged(Object sender, EventArgs e)
    {
      int i = 0;
    }
  }
}
