using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    /// This list contains all the ids of the savedDocument folders.
    /// </summary>
    public List<string> savedFolderIds;

    /// <summary>
    /// The manager of the game, containing all inportant information.
    /// </summary>
    public GameObject gameManager;

    /// <summary>
    /// The button which can be clicked to open an environment.
    /// </summary>
    public Button openEnvironmentButton;

    /// <summary>
    /// The name textbox.
    /// </summary>
    public TMP_Text Name;

    /// <summary>
    /// The email textbox.
    /// </summary>
    public TMP_Text Email;

    /// <summary>
    /// The delete textbox information.
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
    public void SelectEnvironmentButton()
    {
        // Load the new scene using the environment information
        parentId = this.RequestList.ResponseData.Files[this.dropdown.value].Id;

        string envName = this.dropdown.options[this.dropdown.value].text;

        EnvironmentController.saveFolderId = this.savedFolderIds[this.dropdown.value];

        this.CoroutineRunner.StartCoroutine(this.GetEnvironment(parentId, envName));

        // Make the open button not interactable to prevent spam clicking
        this.StartCoroutine(this.DisableOpenButton());
    }

    /// <summary>
    /// Changes the DeleteText to the selected environment to include the environments name that has to be deleted.
    /// </summary>
    public void ChangeDeleteText()
    {
        string n = this.RequestList.ResponseData.Files[this.dropdown.value].Id;

        // Prompt the user with a confirmation text to avoid unwanted deletions of environments
        this.DeleteText.text = "Are you sure you want to delete " + this.dropdown.options[this.dropdown.value].text + "?";
    }

    /// <summary>
    /// Is called when the button is pressed, finding the correct ID and then creating the delete request.
    /// </summary>
    public void DeleteEnvironmentButton()
    {
        string s = this.RequestList.ResponseData.Files[this.dropdown.value].Id;

        // Creates the delete request
        this.CoroutineRunner.StartCoroutine(this.DeleteEnvironmenet(new GoogleDriveFiles.DeleteRequest(s)));
    }

    /// <summary>
    /// Deletes the selected environment.
    /// </summary>
    /// <param name="r">The request.</param>
    /// <returns>Needs to wait for request return.</returns>
    public IEnumerator DeleteEnvironmenet(GoogleDriveFiles.DeleteRequest r)
    {
        // Waits for the request to return, then deletes the environment
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
        // Create a new download request for the file with the given id
        GoogleDriveFiles.DownloadRequest req = new GoogleDriveFiles.DownloadRequest(id);
        UnityGoogleDrive.Data.File returnFile = null;

        // Send the request and set the returnFile once the download is done
        req.Send().OnDone += (UnityGoogleDrive.Data.File file) => { returnFile = file; };
        return returnFile;
    }

    /// <summary>
    /// This method will get the environment information from the selected environment and load the scene.
    /// </summary>
    /// <param name="id">The id of the folder,</param>
    /// <param name="envName">The name of the enviorment.</param>
    /// <returns>An IEnumerator.</returns>
    public IEnumerator GetEnvironment(string id, string envName)
    {
        // Find the json file of the selected environment
        GoogleDriveFiles.ListRequest requestList = this.CreateNewListRequest(id);

        yield return requestList.Send();

        EnvironmentInformation envInfo = new EnvironmentInformation();

        // Check if the file exists, if not create an empty environment
        if (requestList.ResponseData.Files.Count == 0)
        {
            envInfo.environmentName = envName;
            envInfo.floatingDocuments = new List<FloatingDocumentInfo>();
            envInfo.importList = new List<string>();
            envInfo.backgroundColor = Color.white;

            Material loadedMaterial = Resources.Load<Material>("Material/Default");
            envInfo.material = loadedMaterial;
        }
        else
        {
            // Get the json file
            yield return this.googleMethods.DownloadFile(requestList.ResponseData.Files[0].Id);

            // Get the json from the request
            var content = this.googleMethods.returnFile.Content;
            Debug.Log(content);
            string json = System.Text.Encoding.ASCII.GetString(content);
            envInfo = EnvironmentInformation.LoadFromJson(json);
        }

        this.RequestList = requestList;
        this.LoadNewScene(envInfo);
    }

    /// <summary>
    /// Gets and return a new list request from the Google Drive.
    /// </summary>
    /// <param name="id">The ID for the Google Drive.</param>
    /// <returns>The new request list.</returns>
    public virtual GoogleDriveFiles.ListRequest CreateNewListRequest(string id)
    {
        // Make new list request
        var r = new GoogleDriveFiles.ListRequest();
        r.Fields = new List<string> { "files(id, name)" };
        r.Q = $"'{id}' in parents and name contains 'Environment.json' and trashed = false";

        return r;
    }

    /// <summary>
    /// Calls a refresh on the list of folders and the name and email.
    /// </summary>
    public void Refresh()
    {
        // Initialize the request list if it is null
        if (this.RequestList == null)
        {
            this.RequestList = new GoogleDriveFiles.ListRequest();
        }

        // Update the name and email text fields with the logged-in user's information
        this.Name.text = "Name: " + GoogleLogin.name;
        this.Email.text = "Email: " + GoogleLogin.email;

        if (!this.testing)
        {
            // Start the coroutine to update the list of folders
            this.CoroutineRunner.StartCoroutine(this.UpdateList(new GoogleDriveFiles.ListRequest()));

            Debug.Log("List updated");
            Debug.Log("List length: " + this.savedFolderIds.Count);
        }
    }

    /// <summary>
    /// Finds all folders under the main folder and lists their names.
    /// </summary>
    /// <returns>Waits for the request to finish.</returns>
    /// <param name="r">The request to get the list of folders.</param>
    public IEnumerator UpdateList(GoogleDriveFiles.ListRequest r)
    {
        this.openEnvironmentButton.interactable = false;
        // Specify the fields to retrieve and the query to filter folders
        r.Fields = new List<string> { "files(id, name)" };
        r.Q = $"'{GoogleLogin.folderID}' in parents and trashed = false and mimeType = 'application/vnd.google-apps.folder'";
        yield return r.Send();

        // If the request is successful, process the retrieved folders
        if (!r.IsError)
        {
            this.savedFolderIds = new List<string>();
            List<string> environmentNames = new List<string>();
            foreach (var folder in r.ResponseData.Files)
            {
                // Start coroutine to check if each folder contains saved documents
                yield return this.HasSavedDocFolder(folder.Id, new GoogleDriveFiles.ListRequest());
                environmentNames.Add(folder.Name);
            }

            // Clear the dropdown options and add the new folder names
            this.dropdown.ClearOptions();
            this.dropdown.AddOptions(environmentNames);
        }

        this.RequestList = r;
        this.openEnvironmentButton.interactable = true;
    }

    /// <summary>
    /// Checks if the Saved Documents folder exists in the selected folder, if not it will create it.
    /// </summary>
    /// <param name="parentID">The id of the folder to check.</param>
    /// <param name="r">the new request, done for testing.</param>
    /// <returns>so you can wait</returns>
    public virtual IEnumerator HasSavedDocFolder(string parentID, GoogleDriveFiles.ListRequest r)
    {
        // Specify the fields to retrieve and the query to check for the saved documents folder
        r.Fields = new List<string> { "files(id, name)" };
        r.Q = $"'{parentID}' in parents and trashed = false and mimeType = 'application/vnd.google-apps.folder'";
        yield return r.Send();

        if (r.ResponseData.Files.Count == 0)
        {
            // If the environment has no Saved Documents folder, create it
            yield return this.CreateSavedDocFolder(parentID);
        } else
        {
            // Get the id of the saved documents folder
            this.savedFolderIds.Add(r.ResponseData.Files[0].Id);
        }
    }

    /// <summary>
    /// Creates the Saved Documents folder in the selected folder.
    /// </summary>
    /// <param name="parentID">The folder to create it in.</param>
    /// <returns>An IEnumerator.</returns>
    public virtual IEnumerator CreateSavedDocFolder(string parentID)
    {
        // Create a request to create the Saved Documents folder in the specified parent folder
        GoogleDriveFiles.CreateRequest r = this.MakeRequest(parentID);
        yield return r.Send();

        // Add the created folders ID to the list of saved folder IDs
        this.savedFolderIds.Add(r.ResponseData.Id);
    }

    /// <summary>
    /// Creates the request to create the Saved Documents folder.
    /// </summary>
    /// <param name="parentID">The parent ID.</param>
    /// <returns>A waitable object.</returns>
    public virtual GoogleDriveFiles.CreateRequest MakeRequest(string parentID)
    {
        // Define the new folder's properties
        UnityGoogleDrive.Data.File folder = new UnityGoogleDrive.Data.File {
            Name = "Saved Documents",
            MimeType = "application/vnd.google-apps.folder",
            Parents = new List<string> { parentID },
        };

        // Return a new request to create the folder
        return new GoogleDriveFiles.CreateRequest(folder);
    }

    /// <summary>
    /// This method will set the environment information to the new environment information and load the scene.
    /// </summary>
    /// <param name="envInfo">The environment info needed for loading the scene.</param>
    private void LoadNewScene(EnvironmentInformation envInfo)
    {
        // Gets the environment information component of the game manager and loads the new
        // Environment information on to it
        EnvironmentController envInformation = this.gameManager.GetComponent<EnvironmentController>();

        envInformation.LoadNewInformation(envInfo);

        // Loads the environment scene
        SceneManager.LoadSceneAsync("Environment");
    }

    /// <summary>
    /// Makes the open environment button not interactable for three seconds.
    /// </summary>
    /// <returns>An IEnumerator.</returns>
    private IEnumerator DisableOpenButton()
    {
        // Make the open environment button not interactable
        this.openEnvironmentButton.interactable = false;

        // Wait for one second
        yield return new WaitForSeconds(5.0f);

        // Make the open environment button interactable again
        this.openEnvironmentButton.interactable = true;
    }

    /// <summary>
    /// Initializes the CoroutineRunner and refreshes the folder list on start.
    /// </summary>
    void Start()
    {
        // Get the CoroutineRunner component or add it if not present
        this.CoroutineRunner = this.GetComponent<ICoroutineRunner>() ?? this.gameObject.AddComponent<CoroutineRunner>();
        this.Refresh();
    }
}