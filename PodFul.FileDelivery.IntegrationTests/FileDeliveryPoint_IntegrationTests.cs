﻿
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
    public void Deliver_DeliveringFileToDirectory_FileExists()
    {
      // Arrange
      var fileName = Path.GetRandomFileName();
      var filePath = Path.Combine(workingDirectory, fileName);
      var fileStream = File.Create(filePath);
      fileStream.WriteByte(1);
      fileStream.Close();

      var destinationPath = Path.Combine(workingDirectory);
      var expectedDestinationFilePath = Path.Combine(destinationPath, DateTime.Now.ToString("dd-MM-yyyy") + "_1");
      expectedDestinationFilePath = Path.Combine(expectedDestinationFilePath, fileName);

      var fileDeliveryPoint = new FileDeliveryPoint(destinationPath, null, null);

      // Act
      fileDeliveryPoint.Deliver(filePath, "file");

      // Assert
      File.Exists(expectedDestinationFilePath).ShouldBeTrue();
    }
    #endregion 
  }
}
