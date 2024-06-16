using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// The class that handles the logout testing
/// </summary>
public class LogoutTest
{
    private GameObject go;
    private GoogleLogOut googleLogOut;
    private Mock<GoogleDriveSettings> mockSettings;
    private Mock<GoogleLogin> mockGoogleLogin;
    private Mock<ICoroutineRunner> mockCoroutineRunner;

    /// <summary>
    /// setUp the test
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Initialize GoogleLogOut and its dependencies
        this.go = new GameObject();
        this.googleLogOut = this.go.AddComponent<GoogleLogOut>();

        this.mockSettings = new Mock<GoogleDriveSettings>();
        this.mockGoogleLogin = new Mock<GoogleLogin>();
        this.mockCoroutineRunner = new Mock<ICoroutineRunner>();

        this.googleLogOut.Settings = this.mockSettings.Object;
        this.googleLogOut.GoogleLogin = this.mockGoogleLogin.Object;
    }

    /// <summary>
    /// Clean up the test
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(this.go);
    }

    /// <summary>
    /// Makes sure that the logout function works as expected
    /// </summary>
    [Test]
    public void LogOutTest()
    {
        Mock<GoogleLogOut> mock = new Mock<GoogleLogOut>();
        mock.Setup(log => log.Settings).Returns(this.mockSettings.Object);
        mock.Setup(log => log.GoogleLogin).Returns(this.mockGoogleLogin.Object);
        mock.Setup(log => log.CoroutineRunner).Returns(this.mockCoroutineRunner.Object);

        this.mockSettings.Setup(settings => settings.IsAnyAuthTokenCached()).Returns(false);
        this.mockGoogleLogin.Setup(login => login.UpdateInfo(It.IsAny<GoogleDriveAbout.GetRequest>()));

        mock.Object.LogOut();

        this.mockSettings.Verify(settings => settings.DeleteCachedAuthTokens());
        this.mockSettings.Verify(settings => settings.IsAnyAuthTokenCached());
    }

    /// <summary>
    /// Makes sure that the login function works as expected
    /// </summary>
    /// <returns>Its a unity test, so it returns an Enumator</returns>
    [UnityTest]
    public IEnumerator LoginTest()
    {
        Mock<GoogleLogOut> mock = new Mock<GoogleLogOut>();
        mock.Setup(log => log.Settings).Returns(this.mockSettings.Object);
        mock.Setup(log => log.GoogleLogin).Returns(this.mockGoogleLogin.Object);
        mock.Setup(log => log.CoroutineRunner).Returns(this.mockCoroutineRunner.Object);

        this.mockSettings.Setup(settings => settings.IsAnyAuthTokenCached()).Returns(false);
        this.mockGoogleLogin.Setup(login => login.UpdateInfo(It.IsAny<GoogleDriveAbout.GetRequest>()));
        this.mockGoogleLogin.Setup(login => login.FindId(It.IsAny<GoogleDriveFiles.ListRequest>(), false));

        yield return mock.Object.LogIn();

        this.mockGoogleLogin.Verify(login => login.FindId(It.IsAny<GoogleDriveFiles.ListRequest>(), false));
        this.mockGoogleLogin.Verify(login => login.UpdateInfo(It.IsAny<GoogleDriveAbout.GetRequest>()));
    }
}
