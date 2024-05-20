using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGoogleDrive;

/// <summary>
/// class for the login tests
/// </summary>
public class LogInTest
{
    /// <summary>
    /// tests the update info
    /// </summary>
    /// <returns>waits for request to finish</returns>
    [UnityTest]
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
        var googleMock = new Mock<GoogleLogin>();
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

        googleMock.Setup(req => req.RequestList).Returns(reqMock.Object);
        Assert.AreEqual(null, GoogleLogin.folderID);
        googleMock.CallBase = true;
        yield return googleMock.Object.FindId();

        googleMock.Verify(a => a.FindId());
        reqMock.Verify(a => a.Send());

        Assert.AreEqual("folder", GoogleLogin.folderID);
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
    }
}
