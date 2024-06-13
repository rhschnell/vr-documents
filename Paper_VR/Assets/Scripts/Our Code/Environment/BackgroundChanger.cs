using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class handles the changes for the background of the environment.
/// </summary>
public class BackgroundChanger : MonoBehaviour
{
    /// <summary>
    /// This is the list containing all backgrounds.
    /// </summary>
    public List<Material> Backgrounds;

    /// <summary>
    /// This is the dropdown.
    /// </summary>
    public TMP_Dropdown Dropdown;

    /// <summary>
    /// This event handler changes the skybox in the environment.
    /// </summary>
    /// <param name="change">The new dropdown value.</param>
    public void DropdownValueChanged(TMP_Dropdown change)
    {
        // Update the skybox to the selected background based on the dropdown value
        RenderSettings.skybox = this.Backgrounds[change.value];
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        List<string> backgroundNames = new List<string>();

        // Get the names of the materials in the backgrounds list
        foreach (Material material in this.Backgrounds)
        {
            backgroundNames.Add(material.name);
        }

        // Add the background options to the dropdown
        this.Dropdown.ClearOptions();
        this.Dropdown.AddOptions(backgroundNames);
    }
}
