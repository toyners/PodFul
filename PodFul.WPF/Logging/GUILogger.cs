
namespace PodFul.WPF.Logging
{
  using System;
  using Jabberwocky.Toolkit.Object;

  public class GUILogger : ILogger
  {
    #region Members
    private const String lineBreakText = "\r\n";

    private readonly FileLogger fileLogger;
    #endregion

    #region Construction
    public GUILogger(FileLogger logger)
    {
      this.fileLogger = logger;
    }
    #endregion

    #region Events
    public Action<String> PostMessage;
    #endregion

    #region Methods
    public void Message(String message)
    {
      this.Message(message, true);
    }

    public void Message(String message, Boolean lineBreak)
    {
      if (!String.IsNullOrEmpty(message))
      {
        this.fileLogger.Message(message);
      }

      this.Post(message, lineBreak);
    }

    private void Post(String message, Boolean lineBreak)
    {
      if (this.PostMessage != null)
      {
        message += (lineBreak ? lineBreakText : null);
        this.PostMessage(message);
      }
    }
    #endregion
  }

  public class CombinedLogger : ILogger
  {
    #region Members
    private readonly FileLogger fileLogger;

    private readonly GUILogger uiLogger;
    #endregion

    #region Construction
    public CombinedLogger(FileLogger fileLogger, GUILogger uiLogger)
    {
      fileLogger.VerifyThatObjectIsNotNull("Parameter 'fileLogger' is null.");
      uiLogger.VerifyThatObjectIsNotNull("Parameter 'uiLogger' is null.");
      this.fileLogger = fileLogger;
      this.uiLogger = uiLogger;
    }
    #endregion

    #region Methods
    public void Message(String message)
    {
      this.fileLogger.Message(message);
      this.uiLogger.Message(message);
    }
    #endregion
  }
}
