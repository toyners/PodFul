
namespace PodFul.WPF.UI_Support
{
  using Processing;

  public interface IJobCountDisplayManager
  {
    void DisplayCounts();

    void UpdateCounts(DownloadJob job);
  }
}
