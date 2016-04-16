
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class MainForm : Form
  {
    private List<Feed> feeds;

    public MainForm()
    {
      InitializeComponent();

      this.feeds = new List<Feed>();
    }

    private void AddFeed_Click(Object sender, EventArgs e)
    {
      var form = new AddFeedForm();
      if (form.ShowDialog() == DialogResult.Abort)
      {
        return;
      }

      var feed = FeedFunctions.CreateFeed(form.FeedURL.Text, form.FeedDirectory.Text);
      this.feeds.Add(feed);
    }

    private void removeFeed_Click(Object sender, EventArgs e)
    {
      var index = this.feedList.SelectedIndex;
      this.feeds.RemoveAt(this.feedList.SelectedIndex);

      if (this.feeds.Count == 0)
      {
        this.feedList.SelectedIndex = -1;
        return;
      }

      if (index == 0)
      {
        this.feedList.SelectedIndex = 0;
        return;
      }

      if (index == this.feedList.Items.Count)
      {
        this.feedList.SelectedIndex = this.feedList.Items.Count - 1;
        return;
      }

      this.feedList.SelectedIndex -= 1;
    }

    private void feeds_SelectedIndexChanged(Object sender, EventArgs e)
    {
      this.removeFeed.Enabled = (this.feedList.SelectedIndex != -1);
    }
  }
}
