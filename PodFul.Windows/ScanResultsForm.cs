
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class ScanResultsForm : Form
  {
    public ScanResultsForm(IEnumerable<Feed> feeds)
    {
      InitializeComponent();

      List<Podcast> newPodcasts = new List<Podcast>();
      foreach (var feed in feeds)
      {
        var podcasts = FeedFunctions.CreatePodcastList(feed.URL);

        Int32 index = 0;
        while (index < podcasts.Length && index < feed.Podcasts.Length && !podcasts[index].Equals(feed.Podcasts[index]))
        {
          newPodcasts.Add(podcasts[index]);
          index++;
        }
      }

      if (newPodcasts.Count == 0)
      {
        MessageBox.Show("No new podcasts found", "Podful - Scan results", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }
    }
  }
}
