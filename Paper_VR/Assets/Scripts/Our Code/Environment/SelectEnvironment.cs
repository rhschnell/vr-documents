using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGoogleDrive;

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
    /// The delete textbox information
    /// </summary>
    public TMP_Text DeleteText;

    /// <summary>
    /// boolean to indicate if the script is being tested, to make sure requests are not sent.
    /// </summary>
    public bool testing = false;

    /// <summary>
    /// The google methods.
    /// </summary>
    public GoogleMethods googleMethods = new GoogleMethods();
    private GoogleDriveSettings GoogleDriveSettings;
    private GoogleDriveRequest LoginRequest;

    /// <summary>
    /// Gets or sets The request list to get the list of folders.
    /// </summary>
    public virtual GoogleDriveFiles.ListRequest RequestList { get; set; } = new GoogleDriveFiles.ListRequest();


    /// <summary>
    /// Gets or sets A coroutine runner to run the coroutines.
    /// </summary>
    public virtual ICoroutineRunner CoroutineRunner { get; set; }

    /// <summary>
    /// When the select button is clicked, this method will be called. This will result in the
    /// the yaml file being received that corresponds to the selected dropwon item, the conversion to environment information
    /// and the loading of the scene using this environment information.
    /// </summary>
    public void SelectEnvironmentButton()
    {
        // Load the new scene using the environment information
        Debug.Log(this.RequestList.ResponseData.Files.ToString());
        parentId = this.RequestList.ResponseData.Files[this.dropdown.value].Id;

        string envName = this.dropdown.options[this.dropdown.value].text;

        this.CoroutineRunner.StartCoroutine(this.GetEnvironment(parentId, envName));
    }

    /// <summary>
    /// Changes the DeleteText to the selected environment to include the environments name that has to be deleted.
    /// </summary>
    public void ChangeDeleteText()
    {
        string n = this.RequestList.ResponseData.Files[this.dropdown.value].Id;
        this.DeleteText.text = "Are you sure you want to delete " + this.dropdown.options[this.dropdown.value].text + "?";
    }

    /// <summary>
    /// Is called when the button is pressed, finding the correct ID and then creating the delete request
    /// </summary>
    public void DeleteEnvironmentButton()
    {
        string s = this.RequestList.ResponseData.Files[this.dropdown.value].Id;
        this.CoroutineRunner.StartCoroutine(this.DeleteEnvironmenet(new GoogleDriveFiles.DeleteRequest(s)));
    }

    /// <summary>
    /// Deletes the selected environment
    /// </summary>
    /// <param name="r">the request</param>
    /// <returns>Needs to wait for request return</returns>
    public IEnumerator DeleteEnvironmenet(GoogleDriveFiles.DeleteRequest r)
    {
        yield return r.Send();
        this.CoroutineRunner.StartCoroutine(this.UpdateList(new GoogleDriveFiles.ListRequest()));
    }

    /// <summary>
    /// The method that downloads the file.
    /// </summary>
    /// <param name="id">The id of the file that needs to be downloaded.</param>
    /// <returns>Returns the file.</returns>
    public virtual UnityGoogleDrive.Data.File Download(string id)
    {
        GoogleDriveFiles.DownloadRequest req = new GoogleDriveFiles.DownloadRequest(id);
        UnityGoogleDrive.Data.File returnFile = null;
        req.Send().OnDone += (UnityGoogleDrive.Data.File file) => { returnFile = file; };
        return returnFile;
    }

    /// <summary>
    /// This method will get the environment information from the selected environment and load the scene.
    /// </summary>
    /// <param name="id">The id of the folder</param>
    /// <param name="envName">the name of the enviorment</param>
    /// <returns>An IEnumerator</returns>
    public IEnumerator GetEnvironment(string id, string envName)
    {
        // Find the json file of the selected environment
        this.RequestList = new GoogleDriveFiles.ListRequest();
        this.RequestList.Fields = new List<string> { "files(id, name)" };
        this.RequestList.Q = $"'{id}' in parents and name contains 'Environment.json' and trashed = false";

        yield return this.RequestList.Send();

        EnvironmentInfo envInfo = new EnvironmentInfo();

        // Check if the file exists, if not create an empty environment
        if (this.RequestList.ResponseData.Files.Count == 0)
        {
            envInfo.environmentName = envName;
            envInfo.floatingDocuments = new List<FloatingDocumentInfo>();
            envInfo.importList = new List<string>();
            envInfo.backgroundColor = Color.white;
        }
        else
        {
            // Get the json file
            yield return this.googleMethods.DownloadFile(this.RequestList.ResponseData.Files[0].Id);

            // Get the json from the request
            var content = this.googleMethods.returnFile.Content;
            Debug.Log(content);
            string json = System.Text.Encoding.ASCII.GetString(content);
            envInfo = EnvironmentInfo.LoadFromJson(json);
        }

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
                this.CoroutineRunner.StartCoroutine(this.HasSavedDocFolder(folder.Id, new GoogleDriveFiles.ListRequest()));
                environmentNames.Add(folder.Name);
            }

            this.dropdown.ClearOptions();
            this.dropdown.AddOptions(environmentNames);
        }

        this.RequestList = r;
    }

    /// <summary>
    /// Checks if the Saved Documents folder exists in the selected folder, if not it will create it.
    /// </summary>
    /// <param name="parentID">The id of the folder to check</param>
    /// <param name="r">the new request, done for testing</param>
    /// <returns>so you can wait</returns>
    public virtual IEnumerator HasSavedDocFolder(string parentID, GoogleDriveFiles.ListRequest r)
    {
        r.Fields = new List<string> { "files(id, name)" };
        r.Q = $"'{parentID}' in parents and trashed = false and mimeType = 'application/vnd.google-apps.folder'";
        yield return r.Send();
        if (r.ResponseData.Files.Count == 0)
        {
            this.CoroutineRunner.StartCoroutine(this.CreateSavedDocFolder(parentID));
        }
    }

    /// <summary>
    /// Creates the Saved Documents folder in the selected folder
    /// </summary>
    /// <param name="parentID">the folder to create it in</param>
    /// <returns>so it can wait</returns>
    public virtual IEnumerator CreateSavedDocFolder(string parentID)
    {
        GoogleDriveFiles.CreateRequest r = this.MakeRequest(parentID);
        yield return r.Send();
    }

    /// <summary>
    /// Creates the request to create the Saved Documents folder
    /// </summary>
    /// <param name="parentID">The parent ID</param>
    /// <returns>A waitable object</returns>
    public virtual GoogleDriveFiles.CreateRequest MakeRequest(string parentID)
    {
        UnityGoogleDrive.Data.File folder = new UnityGoogleDrive.Data.File { Name = "Saved Documents", MimeType = "application/vnd.google-apps.folder", Parents = new List<string> { parentID } };
        return new GoogleDriveFiles.CreateRequest(folder);
    }

    /// <summary>
    /// This method will set the environment information to the new environment information and load the scene.
    /// </summary>
    /// <param name="envInfo">The environment info needed for loading the scene.</param>
    private void LoadNewScene(EnvironmentInfo envInfo)
    {
        // Gets the environment information component of the game manager and loads the new
        // Environment information on to it.
        EnvironmentInformation envInformation = this.gameManager.GetComponent<EnvironmentInformation>();
        // Debug.Log("first element is: " + envInfo.floatingDocuments[0].position.ToString());

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