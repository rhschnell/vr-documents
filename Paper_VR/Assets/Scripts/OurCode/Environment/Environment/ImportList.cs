using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    /// The button which can be clicked to import the selected document.
    /// </summary>
    public Button importButton;

    /// <summary>
    /// The list of pdfs in this folder.
    /// </summary>
    public List<File> PDFs;

    /// <summary>
    /// the list request for the environment.
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
    /// Finds all folders under the main folder and lists their names.
    /// </summary>
    public void UpdateList()
    {
        List<string> environmentNames = new List<string>();
        foreach (var file in this.PDFs)
        {
            environmentNames.Add(file.Name.Replace(".pdf", ""));
        }

        // Add the found environments to the dropdown
        this.dropdown.ClearOptions();
        this.dropdown.AddOptions(environmentNames);

        // Find the game object called GameManeger
        GameObject gameManager = GameObject.Find("GameManager");

        // Get the GameManager component with the environment script
        EnvironmentController environment = gameManager.GetComponent<EnvironmentController>();

        // Set the import list
        environment.SetImportList(environmentNames);
    }

    /// <summary>
    /// Finds all PDF files inside a selected folder.
    /// </summary>
    /// <returns>Waits for the request.</returns>
    public IEnumerator FindPDF()
    {
        // Define the fields to retrieve
        string parentId = SelectEnvironment.parentId;
        this.envReq.Fields = new List<string> { "files(id, name)" };
        this.envReq.Q = $"'{parentId}' in parents and name contains '.pdf' and trashed = false";

        yield return this.envReq.Send();

        // If no error is thrown, add pdf files to the list
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

        // Create a download request for the selected PDF using its unique ID
        GoogleDriveFiles.DownloadRequest req = new GoogleDriveFiles.DownloadRequest(this.PDFs[this.dropdown.value].Id);

        // Send the download request and set up a callback to handle the completion of the request
        req.Send().OnDone += (UnityGoogleDrive.Data.File file) => this.StartCoroutine(this.ImportPDF(file, name));

        // Make the import button not interactable to prevent spam clicking
        this.StartCoroutine(this.DisableImportButton());
    }

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
            // Calculate the position of the new pdf
            float spawnDistance = 2.0f;
            Vector3 offset = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 spawnPosition = this.characterTransform.position + (this.characterTransform.forward * spawnDistance) + offset;
            Vector3 direction = spawnPosition - this.characterTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Instantiate a new PDFCanvas in the scene
            GameObject pdf = Instantiate(this.pdfPrefab, spawnPosition, lookRotation);
            FloatingDocument script = pdf.GetComponent<FloatingDocument>();

            // Set fields of the floating document
            script.pdfName = name;
            script.pdfId = file.Id;
            script.exportPages = new List<Tuple<string, string, int>>();

            // Find the game object called GameManeger
            GameObject gameManager = GameObject.Find("GameManager");

            // Get the GameManager component with the environment script
            EnvironmentController environment = gameManager.GetComponent<EnvironmentController>();

            environment.GetFloatingDocuments().Add(script);

            // Convert the pdf to sprites and set the first page of the floating document to the first sprite
            yield return this.pdfConverter.AddImagesToFloatingDocument(file.Content, script);
        }
        else
        {
            // If the file is null, throw an error
            Debug.LogError("File is null");
        }
    }

    /// <summary>
    /// Return to the Environment Menu
    /// </summary>
    public void OnClickReturn()
    {
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Iterate through all GameObjects and destroy them
        foreach (GameObject obj in allObjects)
        {
            Destroy(obj);
        }

        SceneManager.LoadSceneAsync("EnviromentMenu");
    }

    /// <summary>
    /// Makes the import button not interactable for two seconds.
    /// </summary>
    /// <returns>An IEnumerator.</returns>
    private IEnumerator DisableImportButton()
    {
        // Make the import button not interactable
        this.importButton.interactable = false;

        // Wait for one second
        yield return new WaitForSeconds(2.0f);

        // Make the import button interactable again
        this.importButton.interactable = true;
    }

    /// <summary>
    /// Initializes the import list by finding all pdfs in the environment.
    /// </summary>
    void Start()
    {
        if (SelectEnvironment.parentId != "" && SelectEnvironment.parentId != null)
        {
            this.envReq = new GoogleDriveFiles.ListRequest();

            // Find all pdfs in the current environment
            this.StartCoroutine(this.FindPDF());
        }
    }
}