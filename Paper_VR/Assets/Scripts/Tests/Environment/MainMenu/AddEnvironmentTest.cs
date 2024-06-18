using Moq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGoogleDrive;

/// <summary>
/// Tests the AddEnvironment class
/// </summary>
public class AddEnvironmentTest
{
    private GameObject gameObject;
    private AddEnvironment addEnvironment;
    private Mock<GoogleDriveFiles.CreateRequest> mockCreateRequest;
    private TMP_InputField mockInputField;
    private TextMeshPro mockError;
    private Button addButton;
    private Mock<SelectEnvironment> selectMock;
    private Mock<ICoroutineRunner> mockCoroutineRunner;

    /// <summary>
    /// Set up the test environment.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Create a new GameObject and add the AddEnvironment component to it
        this.gameObject = new GameObject();
        this.addEnvironment = this.gameObject.AddComponent<AddEnvironment>();

        // Create mocks for dependencies
        this.mockCreateRequest = new Mock<GoogleDriveFiles.CreateRequest>();
        this.mockInputField = new GameObject().AddComponent<TMP_InputField>();
        this.mockError = new GameObject().AddComponent<TextMeshPro>();
        this.addButton = new GameObject().AddComponent<Button>();
        this.selectMock = new Mock<SelectEnvironment>();
        this.mockCoroutineRunner = new Mock<ICoroutineRunner>();

        // Set up the component with the mocks
        this.addEnvironment.InputField = this.mockInputField;
        this.addEnvironment.error = this.mockError;
        this.addEnvironment.addButton = this.addButton;
        this.addEnvironment.selectEnvironment = this.selectMock.Object;
        this.addEnvironment.coroutineRunner = this.mockCoroutineRunner.Object;
    }

    /// <summary>
    /// Clean up the test environment.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(this.gameObject);
        GameObject.DestroyImmediate(this.mockInputField.gameObject);
        GameObject.DestroyImmediate(this.mockError.gameObject);
        GameObject.DestroyImmediate(this.addButton.gameObject);
    }

    /// <summary>
    /// Tests the MakeRequest method.
    /// </summary>
    [Test]
    public void MakeRequest_ReturnsCreateRequest()
    {
        var name = "TestName";

        GoogleDriveFiles.CreateRequest result = this.addEnvironment.MakeRequest(name);

        // Assert request is created
        Assert.IsNotNull(result);
        Assert.AreEqual(name, result.RequestData.Name);
        Assert.AreEqual("application/vnd.google-apps.folder", result.RequestData.MimeType);
    }

    /// <summary>
    /// Tests the AddEnvironmentButton method when the name is empty.
    /// </summary>
    [Test]
    public void AddEnvironmentButton_WhenNameIsEmpty_SetsErrorMessage()
    {
        // Set empty name
        this.addEnvironment.InputField.text = "";

        this.addEnvironment.AddEnvironmentButton();

        Assert.AreEqual("Name must not be empty!", this.addEnvironment.error.text);
    }

    /// <summary>
    /// Tests the AddEnvironmentButton method when the name is not empty.
    /// </summary>
    [Test]
    public void AddEnvironmentButton_WhenNameIsNotEmpty_CreatesNewFolder()
    {
        // Set the input field text
        this.mockInputField.text = "TestName";
        Mock<AddEnvironment> addEnvironmentMock = new Mock<AddEnvironment>();

        // Setup mock behavior
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.File>>();
        responseMock.Setup(x => x.GoogleDriveRequest).Returns(this.mockCreateRequest.Object);
        addEnvironmentMock.Setup(x => x.MakeRequest("TestName")).Returns(this.mockCreateRequest.Object);
        this.mockCreateRequest.Setup(req => req.Send()).Returns(responseMock.Object);

        addEnvironmentMock.Object.error = this.mockError;
        addEnvironmentMock.Object.InputField = this.mockInputField;
        addEnvironmentMock.Object.coroutineRunner = this.mockCoroutineRunner.Object;

        addEnvironmentMock.Object.AddEnvironmentButton();

        Assert.AreEqual("", this.mockError.text);
    }
}
