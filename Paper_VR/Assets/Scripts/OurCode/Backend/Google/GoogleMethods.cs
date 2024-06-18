using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGoogleDrive;

/// <summary>
/// Class that handles the login of Google.
/// </summary>
public class GoogleMethods
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
    /// The current id that the findId method sets.
    /// </summary>
    public string currentEnvId;

    /// <summary>
    /// The returned file.
    /// </summary>
    public UnityGoogleDrive.Data.File returnFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleMethods"/> class.
    /// </summary>
    public GoogleMethods()
    {
        folderID = GoogleLogin.folderID;
    }

    /// <summary>
    /// Finds the ID of the folder with the name of the enviorment, You can retrieve the value in currentEnvId.
    /// </summary>
    /// <param name="folder">The folder to find.</param>
    /// <param name="requestList">The requestlist to use.</param>
    /// <returns>Waits for things to be done until it continues.</returns>
    public virtual IEnumerator FindEnviormentId(string folder, GoogleDriveFiles.ListRequest requestList)
    {
        // Find the folder id of the folder with the name of the environment
        requestList.Fields = new List<string> { "files(id)" };
        requestList.Q = $"'{folderID}' in parents and name = '{folder}' and trashed = false";
        yield return requestList.Send();

        if (requestList.ResponseData.Files.Count == 0)
        {
            this.currentEnvId = "no folder";
            Debug.Log("No folder found");
            Debug.Log(folderID);
            Debug.Log(folder);
        }
        else
        {
            this.currentEnvId = requestList.ResponseData.Files[0].Id;
            Debug.Log(this.currentEnvId);
        }
    }

    /// <summary>
    /// This method deletes a file with the given name and parent ID.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="parentId">The ID of the environment.</param>
    /// <param name="requestList">The request list used for mocking.</param>
    /// <param name="testing">If the method is being tested.</param>
    /// <returns>IEnumerator.</returns>
    public virtual IEnumerator DeleteFile(string fileName, string parentId, GoogleDriveFiles.ListRequest requestList, bool testing)
    {
        // Define the fields to retrieve and the query to find the file by name and parent ID
        requestList.Fields = new List<string> { "files(id)" };
        requestList.Q = $"'{parentId}' in parents and name = '{fileName}' and trashed = false";
        yield return requestList.Send();

        // Check if any files are found with the specified name and parent ID
        if (requestList.ResponseData.Files.Count == 0)
        {
            Debug.Log("No file found");
        }
        else
        {
            // Iterate through each file found
            foreach (var fileToDelete in requestList.ResponseData.Files)
            {
                string fileId = fileToDelete.Id;

                // Create a delete request for the file
                GoogleDriveFiles.DeleteRequest deleteRequest = GoogleDriveFiles.Delete(fileId);
                if (!testing)
                {
                    yield return deleteRequest.Send();
                }
            }
        }
    }

    /// <summary>
    /// This method creates a json file with the given name and content.
    /// This json file is then sent to Google Drive.
    /// This is used for saving the environment.
    /// </summary>
    /// <param name="folderId">The id of the folder of the environment.</param>
    /// <param name="content">The content of the file.</param>
    /// <param name="request">The request to use for mocking.</param>
    /// <param name="testing">Whether the method is being tested.</param>
    /// <returns>IEnumerator.</returns>
    public virtual IEnumerator CreateJsonFile(string folderId, byte[] content, GoogleDriveFiles.CreateRequest request, bool testing)
    {
        // Create a new file object
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = "Environment.json", Content = content, MimeType = "application/json" };
        newFile.Parents = new List<string> { folderId };

        // If the method is not being tested, create a new request
        if (!testing)
        {
            request = GoogleDriveFiles.Create(newFile);
        }

        request.Fields = new List<string> { "id" };

        yield return request.Send();
    }

    /// <summary>
    /// Downloads a file using its ID.
    /// </summary>
    /// <param name="id">The ID of the file.</param>
    /// <returns>The downloaded file.</returns>
    public virtual IEnumerator DownloadFile(string id)
    {
        // Create a download request for the specified file ID
        GoogleDriveFiles.DownloadRequest req = new GoogleDriveFiles.DownloadRequest(id);

        // Send the request and wait for it to complete, then store it
        yield return req.Send();
        this.returnFile = req.ResponseData;
    }

    /// <summary>
    /// Makes a request to create a new environment folder.
    /// </summary>
    /// <param name="name">The name of the new folder.</param>
    /// <returns>A new create file request.</returns>
    public virtual GoogleDriveFiles.CreateRequest MakeRequest(string name)
    {
        // Create a new folder with the name of the new environment
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = name, MimeType = "application/vnd.google-apps.folder" };
        newFile.Parents = new List<string> { GoogleLogin.folderID };
        return GoogleDriveFiles.Create(newFile);
    }
}
