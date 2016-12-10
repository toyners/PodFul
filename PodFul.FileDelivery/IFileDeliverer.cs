
namespace PodFul.FileDelivery
{
  using System;

  public interface IFileDeliverer
  {
    void InitialiseDeliverypoints();

    void DeliverFileToDeliveryPoints(String filePath, String title);
  }
}
