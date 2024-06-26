using System.Collections.Generic;
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

        // Set up the component with the mocks
        this.addEnvironment.InputField = this.mockInputField;
        this.addEnvironment.error = this.mockError;
        this.addEnvironment.addButton = this.addButton;
        this.addEnvironment.selectEnvironment = this.selectMock.Object;
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
        AddEnvironment addEnvironment = this.gameObject.AddComponent<AddEnvironment>();

        // Mock googleMethods
        var googleMethods = new Mock<GoogleMethods>();

        Mock<SelectEnvironment> selectEnvironmentMock = new Mock<SelectEnvironment>();
        Mock<TMP_Dropdown> dropdownMock = new Mock<TMP_Dropdown>();
        var optionsDropdown = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Environment 1"),
            new TMP_Dropdown.OptionData("Environment 2"),
        };
        dropdownMock.Object.options = optionsDropdown;
        selectEnvironmentMock.Object.dropdown = dropdownMock.Object;

        // Setup mock behavior
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.File>>();
        responseMock.Setup(x => x.GoogleDriveRequest).Returns(this.mockCreateRequest.Object);
        googleMethods.Setup(x => x.MakeRequest("TestName")).Returns(this.mockCreateRequest.Object);
        this.mockCreateRequest.Setup(req => req.Send()).Returns(responseMock.Object);

        addEnvironment.error = this.mockError;
        addEnvironment.InputField = this.mockInputField;
        addEnvironment.googleMethods = googleMethods.Object;
        addEnvironment.addButton = this.addButton;
        addEnvironment.selectEnvironment = selectEnvironmentMock.Object;

        addEnvironment.AddEnvironmentButton();

        Assert.AreEqual("", this.mockError.text);
    }

    /// <summary>
    /// Tests the AddEnvironmentButton method when the name is already in the dropdown.
    /// </summary>
    [Test]
    public void AddEnvironmentButton_WhenNameIsNotEmpty_DuplicateName()
    {
        // Set the input field text
        this.mockInputField.text = "Env1";
        Mock<AddEnvironment> addEnvironmentMock = new Mock<AddEnvironment>();
        Mock<SelectEnvironment> selectEnvironmentMock = new Mock<SelectEnvironment>();
        Mock<TMP_Dropdown> dropdownMock = new Mock<TMP_Dropdown>();
        var optionsDropdown = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Env1"),
            new TMP_Dropdown.OptionData("Env2"),
        };
        dropdownMock.Object.options = optionsDropdown;
        selectEnvironmentMock.Object.dropdown = dropdownMock.Object;

        addEnvironmentMock.Object.error = this.mockError;
        addEnvironmentMock.Object.InputField = this.mockInputField;
        addEnvironmentMock.Object.selectEnvironment = selectEnvironmentMock.Object;
        addEnvironmentMock.Object.AddEnvironmentButton();

        Assert.AreEqual("An environment with this name already exists", this.mockError.text);
    }
}
