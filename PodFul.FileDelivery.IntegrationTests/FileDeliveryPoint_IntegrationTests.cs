
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
      DirectoryOperations.EnsureDirectoryIsEmpty(this.workingDirectory);
    }

    [Test]
    public void Test()
    {
      var filePath = Path.Combine(workingDirectory, Path.GetTempFileName());
      var destinationPath = Path.Combine(workingDirectory, "Destination");
      var fileDeliveryPoint = new FileDeliveryPoint(destinationPath, null, null);

      fileDeliveryPoint.Deliver(filePath, "file");


    }
    #endregion 
  }
}
