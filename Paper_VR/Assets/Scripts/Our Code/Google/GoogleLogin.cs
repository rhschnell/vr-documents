using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGoogleDrive;

/// <summary>
/// class that handles the login of google
/// </summary>
public class GoogleLogin : MonoBehaviour
{
    /// <summary>
    /// the foldername you can specify
    /// </summary>
    public static string folderName = "PaperVR";

    /// <summary>
    /// the id of the PaperVR folder, make static so all can use it
    /// </summary>
    public static string folderID;

    /// <summary>
    /// the name of the user
    /// </summary>
    public static new string name;

    /// <summary>
    /// the email of the user
    /// </summary>
    public static string email;

    /// <summary>
    /// boolean to only check the data once
    /// </summary>
    public bool once = true;

    private GoogleDriveSettings settings;
    private ICoroutineRunner coroutineRunner;

    /// <summary>
    /// Gets or sets The google login request
    /// </summary>
    public virtual GoogleDriveAbout.GetRequest Request { get; set; }

    /// <summary>
    /// Gets all the files of a drive or folder
    /// </summary>
    public virtual GoogleDriveFiles.ListRequest RequestList { get; private set; }

    /// <summary>
    /// Gets a create request to create a folder
    /// </summary>
    public virtual GoogleDriveFiles.CreateRequest CreateRequest { get; private set; }

    /// <summary>
    /// setter of the coroutinerunner
    /// </summary>
    /// <param name="runner">the runner to set</param>
    public void SetCoroutineRunner(ICoroutineRunner runner)
    {
        this.coroutineRunner = runner;
    }

    /// <summary>
    /// the logic being called in the update function, so that its more easily testable
    /// </summary>
    public void UpdateLogic()
    {
        if (this.Request.IsDone && this.once)
        {
           this.once = false;
           this.coroutineRunner.StartCoroutine(this.FindId());
        }
    }

    /// <summary>
    /// creates the get request and send it, prompting the user to login, then getting the name and email
    /// </summary>
    /// <returns>waits until the request is done to continue</returns>
    public virtual IEnumerator UpdateInfo()
    {
        AuthController.CancelAuth();

        this.Request = GoogleDriveAbout.Get();
        this.Request.Fields = new List<string> { "user" };
        yield return this.Request.Send();

        name = this.Request.ResponseData.User.DisplayName;
        email = this.Request.ResponseData.User.EmailAddress;
    }

    /// <summary>
    /// find the ID if the PaperVR folder and if it doesnt exsits yet, create it
    /// </summary>
    /// <returns>waits for things to be done until it continues</returns>
    public virtual IEnumerator FindId()
    {
        this.RequestList = new GoogleDriveFiles.ListRequest();
        this.RequestList.Fields = new List<string> { "files(id)" };
        this.RequestList.Q = $"'root' in parents and name = '{folderName}' and trashed = false";
        yield return this.RequestList.Send();
        // if 0 => make one
        if (this.RequestList.ResponseData.Files.Count == 0)
        {
            yield return this.CreateFolder();
        }
        else
        {
            folderID = this.RequestList.ResponseData.Files[0].Id;
            SceneManager.LoadSceneAsync(1);
        }
    }

    /// <summary>
    /// creates a PaperVR folder
    /// </summary>
    /// <returns>wait until things are done before it continues</returns>
    public virtual IEnumerator CreateFolder()
    {
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = folderName, MimeType = "application/vnd.google-apps.folder" };
        newFile.Parents = new List<string> { "root" };
        this.CreateRequest = GoogleDriveFiles.Create(newFile);
        yield return this.CreateRequest.Send();
        yield return this.FindId();
    }

    private void Awake()
    {
        this.settings = GoogleDriveSettings.LoadFromResources();
        this.coroutineRunner = this.GetComponent<ICoroutineRunner>() ?? this.gameObject.AddComponent<CoroutineRunner>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.coroutineRunner.StartCoroutine(this.UpdateInfo());
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateLogic();
    }
}
