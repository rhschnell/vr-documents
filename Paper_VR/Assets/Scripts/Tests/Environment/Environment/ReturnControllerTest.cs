using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

/// <summary>
/// This class tests the ReturnConroller class.
/// </summary>
public class ReturnControllerTest
{
    private GameObject returnButton;
    private ReturnController controller;
    private GameObject gameManager;

    /// <summary>
    /// Sets up the test environment.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Create a return button and attach the ReturnController script
        this.returnButton = new GameObject();
        this.controller = this.returnButton.AddComponent<ReturnController>();
        this.controller.testing = true;

        this.gameManager = new GameObject("GameManager");
    }

    /// <summary>
    /// Cleans up the test environment.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        // Clean up objects
        GameObject.Destroy(this.controller);
        GameObject.Destroy(this.gameManager);
    }

    /// <summary>
    /// Tests if the GameManager is destroyed after clicking return.
    /// </summary>
    [Test]
    public void TestReturnButtonClickDestroysGameManager()
    {
        GameObject gameManagerOriginal = GameObject.Find("GameManager");

        // Simulate clicking the return button
        this.controller.OnClickReturn();

        // Assert that the GameManager object is destroyed
        GameObject remainingGameManager = GameObject.Find("GameManager");
        Assert.AreNotEqual(remainingGameManager, gameManagerOriginal);
    }
}