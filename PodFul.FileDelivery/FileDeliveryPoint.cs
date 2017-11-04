
namespace PodFul.FileDelivery
{
  using System;
  using System.IO;
  using Jabberwocky.Toolkit.IO;
  using Jabberwocky.Toolkit.String;

  /// <summary>
  /// Copies files from different locations to one central location.
  /// </summary>
  public class FileDeliveryPoint : IDeliveryPoint
  {
    #region Fields
    private String baseDirectoryPath;

    private String directoryPath;

    private Action<String> postMessageMethod;

    private Action<String> postExceptionMethod;
    #endregion

    #region Construction
    /// <summary>
    /// Creates a new instance of the FileDeliveryPoint class.
    /// </summary>
    /// <param name="baseDirectoryPath">Base directory path to create the destination directory within.</param>
    /// <param name="postMessageMethod">Method used to post file delivery success message. Can be null.</param>
    /// <param name="postExceptionMethod">Method used to post exception messages. Can be null.</param>
    public FileDeliveryPoint(String baseDirectoryPath, Action<String> postMessageMethod, Action<String> postExceptionMethod)
    {
      baseDirectoryPath.VerifyThatStringIsNotNullAndNotEmpty("Parameter 'baseDirectoryPath' is null or empty.");
      this.baseDirectoryPath = baseDirectoryPath;
      this.postMessageMethod = postMessageMethod;
      this.postExceptionMethod = postExceptionMethod;

      this.Description = "Copies podcast files to '" + this.baseDirectoryPath + "'";
    }
    #endregion

    #region Properties
    public String Description { get; private set; }
    #endregion

    #region Methods
    /// <summary>
    /// Delivers (copies) a file to a single destination directory that is created during Initialise method. 
    /// </summary>
    /// <param name="filePath">Full path to file to be delivered to destination directory.</param>
    /// <param name="fileTitle">Title used when posting success messages or exceptions.</param>
    public void Deliver(String filePath, String fileTitle)
    {
      try
      {
        this.EnsureDestinationDirectoryExists();

        var fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
        var destinationFilePath = Path.Combine(this.directoryPath, fileName);

        File.Copy(filePath, destinationFilePath);

        var message = String.Format("Added '{0}' to '{1}'.", fileTitle, directoryPath);
        this.postMessageMethod?.Invoke(message);
      }
      catch (Exception exception)
      {
        var message = String.Format("Failed to add '{0}' to '{1}': {2}", fileTitle, directoryPath, exception.Message);
        this.postExceptionMethod?.Invoke(message);
      }
    }

    private void EnsureDestinationDirectoryExists()
    {
      if (String.IsNullOrEmpty(this.directoryPath))
      {
        var count = 1;
        var destinationPath = String.Empty;
        var directoryName = String.Empty;
        var today = DateTime.Today.ToString("dd-MM-yyyy");
        do
        {
          directoryName = today + "_" + (count++);
          destinationPath = Path.Combine(this.baseDirectoryPath, directoryName);
        }
        while (Directory.Exists(destinationPath));

        this.directoryPath = destinationPath;
      }

      DirectoryOperations.EnsureDirectoryExists(this.directoryPath);
    }

    /// <summary>
    /// Delete the destination directory if no files delivered there.
    /// </summary>
    /*public void Finalise()
    {
      if (Directory.Exists(this.directoryPath) && Directory.GetFiles(this.directoryPath).Length == 0)
      {
        try
        {
          Directory.Delete(this.directoryPath, true);
        }
        catch
        {
          // Ignore any exception raised when trying to delete the directory. It is not 
          // vital that the directory remains.
        }
      }
    }

    /// <summary>
    /// Initialises the destination directory for file delivery. Destination directory is based on base directory path set during
    /// construction and a date timestamp. Simple counter is appended to destination directory name in case of duplicates. 
    /// </summary>
    public void Initialise()
    {
      var count = 1;
      var destinationPath = String.Empty;
      var directoryName = String.Empty;
      var today = DateTime.Today.ToString("dd-MM-yyyy");
      do
      {
        directoryName = today + "_" + (count++);
        destinationPath = Path.Combine(this.baseDirectoryPath, directoryName);
      }
      while (Directory.Exists(destinationPath));

      this.directoryPath = destinationPath;
      DirectoryOperations.EnsureDirectoryExists(this.directoryPath);
    }*/
    #endregion
  }
}
