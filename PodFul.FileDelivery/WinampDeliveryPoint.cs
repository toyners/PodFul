
namespace PodFul.FileDelivery
{
  using System;

  public class WinampDeliveryPoint : IDeliveryPoint
  {
    private String winampExePath;

    private Action<String> postMessageMethod;

    private Action<String> postExceptionMethod;

    public WinampDeliveryPoint(String winampExePath, Action<String> postMessageMethod, Action<String> postExceptionMethod)
    {
      this.winampExePath = winampExePath;
      this.postMessageMethod = postMessageMethod;
      this.postExceptionMethod = postExceptionMethod;
    }

    public void Deliver(String filePath, String title)
    {
      try
      {
        var arguments = String.Format("/ADD \"{0}\"", filePath);
        System.Diagnostics.Process.Start(this.winampExePath, arguments);

        var message = String.Format("Added '{0}' to Winamp.", title);
        this.postMessageMethod.Invoke(message);
      }
      catch (Exception exception)
      {
        var message = String.Format("Failed to add '{0}' to Winamp: {1}", title, exception.Message);
        this.postExceptionMethod(message);
      }
    }

    public void Initialise() { }
  }
}
