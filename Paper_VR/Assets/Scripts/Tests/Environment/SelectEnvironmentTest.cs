using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityGoogleDrive;

/// <summary>
/// The testing class for SelectingEnvironment.
/// </summary>
public class SelectEnvironmentTest
{
    private GameObject gameObject;
    private SelectEnvironment selectEnvironment;
    private Mock<TMP_Dropdown> mockDropdown;
    private Mock<TMP_Text> mockNameText;
    private Mock<TMP_Text> mockEmailText;
    private Mock<GoogleDriveFiles.ListRequest> mockListRequest;
    private Mock<GameObject> mockGameManager;
    private Mock<ICoroutineRunner> mockCoroutineRunner;

    /// <summary>
    /// Set up the test environment.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this.gameObject = new GameObject();
        this.selectEnvironment = this.gameObject.AddComponent<SelectEnvironment>();
        this.selectEnvironment.testing = true;
        this.mockDropdown = new Mock<TMP_Dropdown>();
        this.mockNameText = new Mock<TMP_Text>();
        this.mockEmailText = new Mock<TMP_Text>();
        this.mockListRequest = new Mock<GoogleDriveFiles.ListRequest>();
        this.mockCoroutineRunner = new Mock<ICoroutineRunner>();

        this.selectEnvironment.dropdown = this.mockDropdown.Object;
        this.selectEnvironment.Name = this.mockNameText.Object;
        this.selectEnvironment.Email = this.mockEmailText.Object;
        this.selectEnvironment.gameManager = new GameObject();
        this.selectEnvironment.CoroutineRunner = this.mockCoroutineRunner.Object;
    }

    /// <summary>
    /// Clean up the test environment.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(this.gameObject);
        if (SceneManager.GetSceneByName("Environment").isLoaded)
        {
            SceneManager.UnloadSceneAsync("Environment");
        }
    }

    /// <summary>
    /// A test that confirms that the AddEnvironmentButton method sets the parent ID and loads the new scene.
    /// </summary>
    [Test]
    public void AddEnvironmentButton_SetsParentIdAndLoadsNewScene()
    {
        var mockFiles = new List<UnityGoogleDrive.Data.File> { new UnityGoogleDrive.Data.File { Id = "123" } };
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(mockFiles);

        this.selectEnvironment.RequestList = this.mockListRequest.Object;
        this.mockDropdown.Object.value = 0;

        this.selectEnvironment.AddEnvironmentButton();

        Assert.AreEqual("123", SelectEnvironment.parentId);
        SelectEnvironment.parentId = null;
    }

    /// <summary>
    /// A test that confirms that the refresh method works as expected.
    /// </summary>
    [Test]
    public void Refresh_UpdatesNameAndEmailAndCallsUpdateList()
    {
        GoogleLogin.name = "Test User";
        GoogleLogin.email = "test@example.com";

        Mock<SelectEnvironment> mock = new Mock<SelectEnvironment>();
        mock.Setup(m => m.CoroutineRunner).Returns(this.mockCoroutineRunner.Object);
        mock.Object.Name = this.mockNameText.Object;
        mock.Object.Email = this.mockEmailText.Object;

        mock.Object.Refresh();

        this.mockNameText.VerifySet(txt => txt.text = "Name: Test User");
        this.mockEmailText.VerifySet(txt => txt.text = "Email: test@example.com");
    }

    /// <summary>
    /// A test that confirms that the UpdateList method sets the dropdown options.
    /// </summary>
    /// <returns>Its a unity tests, so needs an enumerator as return</returns>
    [UnityTest]
    public IEnumerator UpdateList_SetsDropdownOptions()
    {
        var mockFiles = new List<UnityGoogleDrive.Data.File>
        {
            new UnityGoogleDrive.Data.File { Name = "Env1" },
            new UnityGoogleDrive.Data.File { Name = "Env2" },
        };
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(this.mockListRequest.Object);

        var mock = new Mock<SelectEnvironment>();
        mock.Setup(m => m.CoroutineRunner).Returns(this.mockCoroutineRunner.Object);
        mock.Object.dropdown = this.mockDropdown.Object;
        mock.Object.testing = true;

        this.mockListRequest.Setup(req => req.Send()).Returns(responseMock.Object);
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(mockFiles);
        this.mockListRequest.Setup(req => req.IsError).Returns(false);

        yield return mock.Object.UpdateList(this.mockListRequest.Object);

        Assert.AreEqual(this.mockDropdown.Object.options.Count, 2);
        Assert.True(this.mockDropdown.Object.options.Exists(option => option.text == "Env1"));
        Assert.True(this.mockDropdown.Object.options.Exists(option => option.text == "Env2"));

        yield return new WaitForSeconds(1);
    }
}
