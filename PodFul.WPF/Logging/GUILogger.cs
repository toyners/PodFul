﻿
namespace PodFul.WPF.Logging
{
  using System;

  public class GUILogger : ILogger
  {
    #region Events
    public Action<String> PostMessage;
    #endregion

    #region Methods
    public void Message(String message)
    {
      this.PostMessage?.Invoke(message);
    }
    #endregion
  }
}
