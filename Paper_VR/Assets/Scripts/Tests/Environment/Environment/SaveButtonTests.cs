using System.Collections;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Contains unit tests for the <see cref="SaveController"/> class.
/// </summary>
public class SaveButtonTests
{
    private GameObject gameManager;
    private GameObject saveButtonObject;
    private SaveController saveButton;
    private Mock<EnvironmentController> environmentMock;

    /// <summary>
    /// Sets up the test environment by creating necessary GameObjects and components.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Create a GameManager object and attach a mock EnvironmentController component
        this.gameManager = new GameObject("GameManager");
        this.environmentMock = new Mock<EnvironmentController>();
        this.gameManager.AddComponent<EnvironmentController>();

        // Create a SaveController object and attach SaveController component
        this.saveButtonObject = new GameObject("SaveController");
        this.saveButton = this.saveButtonObject.AddComponent<SaveController>();

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
