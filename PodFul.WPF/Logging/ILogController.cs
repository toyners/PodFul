﻿
namespace PodFul.WPF.Logging
{
  using System;

  public interface ILogController
  {
    ILogger GetLogger(String key);

    T GetLogger<T>(String key);

    void Message(String key, String message);
  }
}
