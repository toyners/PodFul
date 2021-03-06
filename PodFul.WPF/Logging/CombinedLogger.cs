﻿
namespace PodFul.WPF.Logging
{
  using System;
  using Jabberwocky.Toolkit.Object;

  public class CombinedLogger : ILogger
  {
    #region Members
    private readonly FileLogger fileLogger;

    private readonly UILogger uiLogger;
    #endregion

    #region Construction
    public CombinedLogger(FileLogger fileLogger, UILogger uiLogger)
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