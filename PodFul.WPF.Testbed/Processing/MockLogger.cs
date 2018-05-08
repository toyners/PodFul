
namespace PodFul.WPF.Testbed.Processing
{
  using System;
  using Logging;

  public class MockLogger : ILogger
  {
    public void Message(String message)
    {
      throw new NotImplementedException();
    }
  }
}
