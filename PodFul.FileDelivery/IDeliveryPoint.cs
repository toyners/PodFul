
namespace PodFul.FileDelivery
{
  using System;

  public interface IDeliveryPoint
  {
    void Initialise();

    void Deliver(String path, String title);
  }
}
