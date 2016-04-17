
namespace PodFul.Windows
{
  using System.Collections.Generic;
  using System.Windows.Forms;
  using PodFul.Library;

  public partial class ScanResultsForm : Form
  {
    public ScanResultsForm(List<Podcast> podcasts)
    {
      InitializeComponent();
    }
  }
}
