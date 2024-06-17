using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

/// <summary>
/// Tests for the BackgroundController
/// </summary>
public class BackgroundChangerTest : MonoBehaviour
{
    private GameObject gameManager;
    private EnvironmentController environmentController;

    /// <summary>
    /// Set up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.gameManager = new GameObject("GameManager");
        this.environmentController = this.gameManager.AddComponent<EnvironmentController>();
    }

    /// <summary>
    /// Clean up the test environment.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        Destroy(this.gameManager);
        Destroy(this.environmentController);
    }

    /// <summary>
    /// Get and set test for the name field.
    /// </summary>
    [Test]
    public void DropdownValueChangedTest()
    {
        // Setup the background changer
        var backgroundChanger = new GameObject("BackgroundController").AddComponent<BackgroundController>();
        List<Material> backgrounds = new List<Material>
        {
            new (Shader.Find("Standard")),
            new (Shader.Find("Unlit/Texture")),
        };

        backgroundChanger.Backgrounds = backgrounds;
        backgroundChanger.Dropdown = new GameObject("Dropdown").AddComponent<TMP_Dropdown>();
        backgroundChanger.Dropdown.value = 1;
        var dropdown = new GameObject("Dropdown").AddComponent<TMP_Dropdown>();
        dropdown.value = 0;
        backgroundChanger.DropdownValueChanged(dropdown);

        // Assert background changed successfully
        Assert.AreEqual(backgrounds[0], RenderSettings.skybox);
    }
}