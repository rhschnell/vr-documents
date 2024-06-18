using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// class for the google methods tests
/// </summary>
[TestFixture]
public class GoogleMethodsTest
{
    private GoogleMethods googleMethods;
    private Mock<ICoroutineRunner> mockCoroutineRunner;
    private Mock<GoogleDriveFiles.ListRequest> mockListRequest;

    /// <summary>
    /// Set up the test
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.googleMethods = new GoogleMethods();
        this.mockCoroutineRunner = new Mock<ICoroutineRunner>();
        this.mockListRequest = new Mock<GoogleDriveFiles.ListRequest>();
    }

    /// <summary>
    /// This test checks if the FindEnviormentId method finds the folder
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator FindEnviormentId_FindsFolder()
    {
        // Create a mock response
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(this.mockListRequest.Object);
        this.mockListRequest.Setup(req => req.IsDone).Returns(true);
        this.mockListRequest.Setup(req => req.Send()).Returns(responseMock.Object);

        // Create a list of files
        List<File> files = new List<File>();
        File file = new File { Id = "folder" };
        files.Add(file);
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(files);

        // Act
        yield return this.googleMethods.FindEnviormentId("folder", this.mockListRequest.Object);

        this.mockListRequest.Verify(a => a.Send());
        Assert.AreEqual("folder", this.googleMethods.currentEnvId);
    }

    /// <summary>
    /// This test checks if the FindEnviormentId method does not find the folder
    /// It should set the currentEnvId to "no folder"
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator FindEnviormentId_DoesNotFindFolder()
    {
        // Create a mock response
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(this.mockListRequest.Object);
        this.mockListRequest.Setup(req => req.IsDone).Returns(true);
        this.mockListRequest.Setup(req => req.Send()).Returns(responseMock.Object);

        // Create a list of files
        List<File> files = new List<File>();
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(files);

        // Act
        yield return this.googleMethods.FindEnviormentId("folder", this.mockListRequest.Object);

        this.mockListRequest.Verify(a => a.Send());
        Assert.AreEqual("no folder", this.googleMethods.currentEnvId);
    }

    /// <summary>
    /// This test checks if the DeleteFile method deletes a file
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator DeleteAFileTest()
    {
        // Create a mock response
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(this.mockListRequest.Object);
        this.mockListRequest.Setup(req => req.IsDone).Returns(true);
        this.mockListRequest.Setup(req => req.Send()).Returns(responseMock.Object);

        // Create a list of files
        List<File> files = new List<File>();
        File file = new File { Id = "file" };
        file.Parents = new List<string> { "folder" };
        files.Add(file);
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(files);

        // Act
        yield return this.googleMethods.DeleteFile("file", "folder", this.mockListRequest.Object, true);

        this.mockListRequest.Verify(a => a.Send());
    }

    /// <summary>
    /// This test checks if the DeleteFile method does not delete a file
    /// When the file is not found
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator DeleteNoFile()
    {
        // Create a mock response
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(this.mockListRequest.Object);
        this.mockListRequest.Setup(req => req.IsDone).Returns(true);
        this.mockListRequest.Setup(req => req.Send()).Returns(responseMock.Object);

        // Create a list of files
        List<File> files = new List<File>();
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(files);

        // Act
        yield return this.googleMethods.DeleteFile("file", "folder", this.mockListRequest.Object, true);

        this.mockListRequest.Verify(a => a.Send());
    }

    /// <summary>
    /// This test checks if the CreateJsonFile method creates a json file
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator CreateJsonFileTest()
    {
        Mock<UnityGoogleDrive.GoogleDriveRequest<UnityGoogleDrive.Data.File>> mockFile = new Mock<GoogleDriveRequest<File>>();
        Mock<UnityGoogleDrive.GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>> mockFileYield = new Mock<GoogleDriveRequestYieldInstruction<FileList>>();
        // Create a mock response
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.File>>();
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(mockFile.Object);

        // Create a mock for the create request
        Mock<GoogleDriveFiles.CreateRequest> mockCreateRequest = new Mock<GoogleDriveFiles.CreateRequest>();

        // Set up the create request
        mockCreateRequest.Setup(req => req.IsDone).Returns(true);
        mockCreateRequest.Setup(req => req.Send()).Returns(responseMock.Object);

        // Act
        yield return this.googleMethods.CreateJsonFile("id", new byte[0], mockCreateRequest.Object, true);

        mockCreateRequest.Verify(a => a.Send());
    }

    /// <summary>
    /// Tests the MakeRequest method.
    /// </summary>
    [Test]
    public void MakeRequest_ReturnsCreateRequest()
    {
        var name = "TestName";

        GoogleDriveFiles.CreateRequest result = this.googleMethods.MakeRequest(name);

        // Assert request is created
        Assert.IsNotNull(result);
        Assert.AreEqual(name, result.RequestData.Name);
        Assert.AreEqual("application/vnd.google-apps.folder", result.RequestData.MimeType);
    }
}
