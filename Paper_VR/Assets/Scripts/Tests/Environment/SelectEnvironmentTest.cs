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
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("env1"),
            new TMP_Dropdown.OptionData("env2"),
        };
        this.mockDropdown.Object.options = options;
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

        this.selectEnvironment.SelectEnvironmentButton();

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

    /// <summary>
    /// This test the get environment method.
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator GetEnvironmentTest()
    {
        // Create a Select Environment mock
        var mock = new Mock<SelectEnvironment>();
        mock.Setup(m => m.RequestList).Returns(this.mockListRequest.Object);
        mock.Object.gameManager = new GameObject("GameManager");

        // Create a json file of a EnvironmentInfo object
        EnvironmentInfo environmentInfo = new EnvironmentInfo();
        environmentInfo.environmentName = "Env1";
        environmentInfo.floatingDocuments = new List<FloatingDocumentInfo>();
        environmentInfo.importList = new List<string>();
        environmentInfo.backgroundColor = Color.red;
        string json = environmentInfo.SaveToJson();
        var content = System.Text.Encoding.ASCII.GetBytes(json);

        // Create a mock response
        var mockFiles = new List<UnityGoogleDrive.Data.File>
        {
            new UnityGoogleDrive.Data.File { Name = "Env1", Content = content },
            new UnityGoogleDrive.Data.File { Name = "Env2", Content = content },
        };
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(this.mockListRequest.Object);

        this.mockListRequest.Setup(req => req.Send()).Returns(responseMock.Object);
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(mockFiles);
        this.mockListRequest.Setup(req => req.IsError).Returns(false);

        yield return mock.Object.GetEnvironment("123", "Env1");

        // Verify that the request was sent
        this.mockListRequest.Verify(req => req.Send());

        // Assert that the environment was loaded
        EnvironmentInformation env = mock.Object.gameManager.GetComponent<EnvironmentInformation>();
        Assert.AreEqual("Env1", env.GetName());
        Assert.AreEqual(Color.red, env.GetBackgroundColor());
    }

    /*
    /// <summary>
    /// This test the get environment method.
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator GetEnvironmentTestNoFiles()
    {
        // Create a Select Environment mock
        var mock = new Mock<SelectEnvironment>();
        mock.Setup(m => m.RequestList).Returns(this.mockListRequest.Object);
        mock.Object.gameManager = new GameObject("GameManager");

        // Create a mock response
        var mockFiles = new List<UnityGoogleDrive.Data.File>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(this.mockListRequest.Object);

        this.mockListRequest.Setup(req => req.Send()).Returns(responseMock.Object);
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(mockFiles);
        this.mockListRequest.Setup(req => req.IsError).Returns(false);

        yield return mock.Object.GetEnvironment("123", "Env1");

        // Verify that the request was sent
        this.mockListRequest.Verify(req => req.Send());

        // Assert that the environment was loaded
        EnvironmentInformation env = mock.Object.gameManager.GetComponent<EnvironmentInformation>();
        Assert.AreEqual("Env1", env.GetName());
        Assert.AreEqual(Color.white, env.GetBackgroundColor());
    }
    */

    /// <summary>
    /// Tests the make create request method.
    /// </summary>
    [Test]
    public void MakeRequestTest()
    {
        var r = this.selectEnvironment.MakeRequest("123");

        Assert.AreEqual("Saved Documents", r.RequestData.Name);
        Assert.AreEqual("application/vnd.google-apps.folder", r.RequestData.MimeType);
        Assert.AreEqual("123", r.RequestData.Parents[0]);
    }

    /// <summary>
    /// Tests the create saved doc folder method.
    /// </summary>
    /// <returns>its a unity test</returns>
    [UnityTest]
    public IEnumerator CreateSavedDocFolderTest()
    {
        var mock = new Mock<SelectEnvironment>();
        var requestMock = new Mock<GoogleDriveFiles.CreateRequest>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.File>>();

        responseMock.Setup(a => a.GoogleDriveRequest).Returns(requestMock.Object);
        requestMock.Setup(req => req.Send()).Returns(responseMock.Object);

        mock.Setup(m => m.MakeRequest("123")).Returns(requestMock.Object);
        mock.CallBase = true;

        yield return mock.Object.CreateSavedDocFolder("123");

        requestMock.Verify(req => req.Send());
    }

    /// <summary>
    /// Tests the HasSavedDocFolder method.
    /// </summary>
    /// <returns>Its a unity test</returns>
    [UnityTest]
    public IEnumerator HasSavedDocFolderTest()
    {
        var mock = new Mock<SelectEnvironment>();
        var requestMock = new Mock<GoogleDriveFiles.ListRequest>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();

        responseMock.Setup(a => a.GoogleDriveRequest).Returns(requestMock.Object);
        requestMock.Setup(req => req.Send()).Returns(responseMock.Object);
        requestMock.Setup(req => req.ResponseData.Files).Returns(new List<UnityGoogleDrive.Data.File> { new UnityGoogleDrive.Data.File() });

        mock.CallBase = true;

        yield return mock.Object.HasSavedDocFolder("123", requestMock.Object);

        requestMock.Verify(req => req.Send());
    }

    /// <summary>
    /// Tests the HasSavedDocFolder method with no files.
    /// </summary>
    /// <returns>Its a unity test</returns>
    [UnityTest]
    public IEnumerator HasSavedDocFolderTestNoFiles()
    {
        var mock = new Mock<SelectEnvironment>();
        var requestMock = new Mock<GoogleDriveFiles.ListRequest>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        var coroutineRunnerMock = new Mock<ICoroutineRunner>();
        mock.Object.CoroutineRunner = coroutineRunnerMock.Object;
        mock.Setup(m => m.CoroutineRunner).Returns(coroutineRunnerMock.Object);

        responseMock.Setup(a => a.GoogleDriveRequest).Returns(requestMock.Object);
        requestMock.Setup(req => req.Send()).Returns(responseMock.Object);
        requestMock.Setup(req => req.ResponseData.Files).Returns(new List<UnityGoogleDrive.Data.File>());

        mock.CallBase = true;

        yield return mock.Object.HasSavedDocFolder("123", requestMock.Object);

        requestMock.Verify(req => req.Send());
    }

    /// <summary>
    /// Test for the ChangeDeleteText method, checks if the text is changed as expected
    /// </summary>
    [Test]
    public void ChangeDeleteTextTest()
    {
        Mock<TMPro.TextMeshPro> mockText = new Mock<TMPro.TextMeshPro>();
        this.selectEnvironment.DeleteText = mockText.Object;
        mockText.Object.text = "nothing";
        mockText.CallBase = true;

        var mockFiles = new List<UnityGoogleDrive.Data.File> { new UnityGoogleDrive.Data.File { Name = "Env1", Id = "123" } };
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(mockFiles);

        this.selectEnvironment.RequestList = this.mockListRequest.Object;
        this.mockDropdown.Object.value = 0;
        this.mockDropdown.Object.options = new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData("Env1") };

        this.selectEnvironment.ChangeDeleteText();
        Assert.AreEqual("Are you sure you want to delete Env1?", mockText.Object.text);
    }

    /// <summary>
    /// Test for when the button is pressed and correct methods are called after
    /// </summary>
    [Test]
    public void DeleteEnvironmentButtonTest()
    {
        var mock = new Mock<SelectEnvironment>();
        var mockFiles = new List<UnityGoogleDrive.Data.File> { new UnityGoogleDrive.Data.File { Name = "Env1", Id = "123" } };
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(mockFiles);

        mock.Setup(m => m.RequestList).Returns(this.mockListRequest.Object);
        mock.Object.dropdown = this.mockDropdown.Object;
        mock.Setup(m => m.CoroutineRunner).Returns(this.mockCoroutineRunner.Object);
        this.mockDropdown.Object.value = 0;

        mock.Object.DeleteEnvironmentButton();
        this.mockCoroutineRunner.Verify(runner => runner.StartCoroutine(It.IsAny<IEnumerator>()));
    }

    /// <summary>
    /// Testing the delete environment method, checks if the request is send and if the next method is called.
    /// </summary>
    /// <returns>its a unity test</returns>
    [UnityTest]
    public IEnumerator DeleteEnvironmentTest()
    {
        var mock = new Mock<SelectEnvironment>();
        var mockFiles = new List<UnityGoogleDrive.Data.File> { new UnityGoogleDrive.Data.File { Name = "Env1", Id = "123" } };
        this.mockListRequest.Setup(req => req.ResponseData.Files).Returns(mockFiles);

        mock.Setup(m => m.RequestList).Returns(this.mockListRequest.Object);
        mock.Setup(m => m.CoroutineRunner).Returns(this.mockCoroutineRunner.Object);

        var mockRequest = new Mock<GoogleDriveFiles.DeleteRequest>("123");

        yield return mock.Object.DeleteEnvironmenet(mockRequest.Object);

        this.mockCoroutineRunner.Verify(runner => runner.StartCoroutine(It.IsAny<IEnumerator>()));
    }
}