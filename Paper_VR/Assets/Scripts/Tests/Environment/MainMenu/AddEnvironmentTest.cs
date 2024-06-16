using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGoogleDrive;

/// <summary>
/// Tests the AddEnvironment class
/// </summary>
public class AddEnvironmentTest
{
    /// <summary>
    /// Tests the MakeRequest method
    /// </summary>
    [Test]
    public void MakeRequest_ReturnsCreateRequest()
    {
        // Arrange
        GameObject go = new GameObject();
        AddEnvironment addEnvironment = go.AddComponent<AddEnvironment>();
        var name = "TestName";

        // Act
        GoogleDriveFiles.CreateRequest result = addEnvironment.MakeRequest(name);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(name, result.RequestData.Name);
        Assert.AreEqual("application/vnd.google-apps.folder", result.RequestData.MimeType);
        GameObject.DestroyImmediate(go);
    }

    /// <summary>
    /// Tests the AddEnvironmentButton method when the name is empty
    /// </summary>
    [Test]
    public void AddEnvironmentButton_WhenNameIsEmpty_SetsErrorMessage()
    {
        // Arrange
        GameObject go = new GameObject();
        AddEnvironment addEnvironment = go.AddComponent<AddEnvironment>();
        addEnvironment.InputField = new GameObject().AddComponent<TMPro.TMP_InputField>();
        addEnvironment.error = new TMPro.TextMeshPro();
        addEnvironment.InputField.text = "";

        // Act
        addEnvironment.AddEnvironmentButton();

        // Assert
        Assert.AreEqual("Name must not be empty!", addEnvironment.error.text);
        GameObject.DestroyImmediate(go);
    }

    /// <summary>
    /// Tests the AddEnvironmentButton method when the name is not empty
    /// </summary>
    [Test]
    public void AddEnvironmentButton_WhenNameIsNotEmpty_CreatesNewFolder()
    {
        Mock<AddEnvironment> mock = new Mock<AddEnvironment>();
        Mock<GoogleDriveFiles.CreateRequest> mockCreateRequest = new Mock<GoogleDriveFiles.CreateRequest>();
        Mock<TMPro.TextMeshPro> mockError = new Mock<TMPro.TextMeshPro>();
        Mock<TMPro.TMP_InputField> mockInputField = new Mock<TMPro.TMP_InputField>();

        mock.Object.InputField = mockInputField.Object;
        mock.Object.error = mockError.Object;
        mockInputField.Object.text = "TestName";
        mockError.CallBase = true;

        mock.Setup(m => m.MakeRequest("TestName")).Returns(mockCreateRequest.Object);
        mock.CallBase = true;

        mock.Object.AddEnvironmentButton();

        Assert.AreEqual("", mockError.Object.text);
        mockCreateRequest.Verify(req => req.Send());
    }
}
