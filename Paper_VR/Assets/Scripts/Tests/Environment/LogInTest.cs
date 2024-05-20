using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// class for the login tests
/// </summary>
public class LogInTest
{

    private GoogleLogin googleLogin;
    private Mock<GoogleDriveAbout.GetRequest> mockRequest;
    private Mock<GoogleDriveFiles.ListRequest> mockListRequest;
    private Mock<GoogleDriveFiles.CreateRequest> mockCreateRequest;
    private Mock<ICoroutineRunner> mockCoroutineRunner;

    [SetUp]
    public void SetUp()
    {
        // Initialize GoogleLogin and its dependencies
        GameObject go = new GameObject();
        googleLogin = go.AddComponent<GoogleLogin>();
        googleLogin.once = false;

        mockRequest = new Mock<GoogleDriveAbout.GetRequest>();
        mockListRequest = new Mock<GoogleDriveFiles.ListRequest>();
        mockCreateRequest = new Mock<GoogleDriveFiles.CreateRequest>();
        mockCoroutineRunner = new Mock<ICoroutineRunner>();

        googleLogin.SetCoroutineRunner(mockCoroutineRunner.Object);
    }

    //[UnityTest]
    public IEnumerator UpdateInfo_ShouldUpdateUserDetails()
    {
        // Arrange
        var mockUser = new User { DisplayName = "Test User", EmailAddress = "test@example.com" };
        var mockResponse = new About { User = mockUser };

        var responseMock2 = new Mock<GoogleDriveRequest<UnityGoogleDrive.Data.About>>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.About>>();
        responseMock2.Setup(a => a.IsDone).Returns(true);
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(responseMock2.Object);

        mockRequest.Setup(r => r.Send()).Returns(responseMock.Object);
        mockRequest.SetupGet(r => r.IsError).Returns(false);
        mockRequest.SetupGet(r => r.ResponseData).Returns(mockResponse);

        googleLogin.Request = mockRequest.Object;
        googleLogin.once = false;

        // Act
        yield return googleLogin.UpdateInfo();

        // Assert
        Assert.AreEqual("Test User", GoogleLogin.name);
        Assert.AreEqual("test@example.com", GoogleLogin.email);
    }

    //[UnityTest]
    public IEnumerator FindId_ShouldSetFolderIdOrCreateFolder()
    {
        // Arrange
        var mockFileList = new FileList { Files = new List<File> { new File { Id = "id" } } };

        var responseMock2 = new Mock<GoogleDriveRequest<UnityGoogleDrive.Data.FileList>>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock2.Setup(a => a.IsDone).Returns(true);
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(responseMock2.Object);

        mockListRequest.Setup(r => r.Send()).Returns(responseMock.Object);
        mockListRequest.SetupGet(r => r.IsError).Returns(false);
        mockListRequest.Setup(r => r.ResponseData).Returns(mockFileList);

        googleLogin.RequestList = mockListRequest.Object;


        Assert.AreEqual(null, GoogleLogin.folderID);


        // Act
        yield return googleLogin.FindId();

        // Assert
        Assert.AreEqual("id", GoogleLogin.folderID);

        GoogleLogin.folderID = null;


    }


    /// <summary>
    /// tests the update info
    /// </summary>
    /// <returns>waits for request to finish</returns>
    /* [UnityTest]
    public IEnumerator UpdateInfoMockTest()
    {
        var googleMock = new Mock<GoogleLogin>();
        var reqMock = new Mock<GoogleDriveAbout.GetRequest>();
        var responseMock2 = new Mock<GoogleDriveRequest<UnityGoogleDrive.Data.About>>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.About>>();
        responseMock2.Setup(a => a.IsDone).Returns(true);
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(responseMock2.Object);
        reqMock.Setup(reqMock => reqMock.ResponseData.User.DisplayName).Returns("a");
        reqMock.Setup(reqMock => reqMock.ResponseData.User.EmailAddress).Returns("b");
        reqMock.Setup(reqMock => reqMock.Send()).Returns(responseMock.Object);
        googleMock.Setup(req => req.Request).Returns(reqMock.Object);
        Assert.AreEqual(null, GoogleLogin.name);
        Assert.AreEqual(null, GoogleLogin.email);
        googleMock.CallBase = true;
        yield return googleMock.Object.UpdateInfo();
        Assert.AreEqual(reqMock.Object, googleMock.Object.Request);
        googleMock.Verify(a => a.UpdateInfo());
        reqMock.Verify(a => a.Send());
        Assert.AreEqual("a", GoogleLogin.name);
        Assert.AreEqual("b", GoogleLogin.email);
    }

    /// <summary>
    /// tests the creating of folders
    /// </summary>
    /// <returns>waits for request to finish</returns>
    [UnityTest]
    public IEnumerator CreateFolderTest()
    {
        var googleMock = new Mock<GoogleLogin>();
        googleMock.CallBase = true;

        var fileMock = new Mock<UnityGoogleDrive.Data.File>();
        fileMock.Setup(f => f.Id).Returns("folder");
        fileMock.Setup(f => f.MimeType).Returns("application/vnd.google-apps.folder");

        var reqMock = new Mock<GoogleDriveFiles.CreateRequest>();
        var reqMock2 = new Mock<GoogleDriveFiles.ListRequest>();

        var responseMock2 = new Mock<GoogleDriveRequest<UnityGoogleDrive.Data.File>>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.File>>();
        responseMock2.Setup(a => a.IsDone).Returns(true);
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(responseMock2.Object);

        var responseMock3 = new Mock<GoogleDriveRequest<UnityGoogleDrive.Data.FileList>>();
        var responseMock4 = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock3.Setup(a => a.IsDone).Returns(true);
        responseMock4.Setup(a => a.GoogleDriveRequest).Returns(responseMock3.Object);

        List<UnityGoogleDrive.Data.File> list = new List<UnityGoogleDrive.Data.File>();
        list.Add(fileMock.Object);

        reqMock.Setup(reqMock => reqMock.Send()).Returns(responseMock.Object);
        reqMock2.Setup(reqMock => reqMock.ResponseData.Files).Returns(list);
        reqMock2.Setup(reqMock => reqMock.Send()).Returns(responseMock4.Object);

        googleMock.Setup(req => req.RequestList).Returns(reqMock2.Object);
        googleMock.Setup(req => req.CreateRequest).Returns(reqMock.Object);

        var mockEnum = new Mock<IEnumerator>();
        mockEnum.CallBase = true;
        googleMock.Setup(req => req.FindId()).Returns(mockEnum.Object);
        Assert.AreEqual(null, GoogleLogin.folderID);

        yield return googleMock.Object.CreateFolder();

        googleMock.Verify(a => a.CreateFolder());
        googleMock.Verify(a => a.FindId());
        reqMock.Verify(a => a.Send());
    }

    /// <summary>
    /// tests the find Id functionallity
    /// </summary>
    /// <returns>waits for request to finish</returns>
    [UnityTest]
    public IEnumerator FindIDTestFolderExists()
    {
        GameObject go = new GameObject();
        GoogleLogin googleMock = go.AddComponent<GoogleLogin>();
        var reqMock = new Mock<GoogleDriveFiles.ListRequest>();
        var responseMock2 = new Mock<GoogleDriveRequest<UnityGoogleDrive.Data.FileList>>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        List<UnityGoogleDrive.Data.File> list = new List<UnityGoogleDrive.Data.File>();
        var fileMock = new Mock<UnityGoogleDrive.Data.File>();
        fileMock.Setup(f => f.Id).Returns("folder");
        list.Add(fileMock.Object);
        responseMock2.Setup(a => a.IsDone).Returns(true);
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(responseMock2.Object);

        reqMock.Setup(reqMock => reqMock.ResponseData.Files).Returns(list);
        reqMock.Setup(reqMock => reqMock.Send()).Returns(responseMock.Object);

        //googleMock.Setup(req => req.RequestList).Returns(reqMock.Object);
        googleMock.RequestList = reqMock.Object;
        Assert.AreEqual(null, GoogleLogin.folderID);

        yield return googleMock.FindId();

        googleMock.Verify(a => a.FindId());
        reqMock.Verify(a => a.Send());

        Assert.AreEqual("folder", GoogleLogin.folderID);

        GoogleLogin.folderID = null;
    }

    /// <summary>
    /// tests the find Id functionallity if the folder doesnt exist
    /// </summary>
    /// <returns>waits for request to finish</returns>
    [UnityTest]
    public IEnumerator FindIDTestFolderDoesntExists()
    {
        var googleMock = new Mock<GoogleLogin>();
        var reqMock = new Mock<GoogleDriveFiles.ListRequest>();
        var responseMock2 = new Mock<GoogleDriveRequest<UnityGoogleDrive.Data.FileList>>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        List<UnityGoogleDrive.Data.File> list = new List<UnityGoogleDrive.Data.File>();
        responseMock2.Setup(a => a.IsDone).Returns(true);
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(responseMock2.Object);

        reqMock.Setup(reqMock => reqMock.ResponseData.Files).Returns(list);
        reqMock.Setup(reqMock => reqMock.Send()).Returns(responseMock.Object);

        googleMock.Setup(req => req.RequestList).Returns(reqMock.Object);
        googleMock.Setup(req => req.CreateFolder()).Verifiable();

        Assert.AreEqual(null, GoogleLogin.folderID);
        googleMock.CallBase = true;
        yield return googleMock.Object.FindId();

        googleMock.Verify(a => a.FindId());
        googleMock.Verify(a => a.CreateFolder());
        reqMock.Verify(a => a.Send());
    }

    /// <summary>
    /// tests the updateFunctionality
    /// </summary>
    [Test]
    public void UpdateTest()
    {
        var googleMock = new Mock<GoogleLogin>();
        var reqMock = new Mock<GoogleDriveAbout.GetRequest>();
        var coroutineRunnerMock = new Mock<ICoroutineRunner>();

        googleMock.Object.SetCoroutineRunner(coroutineRunnerMock.Object);

        reqMock.Setup(reqMock => reqMock.IsDone).Returns(true);
        googleMock.Setup(req => req.Request).Returns(reqMock.Object);

        Assert.AreEqual(true, googleMock.Object.once);

        googleMock.Object.UpdateLogic();

        Assert.AreEqual(false, googleMock.Object.once);
        coroutineRunnerMock.Verify(cr => cr.StartCoroutine(It.IsAny<IEnumerator>()));
    } */
}
