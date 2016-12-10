
namespace PodFul.FileDelivery.IntegrationTests
{
  using System;
  using System.IO;
  using FileDelivery;
  using Jabberwocky.Toolkit.IO;
  using NUnit.Framework;
  using Shouldly;

  [TestFixture]
  public class FileDeliveryPoint_IntegrationTests
  {
    private String workingDirectory;

    #region Methods
    [SetUp]
    public void SetupBeforeEachTest()
    {
      this.workingDirectory = Path.Combine(Path.GetTempPath(), "FileDeliveryPoint_IntegrationTests");
      DirectoryOperations.EnsureDirectoryIsEmpty(this.workingDirectory);
    }

    [Test]
    public void Test()
    {
      // Arrange
      var fileName = Path.GetRandomFileName();
      var filePath = Path.Combine(workingDirectory, fileName);
      var fileStream = File.Create(filePath);
      fileStream.WriteByte(1);
      fileStream.Close();

      var destinationPath = Path.Combine(workingDirectory);
      var expectedDestinationFilePath = Path.Combine(destinationPath, DateTime.Now.ToString("dd-mm-yyyy") + "_1");
      expectedDestinationFilePath = Path.Combine(expectedDestinationFilePath, fileName);

      var fileDeliveryPoint = new FileDeliveryPoint(destinationPath, this.PostMessage, this.PostException);

      // Act
      fileDeliveryPoint.Initialise();
      fileDeliveryPoint.Deliver(filePath, "file");

      // Assert
      File.Exists(expectedDestinationFilePath).ShouldBeTrue();
    }

    private void PostMessage(String message) { }

    private void PostException(String message) { }
    #endregion 
  }
}
