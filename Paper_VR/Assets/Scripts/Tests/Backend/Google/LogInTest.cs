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
[TestFixture]
public class LogInTest
{
    private GameObject gameObject;
    private GoogleLogin googleLogin;
    private Mock<ICoroutineRunner> mockCoroutineRunner;
    private Mock<GoogleDriveAbout.GetRequest> mockGetRequest;
    private Mock<GoogleDriveFiles.ListRequest> mockListRequest;
    private Mock<GoogleDriveFiles.CreateRequest> mockCreateRequest;

    /// <summary>
    /// Set up the test
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.gameObject = new GameObject();
        this.googleLogin = this.gameObject.AddComponent<GoogleLogin>();
        this.mockCoroutineRunner = new Mock<ICoroutineRunner>();
        this.mockGetRequest = new Mock<GoogleDriveAbout.GetRequest>();
        this.mockListRequest = new Mock<GoogleDriveFiles.ListRequest>();
        this.mockCreateRequest = new Mock<GoogleDriveFiles.CreateRequest>();

        this.googleLogin.SetCoroutineRunner(this.mockCoroutineRunner.Object);
        this.googleLogin.Request = this.mockGetRequest.Object;
        this.googleLogin.once = false;
    }

    /// <summary>
    /// Tear down the test
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(this.gameObject);
    }

    /// <summary>
    /// tests the set and get coroutine runner
    /// </summary>
    [Test]
    public void SetCoroutineRunner_SetsCoroutineRunner()
    {
        var runner = new Mock<ICoroutineRunner>().Object;
        this.googleLogin.SetCoroutineRunner(runner);
        Assert.AreEqual(runner, this.googleLogin.GetCoroutineRunner());
    }

    [UnityTest]
    public IEnumerator TestTest()
    {
        yield return new WaitForSeconds(3.0f);

        Assert.IsTrue(true);
    }

    /// <summary>
    /// Tests the update info
    /// </summary>
    /// <returns>Its a unity test, so it returns an enumerator</returns>
    [UnityTest]
    public IEnumerator UpdateInfo_SetsNameAndEmail()
    {
        var mockUser = new UnityGoogleDrive.Data.User
        {
            DisplayName = "Test User",
            EmailAddress = "test@example.com",
        };
        var mockResponse = new UnityGoogleDrive.Data.About
        {
            User = mockUser,
        };
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.About>>();
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(this.mockGetRequest.Object);

        this.mockGetRequest.Setup(req => req.Send()).Returns(responseMock.Object);
        this.mockGetRequest.Setup(req => req.ResponseData).Returns(mockResponse);

        yield return this.googleLogin.UpdateInfo(this.mockGetRequest.Object);

        Assert.AreEqual("Test User", GoogleLogin.name);
        Assert.AreEqual("test@example.com", GoogleLogin.email);
    }

    /// <summary>
    /// Tests the create folder when no Id is found
    /// </summary>
    /// <returns>Its a unity test, so it returns an Enumerator</returns>
    [UnityTest]
    public IEnumerator FindId_CreatesFolderWhenNotFound()
    {
        Mock<GoogleLogin> mock = new Mock<GoogleLogin>();

        mock.Object.SetCoroutineRunner(this.mockCoroutineRunner.Object);

        mock.Setup(m => m.Request).Returns(this.mockGetRequest.Object);

        var responseMock1 = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock1.Setup(a => a.GoogleDriveRequest).Returns(this.mockListRequest.Object);
        this.mockListRequest.Setup(req => req.IsDone).Returns(true);

        var responseMock2 = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.File>>();
        responseMock2.Setup(a => a.GoogleDriveRequest).Returns(this.mockCreateRequest.Object);
        this.mockCreateRequest.Setup(req => req.IsDone).Returns(true);

        this.mockListRequest.Setup(req => req.Send()).Returns(responseMock1.Object);
        List<File> files = new List<File>();
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(files);

        this.mockCreateRequest.Setup(req => req.Send()).Returns(responseMock2.Object);
        this.mockCreateRequest.Setup(req => req.ResponseData.Id).Returns("folder");

        mock.Setup(m => m.MakeCreateRequest()).Returns(this.mockCreateRequest.Object);
        mock.CallBase = true;

        yield return mock.Object.FindId(this.mockListRequest.Object, true);

        this.mockListRequest.Verify(a => a.Send());
        this.mockCreateRequest.Verify(b => b.Send());
        Assert.AreEqual("folder", GoogleLogin.folderID);
    }

    /// <summary>
    /// tests the find id functionallity
    /// </summary>
    /// <returns>Its a unity test, so it returns an Enumerator</returns>
    [UnityTest]
    public IEnumerator FindId_CreatesFolderWhenFound()
    {
        Mock<GoogleLogin> mock = new Mock<GoogleLogin>();

        mock.Object.SetCoroutineRunner(this.mockCoroutineRunner.Object);

        var responseMock1 = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock1.Setup(a => a.GoogleDriveRequest).Returns(this.mockListRequest.Object);
        this.mockListRequest.Setup(req => req.IsDone).Returns(true);

        this.mockListRequest.Setup(req => req.Send()).Returns(responseMock1.Object);
        List<File> files = new List<File>();
        File file = new File { Id = "folder" };
        files.Add(file);
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(files);
        mock.CallBase = true;

        yield return mock.Object.FindId(this.mockListRequest.Object, true);

        this.mockListRequest.Verify(a => a.Send());
        Assert.AreEqual("folder", GoogleLogin.folderID);
    }

    /// <summary>
    /// Tests the create request functionallity
    /// </summary>
    [Test]
    public void MakeCreateRequest_ReturnsCreateRequest()
    {
        var request = this.googleLogin.MakeCreateRequest();
        Assert.AreEqual("application/vnd.google-apps.folder", request.RequestData.MimeType);
        Assert.AreEqual("root", request.RequestData.Parents[0]);
    }
}
