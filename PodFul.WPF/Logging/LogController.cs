﻿
namespace PodFul.WPF.Logging
{
  using System;
  using System.Collections.Generic;

  public class LogController : ILogController
  {
    private IDictionary<String, ILogger> loggers;

    public LogController(Dictionary<String, ILogger> loggers)
    {
      this.loggers = loggers;
    }

    public void Exception(String key, String message)
    {
      throw new NotImplementedException();
    }

    public ILogger GetLogger(String key)
    {
      throw new NotImplementedException();
    }

    public void Info(String key, String message)
    {
      throw new NotImplementedException();
    }
  }
}
