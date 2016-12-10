
namespace PodFul.FileDelivery
{
  using System;

  public class FileDeliverer : IFileDeliverer
  {
    private IDeliveryPoint[] deliveryPoints;

    public FileDeliverer(IDeliveryPoint[] deliveryPoints)
    {
      this.deliveryPoints = deliveryPoints;
    }

    public void DeliverFileToDeliveryPoints(String filePath, String title)
    {
      foreach (var deliveryPoint in this.deliveryPoints)
      {
        deliveryPoint.Deliver(filePath, title);
      }
    }

    public void InitialiseDeliverypoints()
    {
      foreach (var deliveryPoint in this.deliveryPoints)
      {
        deliveryPoint.Initialise();
      }
    }
  }
}
