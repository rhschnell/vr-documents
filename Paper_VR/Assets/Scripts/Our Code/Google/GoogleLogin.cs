using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGoogleDrive;

/// <summary>
/// Class that handles the login of Google.
/// </summary>
public class GoogleLogin : MonoBehaviour
{
    /// <summary>
    /// The foldername you can specify.
    /// </summary>
    public static string folderName = "PaperVR";

    /// <summary>
    /// The id of the PaperVR folder, make static so all can use it.
    /// </summary>
    public static string folderID;

    /// <summary>
    /// The name of the user.
    /// </summary>
    public static new string name;

    /// <summary>
    /// The email of the user.
    /// </summary>
    public static string email;

    /// <summary>
    /// Boolean to only check the data once.
    /// </summary>
    public bool once = true;

    private GoogleDriveSettings settings;
    private ICoroutineRunner coroutineRunner;

    /// <summary>
    /// Gets or sets The google login request.
    /// </summary>
    public virtual GoogleDriveAbout.GetRequest Request { get; set; }

    /// <summary>
    /// Setter of the coroutine runner.
    /// </summary>
    /// <param name="runner">The runner to set.</param>
    public void SetCoroutineRunner(ICoroutineRunner runner)
    {
        // Set the coroutine runner to the provided runner instance
        this.coroutineRunner = runner;
    }

    /// <summary>
    /// Getter of the coroutine runner.
    /// </summary>
    /// <returns>The runner.</returns>
    public ICoroutineRunner GetCoroutineRunner()
    {
        // Return the current coroutine runner instance
        return this.coroutineRunner;
    }

    /// <summary>
    /// The logic being called in the update function, so that its more easily testable.
    /// </summary>
    public void UpdateLogic()
    {
        // Check if the request is done and if its the first time, then start finding the folder ID
        if (this.Request != null && this.Request.IsDone && this.once)
        {
            this.once = false;
            this.coroutineRunner.StartCoroutine(this.FindId(new GoogleDriveFiles.ListRequest(), false));
        }
    }

    /// <summary>
    /// Creates the get request and send it, prompting the user to login, then getting the name and email.
    /// </summary>
    /// <returns>Waits until the request is done to continue.</returns>
    /// <param name="r">The request to send.</param>
    public virtual IEnumerator UpdateInfo(GoogleDriveAbout.GetRequest r)
    {
        // Cancel any ongoing authentication process
        AuthController.CancelAuth();

        // Define the fields to retrieve
        r.Fields = new List<string> { "user" };

        // Send the request and wait for it to complete
        yield return r.Send();

        // Set the users name and email from the response data
        name = r.ResponseData.User.DisplayName;
        email = r.ResponseData.User.EmailAddress;
        this.Request = r;
    }

    /// <summary>
    /// Find the ID if the PaperVR folder and if it doesnt exsits yet, create it.
    /// </summary>
    /// <returns>Waits for things to be done until it continues.</returns>
    /// <param name="r">The request to send.</param>
    /// <param name="testing">If its in testing mode.</param>
    public virtual IEnumerator FindId(GoogleDriveFiles.ListRequest r, bool testing)
    {
        // Define the fields to retrieve
        r.Fields = new List<string> { "files(id)" };
        r.Q = $"'root' in parents and name = '{folderName}' and trashed = false";
        yield return r.Send();

        // If the PaperVR folder doesn't exist, create it
        if (r.ResponseData.Files.Count == 0)
        {
            // Create the PaperVR folder
            yield return this.CreateFolder(testing);
        }
        else
        {
            // Retrieve the ID of the existing PaperVR folder and load the environment menu scene if not in testing mode
            folderID = r.ResponseData.Files[0].Id;

            if (!testing)
            {
                SceneManager.LoadSceneAsync("EnviromentMenu");
            }
        }
    }

    /// <summary>
    /// Creates a PaperVR folder
    /// </summary>
    /// <returns>Wait until things are done before it continues.</returns>
    /// <param name="testing">If its in testing mode.</param>
    public virtual IEnumerator CreateFolder(bool testing)
    {
        // Create a request to create a PaperVR folder
        GoogleDriveFiles.CreateRequest r = this.MakeCreateRequest();
        yield return r.Send();

        // Set the folderID to the ID of the created folder
        folderID = r.ResponseData.Id;
        if (!testing)
        {
            SceneManager.LoadSceneAsync("EnviromentMenu");
        }
    }

    /// <summary>
    /// Creates a request to create a folder.
    /// </summary>
    /// <returns>The new request.</returns>
    public virtual GoogleDriveFiles.CreateRequest MakeCreateRequest()
    {
        // Create a new file object representing the folder
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = folderName, MimeType = "application/vnd.google-apps.folder" };
        newFile.Parents = new List<string> { "root" };

        // Create and return a new request to create the folder
        return GoogleDriveFiles.Create(newFile);
    }

    /// <summary>
    /// Initializes Google Drive settings and coroutine runner on awake.
    /// </summary>
    private void Awake()
    {
        // Load Google Drive settings from resources
        this.settings = GoogleDriveSettings.LoadFromResources();

        // Get or add the CoroutineRunner component
        this.coroutineRunner = this.GetComponent<ICoroutineRunner>() ?? this.gameObject.AddComponent<CoroutineRunner>();
    }

    /// <summary>
    /// Initializes the Google Login by updating user information.
    /// </summary>
    void Start()
    {
        // Start coroutine to update user information
        this.coroutineRunner.StartCoroutine(this.UpdateInfo(GoogleDriveAbout.Get()));
    }

    /// <summary>
    /// Updates the logic for Google Login.
    /// </summary>
    void Update()
    {
        // Update logic for Google Login
        this.UpdateLogic();
    }
}
