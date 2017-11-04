
namespace PodFul.FileDelivery
{
  using System;
  using Jabberwocky.Toolkit.Object;

  /// <summary>
  /// Contains all delivery points used for delivering files.
  /// </summary>
  public class FileDeliverer : IFileDeliverer
  {
    #region Fields
    private IDeliveryPoint[] deliveryPoints;
    #endregion

    #region Construction
    /// <summary>
    /// Creates a new instance of the FileDeliverer class.
    /// </summary>
    /// <param name="deliveryPoints">Array of delivery points to deliver each podcast to.</param>
    public FileDeliverer(IDeliveryPoint[] deliveryPoints)
    {
      deliveryPoints.VerifyThatObjectIsNotNull("Parameter 'deliveryPoints' is null.");
      this.deliveryPoints = deliveryPoints;
    }
    #endregion

    #region Construction
    /// <summary>
    /// Deliver file using all delivery points.
    /// </summary>
    /// <param name="filePath">Full path to file to be delivered.</param>
    /// <param name="title">Title used when posting success messages or exceptions.</param>
    public void DeliverFileToDeliveryPoints(String filePath, String title)
    {
      foreach (var deliveryPoint in this.deliveryPoints)
      {
        deliveryPoint.Deliver(filePath, title);
      }
    }

    /// <summary>
    /// Finalise all delivery points after delivering all files.
    /// </summary>
    /*public void FinaliseDeliverypoints()
    {
      foreach (var deliveryPoint in this.deliveryPoints)
      {
        deliveryPoint.Finalise();
      }
    }

    /// <summary>
    /// Initialise all delivery points before delivering any files.
    /// </summary>
    public void InitialiseDeliverypoints()
    {
      foreach (var deliveryPoint in this.deliveryPoints)
      {
        deliveryPoint.Initialise();
      }
    }*/
    #endregion
  }
}
