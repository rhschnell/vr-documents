using System.Collections;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Contains unit tests for the <see cref="SaveButton"/> class.
/// </summary>
public class SaveButtonTests
{
    private GameObject gameManager;
    private GameObject saveButtonObject;
    private SaveButton saveButton;
    private Mock<EnvironmentInformation> environmentMock;

    /// <summary>
    /// Sets up the test environment by creating necessary GameObjects and components.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Create a GameManager object and attach a mock EnvironmentInformation component
        this.gameManager = new GameObject("GameManager");
        this.environmentMock = new Mock<EnvironmentInformation>();
        this.gameManager.AddComponent<EnvironmentInformation>();

        // Create a SaveButton object and attach SaveButton component
        this.saveButtonObject = new GameObject("SaveButton");
        this.saveButton = this.saveButtonObject.AddComponent<SaveButton>();

        // Optionally set the savingTime if you want to test the interval-based saving
        this.saveButton.savingTime = 1f; // Set a low value for quicker test
    }

    /// <summary>
    /// Cleans up the test environment by destroying the created GameObjects.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        // Clean up objects after tests
        Object.Destroy(this.gameManager);
        Object.Destroy(this.saveButtonObject);
    }
}
