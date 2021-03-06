﻿
namespace PodFul.FileDelivery
{
  using System;
  using Jabberwocky.Toolkit.String;

  /// <summary>
  /// Delivers files to Winamp playlist.
  /// </summary>
  public class WinampDeliveryPoint : IDeliveryPoint
  {
    #region Fields
    private String winampExePath;

    private Action<String> postMessageMethod;

    private Action<String> postExceptionMethod;
    #endregion

    #region Construction
    /// <summary>
    /// Creates a new instance of the WinampDeliveryPoint class.
    /// </summary>
    /// <param name="winampExePath">Full path to Winamp executable.</param>
    /// <param name="postMessageMethod">Method used to post file delivery success message. Can be null.</param>
    /// <param name="postExceptionMethod">Method used to post exception messages. Can be null.</param>
    public WinampDeliveryPoint(String winampExePath, Action<String> postMessageMethod, Action<String> postExceptionMethod)
    {
      winampExePath.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'winampExePath' is null or empty.");
      this.winampExePath = winampExePath;
      this.postMessageMethod = postMessageMethod;
      this.postExceptionMethod = postExceptionMethod;

      this.Description = "Adds podcast files to Winamp playlist";
    }
    #endregion

    #region Properties
    public String Description { get; private set; }
    #endregion

    #region Methods
    /// <summary>
    /// Delivers a file to winamp playlist.
    /// </summary>
    /// <param name="filePath">Full path to file to be delivered.</param>
    /// <param name="fileTitle">Title used when posting success messages or exceptions.</param>
    public void Deliver(String filePath, String fileTitle)
    {
      try
      {
        var arguments = String.Format("/ADD \"{0}\"", filePath);
        System.Diagnostics.Process.Start(this.winampExePath, arguments);

        var message = String.Format("Added '{0}' to Winamp.", fileTitle);
        this.postMessageMethod.Invoke(message);
      }
      catch (Exception exception)
      {
        var message = String.Format("Failed to add '{0}' to Winamp: {1}", fileTitle, exception.Message);
        this.postExceptionMethod(message);
      }
    }

    /// <summary>
    /// Finalises the winamp delivery point.
    /// </summary>
    /*public void Finalise()
    {
      // Nothing to do 
    }

    /// <summary>
    /// Initialises the winamp delivery point.
    /// </summary>
    public void Initialise()
    {
      // Nothing to do 
    }*/
    #endregion
  }
}
