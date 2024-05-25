using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// This class is responsible for selecting the environment, it creates a dropdown containing all existing
/// environments and when pressing the button, the scene is loaded and opened.
/// </summary>
public class SelectEnvironment : MonoBehaviour
{
    /// <summary>
    /// The parent id of the selected environment.
    /// </summary>
    public static string parentId;

    /// <summary>
    /// The dropdown containing all environments.
    /// </summary>
    public TMP_Dropdown dropdown;

    /// <summary>
    /// The manager of the game, containing all inportant information.
    /// </summary>
    public GameObject gameManager;

    /// <summary>
    /// The name textbox
    /// </summary>
    public TMP_Text Name;

    /// <summary>
    /// The email textbox
    /// </summary>
    public TMP_Text Email;

    /// <summary>
    /// boolean to indicate if the script is being tested, to make sure requests are not sent.
    /// </summary>
    public bool testing = false;

    /// <summary>
    /// Gets or sets The request list to get the list of folders.
    /// </summary>
    public virtual GoogleDriveFiles.ListRequest RequestList { get; set; }

    /// <summary>
    /// Gets or sets A coroutine runner to run the coroutines.
    /// </summary>
    public virtual ICoroutineRunner CoroutineRunner { get; set; }

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
        EnvironmentInfo envInfo = new EnvironmentInfo();

        parentId = this.RequestList.ResponseData.Files[this.dropdown.value].Id;

        this.LoadNewScene(envInfo);
    }

    /// <summary>
    /// Calls a refresh on the list of folders and the name and email
    /// </summary>
    public void Refresh()
    {
        this.Name.text = "Name: " + GoogleLogin.name;
        this.Email.text = "Email: " + GoogleLogin.email;
        if (!this.testing)
        {
            this.CoroutineRunner.StartCoroutine(this.UpdateList(new GoogleDriveFiles.ListRequest()));
        }
    }

    /// <summary>
    /// finds all folders under the main folder and lists their names
    /// </summary>
    /// <returns>Waits for the request to finish</returns>
    /// <param name="r">The request to get the list of folders</param>
    public IEnumerator UpdateList(GoogleDriveFiles.ListRequest r)
    {
        r.Fields = new List<string> { "files(id, name)" };
        r.Q = $"'{GoogleLogin.folderID}' in parents and trashed = false and mimeType = 'application/vnd.google-apps.folder'";
        yield return r.Send();

        if (!r.IsError)
        {
            List<string> environmentNames = new List<string>();
            foreach (var folder in r.ResponseData.Files)
            {
                environmentNames.Add(folder.Name);
            }

            this.dropdown.ClearOptions();
            this.dropdown.AddOptions(environmentNames);
        }

        this.RequestList = r;
    }

    /// <summary>
    /// This method will set the environment information to the new environment information and load the scene.
    /// </summary>
    /// <param name="envInfo">The environment info needed for loading the scene.</param>
    private void LoadNewScene(EnvironmentInfo envInfo)
    {
        // Gets the environment information component of the game manager and loads the new
        // Environment information on to it.
        EnvironmentInformation envInformation = this.gameManager.GetComponent<EnvironmentInformation>()
            ?? this.gameManager.AddComponent<EnvironmentInformation>();
        envInformation.LoadNewInformation(envInfo);

        // Loads the environment scene.
        SceneManager.LoadSceneAsync("Environment");
    }

    void Start()
    {
        this.CoroutineRunner = this.GetComponent<ICoroutineRunner>() ?? this.gameObject.AddComponent<CoroutineRunner>();
        this.Refresh();
    }
}
