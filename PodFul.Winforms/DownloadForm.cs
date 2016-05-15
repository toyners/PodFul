
namespace PodFul.Winforms
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class DownloadForm : Form
  {
    public DownloadForm(String formTitle, IEnumerable<Podcast> podcasts)
    {
      InitializeComponent();

      foreach (var podcast in podcasts)
      {
        this.podcastList.Rows.Add(
          podcast.Title, 
          podcast.Description, 
          podcast.FileSize, 
          podcast.DownloadDate != DateTime.MinValue ? podcast.DownloadDate.ToString("ddd, dd-MMM-yyyy HH:mm:ss") : @"n\a");
      }

      this.startDownload.Enabled = (this.podcastList.SelectedRows.Count > 0);
      
      this.Text = formTitle;
    }

    public List<Int32> SelectedRowIndexes
    {
      get
      {
        var indexes = new List<Int32>();
        foreach (DataGridViewRow row in this.podcastList.SelectedRows)
        {
          indexes.Add(row.Index);
        }

        // Sort the indexes into descending order. Podcasts will be downloaded
        // in Chronological order.
        indexes.Sort((x, y) => { return y - x; });

        return indexes;
      }
    }

    private void clearButton_Click(Object sender, EventArgs e)
    {
      this.podcastList.ClearSelection();
    }

    private void allButton_Click(Object sender, EventArgs e)
    {
      this.podcastList.SelectAll();
    }

    private void podcastList_SelectionChanged(Object sender, EventArgs e)
    {
      this.startDownload.Enabled = (this.podcastList.SelectedRows.Count > 0);
    }
  }
}
