using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class handles the changes for the background of the environment.
/// </summary>
public class BackgroundController : MonoBehaviour
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
        this.SetBackground(change.value);
    }

    /// <summary>
    /// Sets the background of the environment.
    /// </summary>
    /// <param name="index">The index of the new material.</param>
    public void SetBackground(int index)
    {
        Material material = this.Backgrounds[index];

        // Update the skybox to the selected background based on the dropdown value
        RenderSettings.skybox = material;

        GameObject gameManager = GameObject.Find("GameManager");

        // Adds the new duplicated pdf to the environment information
        EnvironmentController environment = gameManager.GetComponent<EnvironmentController>();
        environment.SetMaterial(material);

        // Set the current background on the dropdown
        this.Dropdown.value = index;
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        // Get the background material
        GameObject gameManager = GameObject.Find("GameManager");
        EnvironmentController environment = gameManager.GetComponent<EnvironmentController>();

        Material materialToSet = environment.GetMaterial();

        List<string> backgroundNames = new ();

        int newMaterialIndex = 0;
        for (int i = 0; i < this.Backgrounds.Count; i++)
        {
            backgroundNames.Add(this.Backgrounds[i].name);
            if (this.Backgrounds[i].name == materialToSet.name)
            {
                newMaterialIndex = i;
            }
        }

        // Add the background options to the dropdown
        this.Dropdown.ClearOptions();
        this.Dropdown.AddOptions(backgroundNames);

        this.SetBackground(newMaterialIndex);
    }
}
