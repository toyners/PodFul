﻿
namespace PodFul.Windows
{
  using System;
  using System.Collections.Generic;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class ScanResultsForm : Form
  {
    public ScanResultsForm(String formTitle, IEnumerable<Podcast> podcasts)
    {
      InitializeComponent();

      foreach (var podcast in podcasts)
      {
        this.podcastList.Rows.Add(
          podcast.Title, 
          podcast.Description, 
          podcast.FileSize, 
          podcast.LatestDownloadDate != DateTime.MinValue ? podcast.LatestDownloadDate.ToString("ddd, dd-MM-yyyy") : @"n\a");
      }

      this.startDownload.Enabled = (this.podcastList.SelectedRows.Count > 0);
      
      this.Text = formTitle;
    }

    private void clearButton_Click(Object sender, EventArgs e)
    {
      this.podcastList.ClearSelection();
      this.startDownload.Enabled = false;
    }

    private void allButton_Click(Object sender, EventArgs e)
    {
      this.podcastList.SelectAll();
      this.startDownload.Enabled = true;
    }

    private void podcastList_SelectionChanged(Object sender, EventArgs e)
    {
      this.startDownload.Enabled = (this.podcastList.SelectedRows.Count > 0);
    }
  }
}
