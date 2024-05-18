using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityGoogleDrive;

/// <summary>
/// This class is responsible for selecting the environment, it creates a dropdown containing all existing
/// environments and when pressing the button, the scene is loaded and opened.
/// </summary>
public class SelectEnvironment : MonoBehaviour
{
    /// <summary>
    /// The dropdown containing all environments.
    /// </summary>
    public TMP_Dropdown dropdown;

    /// <summary>
    /// The manager of the game, containing all inportant information.
    /// </summary>
    public GameObject gameManager;
    private GoogleDriveSettings GoogleDriveSettings;
    private GoogleDriveRequest LoginRequest;
    private GoogleDriveFiles.ListRequest requestList;

    /// <summary>
    /// When the select button is clicked, this method will be called. This will result in the
    /// the yaml file being received that corresponds to the selected dropwon item, the conversion to environment information
    /// and the loading of the scene using this environment information.
    /// </summary>
    public void AddEnvironmentButton()
    {
        // Yamlfile environmentInformation = StartCoroutine(FindPDF());
        // EnvironmentInformation envInfo = getSceneInfo(environmentInformation)

        // A placeholder for the environment information.
        EnvironmentInformation envInfo = new EnvironmentInformation();

        // Load the new scene using the environment information.
        this.LoadNewScene(envInfo);
    }

    /// <summary>
    /// WILLEM PLEASE DOCUMENT
    /// </summary>
    /// <returns>WILLEM PLEASE DOCUMENT.</returns>
    public IEnumerator FindPDF()
    {
        int selectedIndex = this.dropdown.value;
        // string selectedEnvironmentName = this.dropdown.options[selectedIndex].text;

        string parentId = this.requestList.ResponseData.Files[selectedIndex].Id;
        GoogleDriveFiles.ListRequest envReq = new GoogleDriveFiles.ListRequest();
        envReq.Fields = new List<string> { "files(id, name)" };
        envReq.Q = $"'{parentId}' in parents and name contains '.pdf' and trashed = false";
        yield return envReq.Send();

        print(envReq.ResponseData.Files.Count);
    }

    /// <summary>
    /// WILLEM PLEASE DOCUMENT
    /// </summary>
    public void Refresh()
    {
        // this.StartCoroutine(this.UpdateList());
    }

    /// <summary>
    /// WILLEM PLEASE DOCUMENT
    /// </summary>
    /// <returns>WILLEM PLEASE DOCUMENT.</returns>
    public IEnumerator UpdateList()
    {
        this.requestList = new GoogleDriveFiles.ListRequest();
        this.requestList.Fields = new List<string> { "files(id, name)" };
        this.requestList.Q = $"'{GoogleLogin.folderID}' in parents and trashed = false and mimeType = 'application/vnd.google-apps.folder'";

        yield return this.requestList.Send();

        List<string> environmentNames = new List<string>();
        foreach (var folder in this.requestList.ResponseData.Files)
        {
            environmentNames.Add(folder.Name);
        }

        this.dropdown.ClearOptions();
        this.dropdown.AddOptions(environmentNames);
    }

    /// <summary>
    /// This method will set the environment information to the new environment information and load the scene.
    /// </summary>
    /// <param name="envInfo">The environment info needed for loading the scene.</param>
    private void LoadNewScene(EnvironmentInformation envInfo)
    {
        // Gets the environment information component of the game manager and loads the new
        // Environment information on to it.
        EnvironmentInformation envInformation = this.gameManager.GetComponent<EnvironmentInformation>();
        envInformation.LoadNewInformation(envInfo);

        // Loads the environment scene.
        SceneManager.LoadScene("Environment");
    }

    /// <summary>
    /// WILLEM PLEASE DOCUMENT
    /// </summary>
    private void Awake()
    {
        // this.StartCoroutine(this.UpdateList());
        // this.Refresh();
    }
}
