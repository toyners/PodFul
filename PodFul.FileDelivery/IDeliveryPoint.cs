
namespace PodFul.FileDelivery
{
  using System;

  /// <summary>
  /// Provides members for the delivery of single file.
  /// </summary>
  public interface IDeliveryPoint
  {
    #region Properties
    String Description { get; }
    #endregion

    #region Methods
    /// <summary>
    /// Delivers a file.
    /// </summary>
    /// <param name="filePath">Full path of file to be delivered.</param>
    /// <param name="fileTitle">Title of file being delivered. Not necessarily the file name.</param>
    void Deliver(String filePath, String fileTitle);

    /// <summary>
    /// Initialise the delivery point before delivering any files.
    /// </summary>
    void Initialise();
    #endregion
  }
}
