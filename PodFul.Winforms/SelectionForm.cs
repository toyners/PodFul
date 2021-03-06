﻿
namespace PodFul.Winforms
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class SelectionForm : Form
  {
    private Boolean selectingFeeds;

    public SelectionForm(String title, IEnumerable<Podcast> podcasts)
    {
      InitializeComponent();

      foreach (var podcast in podcasts)
      {
        this.list.Rows.Add(
          podcast.Title, 
          podcast.Description, 
          podcast.FileDetails.FileSize, 
          podcast.FileDetails.DownloadDate != DateTime.MinValue ? podcast.FileDetails.DownloadDate.ToString("ddd, dd-MMM-yyyy HH:mm:ss") : @"n\a");
      }

      this.start.Enabled = (this.list.SelectedRows.Count > 0);
      this.Text = title;
    }

    public SelectionForm(String title, IFeedStorage feedStorage)
    {
      InitializeComponent();

      list.Columns["fileSizeColumn"].HeaderText = "Count";
      list.Columns["latestDownloadDateColumn"].Visible = false;

      foreach (var feed in feedStorage.Feeds)
      {
        this.list.Rows.Add(
          feed.Title,
          feed.Description,
          feed.Podcasts.Length,
          String.Empty);
      }

      this.Text = title;
      this.selectingFeeds = true;
    }

    public List<Int32> SelectedRowIndexes
    {
      get
      {
        var indexes = new List<Int32>();
        foreach (DataGridViewRow row in this.list.SelectedRows)
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
      this.list.ClearSelection();
    }

    private void allButton_Click(Object sender, EventArgs e)
    {
      this.list.SelectAll();
    }

    private void podcastList_SelectionChanged(Object sender, EventArgs e)
    {
      this.start.Enabled = (this.list.SelectedRows.Count > 0);
    }

    private void SelectionForm_Shown(Object sender, EventArgs e)
    {
      if (this.selectingFeeds)
      {
        this.list.SelectAll();
      }
    }
  }
}
