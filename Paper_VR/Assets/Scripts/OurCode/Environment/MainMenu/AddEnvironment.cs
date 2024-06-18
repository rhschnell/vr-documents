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
    /// The manager of the game, containing all inportant information.
    /// </summary>
    public GameObject gameManager;

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
    /// CoroutineRunner used for testing.
    /// </summary>
    public ICoroutineRunner coroutineRunner;

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
        }
        else
        {
            this.error.text = "";
            // Create a new environment folder in the PaperVR folder
            GoogleDriveFiles.CreateRequest createRequest = this.MakeRequest(environmentName);
            createRequest.Send();

            // Disable the addButton for one second and reset the input field
            this.InputField.text = "";
            this.coroutineRunner.StartCoroutine(this.DisableAddButton());
        }
    }

    /// <summary>
    /// Makes a request to create a new environment folder.
    /// </summary>
    /// <param name="name">The name of the new folder.</param>
    /// <returns>A new create file request.</returns>
    public virtual GoogleDriveFiles.CreateRequest MakeRequest(string name)
    {
        // Create a new folder with the name of the new environment
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = name, MimeType = "application/vnd.google-apps.folder" };
        newFile.Parents = new List<string> { GoogleLogin.folderID };
        return GoogleDriveFiles.Create(newFile);
    }

    /// <summary>
    /// Initializes the selectEnvironment object.
    /// </summary>
    void Start()
    {
        this.selectEnvironment = this.gameManager.GetComponent<SelectEnvironment>();
        this.coroutineRunner = this.GetComponent<ICoroutineRunner>() ?? this.gameObject.AddComponent<CoroutineRunner>();
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
