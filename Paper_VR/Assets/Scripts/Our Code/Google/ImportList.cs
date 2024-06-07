using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// Class that deals with importing the list of PDFs from the selected environment.
/// </summary>
public class ImportList : MonoBehaviour
{
    /// <summary>
    /// The transform of the character.
    /// </summary>
    public Transform characterTransform;

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
    /// The basic prefab of a standard pdf.
    /// </summary>
    public GameObject pdfPrefab;

    /// <summary>
    /// The pdf converter object.
    /// This object converts the pdf to sprites.
    /// and creates a floating document.
    /// </summary>
    public BackendPDF pdfConverter;

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

    /// <summary>
    /// Send a download request with the content to ImportPDF.
    /// </summary>
    public void Download()
    {
        string name = this.PDFs[this.dropdown.value].Name;
        GoogleDriveFiles.DownloadRequest req = new GoogleDriveFiles.DownloadRequest(this.PDFs[this.dropdown.value].Id);
        req.Send().OnDone += (UnityGoogleDrive.Data.File file) => this.StartCoroutine(this.ImportPDF(file, name));
    }

    /// <summary>
    /// Convert a pdf and set the image to the first page sprite.
    /// </summary>
    /// <param name="file">The file containing the conten of the pdf.</param>
    /// <param name="name">The name of the pdf.</param>
    //public void ConvertPdf(UnityGoogleDrive.Data.File file, string name)
    //{
     //   // Find the file
     //   string name = this.PDFs[this.dropdown.value].Name;
     //   GoogleDriveFiles.DownloadRequest req = new GoogleDriveFiles.DownloadRequest(this.PDFs[this.dropdown.value].Id);
     //   req.Send().OnDone += (UnityGoogleDrive.Data.File file) => this.StartCoroutine(this.ImportPDF(file, name));
    //}

    /// <summary>
    /// Imports the pdf.
    /// </summary>
    /// <param name="file">The file we want to import.</param>
    /// <param name="name">The name of the file.</param>
    /// <returns>Returns the IEnumerator.</returns>
    public IEnumerator ImportPDF(UnityGoogleDrive.Data.File file, string name)
    {
        if (file != null)
        {
            // Create a new floating document
            float spawnDistance = 2.0f;
            Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 spawnPosition = this.characterTransform.position + (this.characterTransform.forward * spawnDistance) + offset;
            Vector3 direction = spawnPosition - this.characterTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            GameObject pdf = Instantiate(this.pdfPrefab, spawnPosition, lookRotation);
            FloatingDocument script = pdf.GetComponent<FloatingDocument>();
            script.pdfName = name;
            script.pdfId = file.Id;

            // Find the game object called GameManeger
            GameObject gameManager = GameObject.Find("GameManager");

            // Get the GameManager component with the environment script
            EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();

            environment.GetFloatingDocuments().Add(script);

            // Convert the pdf to sprites and set the first page of the floating document to the first sprite
            yield return this.pdfConverter.AddImagesToFloatingDocument(file, script);
        }
        else
        {
            // If the file is null, throw an error
            Debug.LogError("File is null");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SelectEnvironment.parentId != "" && SelectEnvironment.parentId != null)
        {
            this.envReq = new GoogleDriveFiles.ListRequest();
            this.StartCoroutine(this.FindPDF());
        }
    }
}