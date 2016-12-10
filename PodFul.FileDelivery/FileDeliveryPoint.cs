
namespace PodFul.FileDelivery
{
  using System;
  using System.IO;

  public class FileDeliveryPoint : IDeliveryPoint
  {
    private String directoryPath;

    private String destinationPath;

    private Action<String> postMessageMethod;

    private Action<String> postExceptionMethod;

    public FileDeliveryPoint(String directoryPath, Action<String> postMessageMethod, Action<String> postExceptionMethod)
    {
      this.directoryPath = directoryPath;
      this.postMessageMethod = postMessageMethod;
      this.postExceptionMethod = postExceptionMethod;
    }

    public void Deliver(String filePath, String title)
    {
      try
      {
        File.Copy(filePath, this.destinationPath);

        var message = String.Format("Added '{0}' to '{1}'.", title, destinationPath);
        this.postMessageMethod.Invoke(message);
      }
      catch (Exception exception)
      {
        var message = String.Format("Failed to add '{0}' to '{1}': {2}", title, destinationPath, exception.Message);
        this.postExceptionMethod(message);
      }
    }

    public void Initialise()
    {
      var count = 1;
      var destinationPath = String.Empty;
      var directoryName = String.Empty;
      do
      {

      }
      while (Directory.Exists(destinationPath));
      //this.destinationPath = Path.Combine(this.directoryPath, DateTime.Now )
    }
  }
}
