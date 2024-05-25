using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGoogleDrive;

/// <summary>
/// This class adds environments to documents
/// </summary>
public class AddEnvironment : MonoBehaviour
{
    /// <summary>
    /// The input field where the environment name should be inserted.
    /// </summary>
    public TMP_InputField InputField;

    /// <summary>
    /// the error message text field
    /// </summary>
    public TMP_Text error;

    /// <summary>
    /// Adds an environment
    /// </summary>
    public void AddEnvironmentButton()
    {
        string environmentName = this.InputField.text;
        if (environmentName == "")
        {
            this.error.text = "Name must not be empty!";
        }
        else
        {
            this.error.text = "";
            // Create a new environment folder in the PaperVR folder
            GoogleDriveFiles.CreateRequest createRequest = this.MakeRequest(environmentName);
            createRequest.Send();
        }
    }

    /// <summary>
    /// Makes a request to create a new environment folder
    /// </summary>
    /// <param name="name">the name of the new folder</param>
    /// <returns>a new create file request</returns>
    public virtual GoogleDriveFiles.CreateRequest MakeRequest(string name)
    {
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = name, MimeType = "application/vnd.google-apps.folder" };
        newFile.Parents = new List<string> { GoogleLogin.folderID };
        return GoogleDriveFiles.Create(newFile);
    }
}
