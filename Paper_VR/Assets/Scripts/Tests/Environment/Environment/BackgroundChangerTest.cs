using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;

/// <summary>
/// Tests for the BackgroundController
/// </summary>
public class BackgroundChangerTest
{
    /// <summary>
    /// Get and set test for the name field.
    /// </summary>
    [Test]
    public void DropdownValueChangedTest()
    {
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
        Assert.AreEqual(backgrounds[0], RenderSettings.skybox);
    }
}