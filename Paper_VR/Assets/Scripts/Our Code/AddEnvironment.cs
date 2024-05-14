using System.Collections;
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
    private SelectEnvironment SelectEnvironment;

    /// <summary>
    /// Adds an environment
    /// </summary>
    public void AddEnvironmentButton()
    {
        string environmentName = InputField.text;
        if (environmentName == ""){
            print("Name must not be empty!");
        }
        else {
            UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = environmentName, MimeType = "application/vnd.google-apps.folder" };
            newFile.Parents = new List<string> { GoogleLogin.folderID };
            GoogleDriveFiles.CreateRequest createRequest = GoogleDriveFiles.Create(newFile);
            createRequest.Send();
        }
    }
}
