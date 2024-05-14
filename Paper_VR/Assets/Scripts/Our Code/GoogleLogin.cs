using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGoogleDrive;

public class GoogleLogin : MonoBehaviour
{

    private GoogleDriveSettings settings;
    public GoogleDriveAbout.GetRequest request;
    public string folderName = "PaperVR";
    public static string folderID;
    private bool Once = true;


    private void Awake()
    {
        //connect to google account

        settings = GoogleDriveSettings.LoadFromResources();
    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateInfo();

        
    }

    // Update is called once per frame
    void Update()
    {
        if(request.IsDone && Once)
        {
            Once = false;
            StartCoroutine(FindId());
            

        }
    }

    private void UpdateInfo()
    {
        AuthController.CancelAuth();

        request = GoogleDriveAbout.Get();
        request.Fields = new List<string> { "user" };
        request.Send();
    }


    public IEnumerator FindId()
    {
        GoogleDriveFiles.ListRequest requestList;
        requestList = new GoogleDriveFiles.ListRequest();
        requestList.Fields = new List<string> { "files(id)" };

        requestList.Q = $"'root' in parents and name = '{folderName}' and trashed = false";
        yield return requestList.Send();
        // if 0 => make one
        if(requestList.IsError)
        {
            print("ERROR");
        }
        if(requestList.ResponseData.Files.Count == 0)
        {
            StartCoroutine(CreateFolder());
        }
        else
        {
            folderID = requestList.ResponseData.Files[0].Id;
            SceneManager.LoadSceneAsync(1);
            //SceneManager.SetActiveScene(SceneManager.GetSceneByName("EnviromentMenu"));
            SceneManager.UnloadSceneAsync(0);
        }


        
    }

    public IEnumerator CreateFolder()
    {
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = folderName, MimeType = "application/vnd.google-apps.folder" };
        newFile.Parents = new List<string> { "root" };
        GoogleDriveFiles.CreateRequest createRequest = GoogleDriveFiles.Create(newFile);

        yield return createRequest.Send();
        print("Made new folder!");
        StartCoroutine(FindId());
    }
}
