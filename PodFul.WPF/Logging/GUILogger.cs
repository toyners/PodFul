﻿
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
}
