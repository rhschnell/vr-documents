using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

public class BackendPipeline : AdaptiveWindowGUI
{

    private GoogleDriveAbout.GetRequest request;
    private GoogleDriveSettings settings;
    private GoogleDriveFiles.ListRequest requestList;
    private string filePath = string.Empty;
    private string result = string.Empty;

    protected override void Awake()
    {
        base.Awake();
        settings = GoogleDriveSettings.LoadFromResources();
    }

    //Steps:
    //start by logging into your google account with the first file,
    //then find the PaperVR folder in the drive
    //then try and list all files under that folder

    // Start is called before the first frame update
    void Start()
    {
        UpdateInfo();
        
    }

    protected override void OnWindowGUI(int windowId)
    {
        if (request.IsRunning)
        {
            GUILayout.Label($"Loading: {request.Progress:P2}");
        }
        else
        {
            if (GUILayout.Button("Refresh"))
                UpdateInfo();
        }

        if (settings.IsAnyAuthTokenCached() && GUILayout.Button("Delete Cached Tokens"))
            settings.DeleteCachedAuthTokens();

        if (request.ResponseData != null)
        {
            GUILayout.Label(string.Format("User name: {0}\nUser email: {1}",
                request.ResponseData.User.DisplayName,
                request.ResponseData.User.EmailAddress));

            if (requestList != null && requestList.IsRunning)
            {
                GUILayout.Label($"Loading: {requestList.Progress:P2}");
            }
            else
            {
                //GUILayout.BeginHorizontal();
                //GUILayout.Label("File path:", GUILayout.Width(70));
                //filePath = GUILayout.TextField(filePath);
                if (GUILayout.Button("Get", GUILayout.Width(100)))
                    StartCoroutine(GetFileByPathRoutine("PaperVR"));
                //GUILayout.EndHorizontal();
            }

            if (!string.IsNullOrEmpty(result))
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Result:", GUILayout.Width(70));
                result = GUILayout.TextField(result);
                GUILayout.EndHorizontal();
            }
        }

        if (request.IsError)
            GUILayout.Label(string.Format("Request failed: {0}", request.Error));
    }


    private void UpdateInfo()
    {
        AuthController.CancelAuth();

        request = GoogleDriveAbout.Get();
        request.Fields = new List<string> { "user", "storageQuota" };
        request.Send();
    }

    private IEnumerator GetFileByPathRoutine(string filePath)
    {
        // A folder in Google Drive is actually a file with the MIME type 'application/vnd.google-apps.folder'.
        // Hierarchy relationship is implemented via File's 'Parents' property. To get the actual file using it's path
        // we have to find ID of the file's parent folder, and for this we need IDs of all the folders in the chain.
        // Thus, we need to traverse the entire hierarchy chain using List requests.
        // More info about the Google Drive folders: https://developers.google.com/drive/v3/web/folder.

        var fileName = filePath.Contains("/") ? GetAfter(filePath, "/") : filePath;
        var parentNames = filePath.Contains("/") ? GetBeforeLast(filePath, "/").Split('/') : null;

        // Resolving folder IDs one by one to find ID of the file's parent folder.
        var parentId = "root"; // 'root' is alias ID for the root folder in Google Drive.
        if (parentNames != null && false)
        {
            for (int i = 0; i < parentNames.Length; i++)
            {
                requestList = new GoogleDriveFiles.ListRequest();
                requestList.Fields = new List<string> { "files(id)" };
                requestList.Q = $"'{parentId}' in parents and name = '{parentNames[i]}' and mimeType = 'application/vnd.google-apps.folder' and trashed = false";

                yield return request.Send();

                if (requestList.IsError || requestList.ResponseData.Files == null || requestList.ResponseData.Files.Count == 0)
                {
                    result = $"Failed to retrieve '{parentNames[i]}' part of '{filePath}' file path.";
                    yield break;
                }

                if (requestList.ResponseData.Files.Count > 1)
                    Debug.LogWarning($"Multiple '{parentNames[i]}' folders been found.");

                parentId = requestList.ResponseData.Files[0].Id;
            }
        }

        // Searching the file.
        requestList = new GoogleDriveFiles.ListRequest();
        requestList.Fields = new List<string> { "files(id, size, modifiedTime)" };
        requestList.Q = $"'{parentId}' in parents and name = '{fileName}' and trashed = false";
        //requestList.Q = $"'{parentId}' in parents and name ";

        //print(requestList);

        yield return requestList.Send();

        if (requestList.IsError || requestList.ResponseData.Files == null || requestList.ResponseData.Files.Count == 0)
        {
            result = $"Failed to retrieve '{filePath}' file.";

            // maak niewe folder
            UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = "PaperVR", MimeType = "application/vnd.google-apps.folder" };
            newFile.Parents = new List<string> { "root" };
            GoogleDriveFiles.CreateRequest createRequest = GoogleDriveFiles.Create(newFile);

            createRequest.Send();
            result = "Made new PaperVR folder";
            yield break;
        }

        if (requestList.ResponseData.Files.Count > 1)
            Debug.LogWarning($"Multiple '{filePath}' files been found.");

        var file = requestList.ResponseData.Files[0];
        //print(requestList.ResponseData.Files.Count);

        result = $"ID: {file.Id} Size: {file.Size * .000001f:0.00}MB Modified: {file.CreatedTime:dd.MM.yyyy HH:MM:ss}";

        requestList = new GoogleDriveFiles.ListRequest();
        requestList.Fields = new List<string> { "files(name, id, size, modifiedTime)" };
        requestList.Q = $"'{file.Id}' in parents";
        yield return requestList.Send();

        UnityGoogleDrive.Data.File file1 = requestList.ResponseData.Files[0];
        var file2 = requestList.ResponseData.Files[1];
        print(requestList.ResponseData.Files.Count);
        print(file1.Name);
        print(file2.Id);
    }

    private static string GetBeforeLast(string content, string matchString)
    {
        if (content.Contains(matchString))
        {
            var endIndex = content.LastIndexOf(matchString, StringComparison.Ordinal);
            return content.Substring(0, endIndex);
        }
        return null;
    }

    private static string GetAfter(string content, string matchString)
    {
        if (content.Contains(matchString))
        {
            var startIndex = content.LastIndexOf(matchString, StringComparison.Ordinal) + matchString.Length;
            if (content.Length <= startIndex) return string.Empty;
            return content.Substring(startIndex);
        }
        return null;
    }
}
