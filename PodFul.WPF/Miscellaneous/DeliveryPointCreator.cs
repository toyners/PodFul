
namespace PodFul.WPF.Miscellaneous
{
  using System;
  using System.Collections.Generic;
  using FileDelivery;
  using Jabberwocky.Toolkit.Object;
  using Logging;

  /// <summary>
  /// Creates delivery point instances given delivery point data.
  /// </summary>
  public static class DeliveryPointCreator
  {
    /// <summary>
    /// Creates an array of delivery point instances based on delivery point data.
    /// </summary>
    /// <param name="deliveryPointData">Data for the delivery points.</param>
    /// <param name="fileDeliveryLogger">Provides logging for delivery points.</param>
    /// <returns>Array of delivery point instances.</returns>
    public static IDeliveryPoint[] CreateDeliveryPoints(List<Settings.SettingsData.DeliveryPointData> deliveryPointData, ILogger fileDeliveryLogger)
    {
      fileDeliveryLogger.VerifyThatObjectIsNotNull("Parameter 'fileDeliveryLogger' is null.");

      if (deliveryPointData == null)
      {
        return new IDeliveryPoint[0];
      }

      var deliveryPoints = new List<IDeliveryPoint>();
      foreach (var deliveryPoint in deliveryPointData)
      {
        if (!deliveryPoint.Enabled)
        {
          continue;
        }

        switch (deliveryPoint.Type)
        {
          case Settings.SettingsData.DeliveryPointData.Types.Winamp:
          deliveryPoints.Add(new WinampDeliveryPoint(deliveryPoint.Location, fileDeliveryLogger.Message, fileDeliveryLogger.Exception));
          break;
          case Settings.SettingsData.DeliveryPointData.Types.Directory:
          deliveryPoints.Add(new FileDeliveryPoint(deliveryPoint.Location, fileDeliveryLogger.Message, fileDeliveryLogger.Exception));
          break;
          default:
          throw new NotImplementedException("Delivery point '" + deliveryPoint.Type + "' not recognised");
        }
      }

      return deliveryPoints.ToArray();
    }
  }
}
