using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityGoogleDrive;
using UnityEngine.UIElements;

public class SelectEnvironment : MonoBehaviour
{
    public TMP_Dropdown Dropdown;
    private GoogleDriveSettings GoogleDriveSettings;
    private GoogleDriveRequest LoginRequest;
    private GoogleDriveFiles.ListRequest requestList;


    private async void Awake()
    {
        //StartCoroutine(UpdateList());
        Refresh();
    }
    public void AddEnvironmentButton()
    {
        StartCoroutine(FindPDF());
    }

    public IEnumerator FindPDF()
    {
        int selectedIndex = Dropdown.value;
        string selectedEnvironmentName = Dropdown.options[selectedIndex].text;

        string parentId = requestList.ResponseData.Files[selectedIndex].Id;
        GoogleDriveFiles.ListRequest envReq = new GoogleDriveFiles.ListRequest();
        envReq.Fields = new List<string> { "files(id, name)" };
        envReq.Q = $"'{parentId}' in parents and name contains '.pdf' and trashed = false";
        yield return envReq.Send();

        print(envReq.ResponseData.Files.Count);
    }

    public void Refresh()
    {
        StartCoroutine(UpdateList());
    }

    public IEnumerator UpdateList()
    {
        requestList = new GoogleDriveFiles.ListRequest();
        requestList.Fields = new List<string> { "files(id, name)" };
        requestList.Q = $"'{GoogleLogin.folderID}' in parents and trashed = false and mimeType = 'application/vnd.google-apps.folder'";

        yield return requestList.Send();

        List<string> environmentNames = new List<string>();
        foreach(var folder in requestList.ResponseData.Files)
        {
            environmentNames.Add(folder.Name);
        }
        Dropdown.ClearOptions();
        Dropdown.AddOptions(environmentNames);
    }
}
