using System.Collections;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityGoogleDrive;

/// <summary>
/// Contains unit tests for the <see cref="SaveController"/> class.
/// </summary>
public class SaveButtonTests
{
    private GameObject gameManager;
    private GameObject saveButtonObject;
    private SaveController saveButton;
    private Mock<GoogleMethods> googleMock;

    /// <summary>
    /// Sets up the test environment by creating necessary GameObjects and components.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Create a GameManager object and attach a mock EnvironmentController component
        this.gameManager = new GameObject("GameManager");
        this.googleMock = new Mock<GoogleMethods>();

        // this.gameManager.AddComponent<EnvironmentInformation>();

        // Create real GameObjects for TMP_Text components
        GameObject saveMenuNameObject = new GameObject("SaveMenuName");
        TextMeshProUGUI saveMenuName = saveMenuNameObject.AddComponent<TextMeshProUGUI>();

        GameObject saveMenuConfirmationObject = new GameObject("SaveMenuConfirmation");
        TextMeshProUGUI saveMenuConfirmation = saveMenuConfirmationObject.AddComponent<TextMeshProUGUI>();

        // Create SaveButton GameObject and attach SaveButton component and Button component
        this.saveButtonObject = new GameObject("SaveButton");
        this.saveButton = this.saveButtonObject.AddComponent<SaveController>();
        this.saveButton.saveButton = this.saveButtonObject.AddComponent<Button>();

        // Assign real TextMeshProUGUI components to SaveButton
        this.saveButton.saveMenuName = saveMenuName;
        this.saveButton.saveMenuConfirmation = saveMenuConfirmation;

        // Assign the mock GoogleMethods
        this.saveButton.googleMethods = this.googleMock.Object;

        // Set the savingTime to 1 second instead of 45 to make testing faster
        this.saveButton.savingTime = 1f;
    }

    /// <summary>
    /// Clean up the test environment.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        // Clean up GameObjects
        GameObject.Destroy(this.gameManager);
        GameObject.Destroy(this.saveButtonObject);
    }

    /// <summary>
    /// Tests if the Save method calls the FindEnviormentId method.
    /// </summary>
    [Test]
    public void TestSave()
    {
        // Call the Save() method to act like the autosave method has saved the environment
        this.saveButton.Save();

        // Verify that the Save() method has saved the environment
        this.googleMock.Verify(x => x.FindEnviormentId("New Environment", It.IsAny<GoogleDriveFiles.ListRequest>()));
    }

    /// <summary>
    /// Tests if the environment is saved after quitting the application.
    /// </summary>
    [Test]
    public void TestOnApplicationQuick()
    {
        // Call the OnApplicationQuit() method to act like we have quitted the application
        this.saveButton.OnApplicationQuit();

        // Verify that the Save() method has saved the environment
        this.googleMock.Verify(x => x.FindEnviormentId("New Environment", It.IsAny<GoogleDriveFiles.ListRequest>()));
    }

    /// <summary>
    /// Tests if the manual saves makes the save button not interactable.
    /// </summary>
    [Test]
    public void TestOnSaveClick()
    {
        this.googleMock.Setup(x => x.FindEnviormentId(It.IsAny<string>(), It.IsAny<GoogleDriveFiles.ListRequest>()))
                  .Returns(this.FakeEnumerator());

        // Call OnSaveClick() method to act like we have manually saved the environment
        this.saveButton.OnSaveClick();

        // Verify that the Save() method has saved the environment
        this.googleMock.Verify(x => x.FindEnviormentId("New Environment", It.IsAny<GoogleDriveFiles.ListRequest>()));
        Assert.IsFalse(this.saveButton.saveButton.interactable);
        Assert.IsFalse(this.saveButton.saveMenuName.IsActive());
        Assert.IsTrue(this.saveButton.saveMenuConfirmation.IsActive());
    }

    /// <summary>
    /// Returns an IEnumerator used for mocking.
    /// </summary>
    /// <returns>An IEnumerator.</returns>
    private IEnumerator FakeEnumerator()
    {
        yield return null;
    }
}
