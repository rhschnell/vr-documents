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
    /// setter of the coroutinerunner
    /// </summary>
    /// <param name="runner">the runner to set</param>
    public void SetCoroutineRunner(ICoroutineRunner runner)
    {
        this.coroutineRunner = runner;
    }

    /// <summary>
    /// getter of the coroutinerunner
    /// </summary>
    /// <returns>the runner</returns>
    public ICoroutineRunner GetCoroutineRunner()
    {
        return this.coroutineRunner;
    }

    /// <summary>
    /// the logic being called in the update function, so that its more easily testable
    /// </summary>
    public void UpdateLogic()
    {
        if (this.Request != null && this.Request.IsDone && this.once)
        {
            this.once = false;
            this.coroutineRunner.StartCoroutine(this.FindId(new GoogleDriveFiles.ListRequest(), false));
        }
    }

    /// <summary>
    /// creates the get request and send it, prompting the user to login, then getting the name and email
    /// </summary>
    /// <returns>waits until the request is done to continue</returns>
    /// <param name="r">the request to send</param>
    public virtual IEnumerator UpdateInfo(GoogleDriveAbout.GetRequest r)
    {
        AuthController.CancelAuth();

        r.Fields = new List<string> { "user" };
        yield return r.Send();
        name = r.ResponseData.User.DisplayName;
        email = r.ResponseData.User.EmailAddress;
        this.Request = r;
    }

    /// <summary>
    /// find the ID if the PaperVR folder and if it doesnt exsits yet, create it
    /// </summary>
    /// <returns>waits for things to be done until it continues</returns>
    /// <param name="r">the request to send</param>
    /// <param name="testing">if its in testing mode</param>
    public virtual IEnumerator FindId(GoogleDriveFiles.ListRequest r, bool testing)
    {
        r.Fields = new List<string> { "files(id)" };
        r.Q = $"'root' in parents and name = '{folderName}' and trashed = false";
        yield return r.Send();
        // if 0 => make one
        if (r.ResponseData.Files.Count == 0)
        {
            yield return this.CreateFolder(testing);
        }
        else
        {
            folderID = r.ResponseData.Files[0].Id;
            if (!testing)
            {
                SceneManager.LoadSceneAsync("EnviromentMenu");
            }
        }
    }

    /// <summary>
    /// creates a PaperVR folder
    /// </summary>
    /// <returns>wait until things are done before it continues</returns>
    /// <param name="testing">if its in testing mode</param>
    public virtual IEnumerator CreateFolder(bool testing)
    {
        GoogleDriveFiles.CreateRequest r = this.MakeCreateRequest();
        yield return r.Send();
        folderID = r.ResponseData.Id;
        if (!testing)
        {
            SceneManager.LoadSceneAsync("EnviromentMenu");
        }
    }

    /// <summary>
    /// Creates a request to create a folder
    /// </summary>
    /// <returns>the new request</returns>
    public virtual GoogleDriveFiles.CreateRequest MakeCreateRequest()
    {
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = folderName, MimeType = "application/vnd.google-apps.folder" };
        newFile.Parents = new List<string> { "root" };
        return GoogleDriveFiles.Create(newFile);
    }

    private void Awake()
    {
        this.settings = GoogleDriveSettings.LoadFromResources();
        this.coroutineRunner = this.GetComponent<ICoroutineRunner>() ?? this.gameObject.AddComponent<CoroutineRunner>();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.coroutineRunner.StartCoroutine(this.UpdateInfo(GoogleDriveAbout.Get()));
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateLogic();
    }
}
