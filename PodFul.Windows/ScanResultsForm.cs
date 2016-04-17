
namespace PodFul.Windows
{
  using System.Collections.Generic;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class ScanResultsForm : Form
  {
    public ScanResultsForm(IEnumerable<Podcast> podcasts)
    {
      InitializeComponent();

      foreach (var podcast in podcasts)
      {
        this.podcastList.Rows.Add(podcast.Title, podcast.Description, podcast.FileSize);
      }
    }
  }
}
