using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

public class ImportList : MonoBehaviour
{
    /// <summary>
    /// The dropdown containing all environments.
    /// </summary>
    public TMP_Dropdown dropdown;

    public List<File> PDFs;

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
    }

    /// <summary>
    /// finds all PDF files inside a selected folder
    /// </summary>
    /// <returns>waits for the request</returns>
    public IEnumerator FindPDF()
    {
        // string selectedEnvironmentName = this.dropdown.options[selectedIndex].text;

        string parentId = SelectEnvironment.parentId;
        envReq.Fields = new List<string> { "files(id, name)" };
        envReq.Q = $"'{parentId}' in parents and name contains '.pdf' and trashed = false";
        yield return envReq.Send();
        if (!envReq.IsError)
        {
            this.PDFs = envReq.ResponseData.Files;

            this.UpdateList();
        }
    }

    void Start()
    {
        if (SelectEnvironment.parentId != "" && SelectEnvironment.parentId != null)
        {
            print("parent = " + SelectEnvironment.parentId);
            envReq = new GoogleDriveFiles.ListRequest();
            this.StartCoroutine(this.FindPDF());
        }
    }
}
