
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Collections.Generic;
  using FileDelivery;
  using Jabberwocky.Toolkit.Object;

  public static class DeliveryPointCreator
  {
    public static IDeliveryPoint[] CreateDeliveryPoints(List<Settings.SettingsData.DeliveryPointData> deliveryPointData, ILogger fileDeliveryLogger)
    {
      fileDeliveryLogger.VerifyThatObjectIsNotNull("Parameter 'fileDeliveryLogger' is null.");
      var deliveryPoints = new IDeliveryPoint[deliveryPointData.Count];
      var index = 0;

      foreach (var data in deliveryPointData)
      {
        switch (data.Type)
        {
          case Settings.SettingsData.DeliveryPointData.Types.Winamp:
          deliveryPoints[index] = new WinampDeliveryPoint(data.Location, fileDeliveryLogger.Message, fileDeliveryLogger.Exception);
          break;
          case Settings.SettingsData.DeliveryPointData.Types.Directory:
          deliveryPoints[index] = new FileDeliveryPoint(data.Location, fileDeliveryLogger.Message, fileDeliveryLogger.Exception);
          break;
          default:
          throw new NotImplementedException("Delivery point '" + data.Type + "' not recognised");
        }

        index++;
      }

      return deliveryPoints;
    }
  }
}
