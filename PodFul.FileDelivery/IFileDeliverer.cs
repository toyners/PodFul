
namespace PodFul.FileDelivery
{
  using System;

  /// <summary>
  /// Provides members for the delivery of single file to multiple delivery points.
  /// </summary>
  public interface IFileDeliverer
  {
    #region Methods
    /// <summary>
    /// Delivers a file to all delivery points.
    /// </summary>
    /// <param name="filePath">Full path of file to be delivered.</param>
    /// <param name="fileTitle">Title of file being delivered. Not necessarily the file name.</param>
    void DeliverFileToDeliveryPoints(String filePath, String fileTitle);

    /// <summary>
    /// Finalise all delivery points after delivering all files.
    /// </summary>
    //void FinaliseDeliverypoints();
  
    /// <summary>
    /// Initialise all delivery points before delivering any files.
    /// </summary>
    //void InitialiseDeliverypoints();
    #endregion
  }
}
