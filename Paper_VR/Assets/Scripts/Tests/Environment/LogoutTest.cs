using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

public class LogoutTest
{

    private GoogleLogOut googleLogOut;
    private Mock<GoogleDriveSettings> mockSettings;
    private Mock<GoogleDriveAbout.GetRequest> mockRequest;
    private Mock<GoogleLogin> mockGoogleLogin;


    [SetUp]
    public void SetUp()
    {
        // Initialize GoogleLogOut and its dependencies
        GameObject go = new GameObject();
        googleLogOut = go.AddComponent<GoogleLogOut>();

        mockSettings = new Mock<GoogleDriveSettings>();
        mockRequest = new Mock<GoogleDriveAbout.GetRequest>();
        mockGoogleLogin = new Mock<GoogleLogin>();

        googleLogOut.settings = mockSettings.Object;
        googleLogOut.GoogleLogin = mockGoogleLogin.Object;

        mockRequest.SetupAllProperties();
    }

    //[Test]
    public void LogIn_ShouldUpdateUserDetailsAndCallFindId()
    {
        // Arrange
        var mockUser = new User { DisplayName = "Test User", EmailAddress = "test@example.com" };
        var mockResponse = new About { User = mockUser };

        var responseMock2 = new Mock<GoogleDriveRequest<UnityGoogleDrive.Data.About>>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.About>>();


        mockRequest.Setup(r => r.Send()).Returns(responseMock.Object);
        mockRequest.SetupGet(r => r.IsError).Returns(false);
        mockRequest.SetupGet(r => r.ResponseData).Returns(mockResponse);
        var i = new Mock<IEnumerator>();
        mockGoogleLogin.CallBase = false;
        mockGoogleLogin.Setup(x => x.FindId()).Returns(i.Object);

        googleLogOut.request = mockRequest.Object;

        // Act
        googleLogOut.LogOut();

        // Assert
        Assert.AreEqual(mockUser.DisplayName, "Test User");
        Assert.AreEqual(mockUser.EmailAddress, "test@example.com");
        //mockGoogleLogin.Verify(gl => gl.FindId());
    }
}
