
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

      this.Text = formTitle;
    }
  }
}
