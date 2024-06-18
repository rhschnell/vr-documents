using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGoogleDrive;

/// <summary>
/// This class adds environments to documents.
/// </summary>
public class AddEnvironment : MonoBehaviour
{
    /// <summary>
    /// The input field where the environment name should be inserted.
    /// </summary>
    public TMP_InputField InputField;

    /// <summary>
    /// The button which can be clicked to add an environment.
    /// </summary>
    public Button addButton;

    /// <summary>
    /// the error message text field.
    /// </summary>
    public TMP_Text error;

    /// <summary>
    /// An instance of the SelectEnvironment class to update the environment list.
    /// </summary>
    public SelectEnvironment selectEnvironment;

    /// <summary>
    /// The GoogleMethods class used for creating the environment folder.
    /// </summary>
    public GoogleMethods googleMethods = new GoogleMethods();

    /// <summary>
    /// Adds an environment.
    /// </summary>
    public void AddEnvironmentButton()
    {
        string environmentName = this.InputField.text;
        if (environmentName == "")
        {
            // Give an error in the environment when the given name is an empty string
            this.error.text = "Name must not be empty!";
            return;
        }

        this.error.text = "";
        bool nameExists = false;
        foreach (var option in this.selectEnvironment.dropdown.options)
        {
            if (option.text == environmentName)
            {
                nameExists = true;
                break;
            }
        }

        if (nameExists)
        {
            this.error.text = "An environment with this name already exists";
        }
        else
        {
            // Create a new environment folder in the PaperVR folder
            GoogleDriveFiles.CreateRequest createRequest = this.googleMethods.MakeRequest(environmentName);
            createRequest.Send();

            // Disable the addButton for one second and reset the input field
            this.InputField.text = "";
            this.StartCoroutine(this.DisableAddButton());
        }
    }

    /// <summary>
    /// Makes the open add environment button not interactable for one second.
    /// </summary>
    /// <returns>An IEnumerator.</returns>
    private IEnumerator DisableAddButton()
    {
        // Make the add environment button not interactable
        this.addButton.interactable = false;

        // Wait for one second
        yield return new WaitForSeconds(1.0f);

        // Automatically refresh the environment dropdown after adding one
        this.selectEnvironment.Refresh();

        // Make the add environment button interactable again
        this.addButton.interactable = true;
    }
}
