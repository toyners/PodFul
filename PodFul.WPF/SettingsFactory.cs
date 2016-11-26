
namespace PodFul.WPF
{
  using System;
  using System.IO;
  using Jabberwocky.Toolkit.Object;

  public static class SettingsFactory
  {
    public static Settings Create(String filePath, ILogger fileDeliveryLogger)
    {
     /* fileDeliveryLogger.VerifyThatObjectIsNotNull("Parameter 'fileDeliveryLogger' is null.");
      if (String.IsNullOrEmpty(filePath) || !File.Exists(filePath))
      {
        return new Settings(fileDeliveryLogger);
      }*/


      throw new NotImplementedException();
    }

    public static void Save(String filePath, Settings settings)
    {

    }
  }
}
