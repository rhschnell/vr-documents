using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// Class that deals with importing the list of PDFs from the selected environment.
/// </summary>
public class ImportList : MonoBehaviour
{
    /// <summary>
    /// The dropdown containing all environments.
    /// </summary>
    public TMP_Dropdown dropdown;

    /// <summary>
    /// The list of pdfs in this folder
    /// </summary>
    public List<File> PDFs;

    /// <summary>
    /// the list request for the environment
    /// </summary>
    public GoogleDriveFiles.ListRequest envReq;

    /// <summary>
    /// finds all folders under the main folder and lists their names
    /// </summary>
    public void UpdateList()
    {
        List<string> environmentNames = new List<string>();
        foreach (var file in this.PDFs)
        {
            environmentNames.Add(file.Name.Replace(".pdf", ""));
        }

        this.dropdown.ClearOptions();
        this.dropdown.AddOptions(environmentNames);

        // Find the game object called GameManeger
        GameObject gameManager = GameObject.Find("GameManager");

        // Get the GameManager component with the environment script
        EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();

        // Set the import list
        environment.SetImportList(environmentNames);
    }

    /// <summary>
    /// finds all PDF files inside a selected folder
    /// </summary>
    /// <returns>waits for the request</returns>
    public IEnumerator FindPDF()
    {
        string parentId = SelectEnvironment.parentId;
        this.envReq.Fields = new List<string> { "files(id, name)" };
        this.envReq.Q = $"'{parentId}' in parents and name contains '.pdf' and trashed = false";
        yield return this.envReq.Send();
        if (!this.envReq.IsError)
        {
            this.PDFs = this.envReq.ResponseData.Files;

            this.UpdateList();
        }
    }

    void Start()
    {
        if (SelectEnvironment.parentId != "" && SelectEnvironment.parentId != null)
        {
            this.envReq = new GoogleDriveFiles.ListRequest();
            this.StartCoroutine(this.FindPDF());
        }
    }
}
