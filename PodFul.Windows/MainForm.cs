
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
      var index = this.feeds.SelectedIndex;
      this.channels.RemoveAt(this.feeds.SelectedIndex);

      if (this.channels.Count == 0)
      {
        this.feeds.SelectedIndex = -1;
        return;
      }

      if (index == 0)
      {
        this.feeds.SelectedIndex = 0;
        return;
      }

      if (index == this.feeds.Items.Count)
      {
        this.feeds.SelectedIndex = this.feeds.Items.Count - 1;
        return;
      }

      this.feeds.SelectedIndex -= 1;
    }

    private void feeds_SelectedIndexChanged(Object sender, EventArgs e)
    {
      this.removeFeed.Enabled = (this.feeds.SelectedIndex != -1);
    }
  }
}
