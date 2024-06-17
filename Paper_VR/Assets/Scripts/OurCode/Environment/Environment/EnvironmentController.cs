using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityGoogleDrive;

/// <summary>
/// The class containing all information of the environment and
/// makes it such that the game manager will not be destroyed on load.
/// </summary>
public class EnvironmentController : MonoBehaviour
{
    /// <summary>
    /// This is the id of the folder where documents are saved.
    /// </summary>
    public static string saveFolderId;

    /// <summary>
    /// This is the prefab for all the floating documents.
    /// </summary>
    public GameObject docPrefab;

    private string environmentName = "New Environment";
    private List<FloatingDocument> floatingDocuments = new List<FloatingDocument>();
    private List<string> importList = new List<string>();
    private Color backgroundColor = Color.white;

    /// <summary>
    /// Loads the new environment information on this instance of the class.
    /// </summary>
    /// <param name="newEnvironmentInformation">the information of the new environment.</param>
    public void LoadNewInformation(EnvironmentInformation newEnvironmentInformation)
    {
        // Sets the environment name to the new name.
        this.environmentName = newEnvironmentInformation.environmentName;

        // Sets the floating documents to the new floating documents.
        this.floatingDocuments = new List<FloatingDocument>();

        foreach (FloatingDocumentInfo f in newEnvironmentInformation.floatingDocuments)
        {
            // Creates a new GameObject
            GameObject instance = Instantiate(this.docPrefab, f.position, f.rotation);
            instance.transform.localScale = f.scale;
            FloatingDocument floatDoc = instance.AddComponent<FloatingDocument>();
            floatDoc.SetAttributes(
                f.position,
                f.rotation,
                f.scale,
                f.pdfId,
                f.pdfName,
                f.pages,
                f.exportPages);
            this.floatingDocuments.Add(floatDoc);
        }

        // Sets the import list to the new import list.
        this.importList = newEnvironmentInformation.importList;

        // Sets the background color to the new background color.
        this.backgroundColor = newEnvironmentInformation.backgroundColor;
    }

    /// <summary>
    /// This method saves the environment information to a json file.
    /// And sends it to google drive.
    /// </summary>
    /// <param name="googleMethods">The google methods object</param>
    /// <returns>An IEnumerator</returns>
    public virtual IEnumerator SaveEnvironment(GoogleMethods googleMethods)
    {
        // Find the folder id of the folder with the name of the environment.
        yield return googleMethods.FindEnviormentId(this.environmentName, new GoogleDriveFiles.ListRequest());
        string folderId = googleMethods.currentEnvId;

        // Create a new environment information object.
        EnvironmentInformation environmentInfo = new EnvironmentInformation(this);

        // Serialize the environment information object to a json string.
        string json = JsonConvert.SerializeObject(environmentInfo);

        // Create a new json file
        var content = Encoding.ASCII.GetBytes(json);

        // Delete the old environment file.
        yield return googleMethods.DeleteFile("Environment.json", folderId, new GoogleDriveFiles.ListRequest(), false);

        // Send the json file to google drive.
        GoogleDriveFiles.CreateRequest request = new GoogleDriveFiles.CreateRequest();

        yield return googleMethods.CreateJsonFile(folderId, content, request, false);
    }

    /// <summary>
    /// Returns the name of the environment.
    /// </summary>
    /// <returns>The name of the environment.</returns>
    public string GetName()
    {
        // Returns the name of the environment.
        return this.environmentName;
    }

    /// <summary>
    /// Sets the environment name to the new name.
    /// </summary>
    /// <param name="name">The new environment name.</param>
    public void SetName(string name)
    {
        // Sets the environment name to the new name.
        this.environmentName = name;
    }

    /// <summary>
    /// Returns the floating documents of the environment.
    /// </summary>
    /// <returns>The floating documents of the environment.</returns>
    public List<FloatingDocument> GetFloatingDocuments()
    {
        // Returns the floating documents of the environment.
        return this.floatingDocuments;
    }

    /// <summary>
    /// Sets the environment floating documents to the new floating documents.
    /// </summary>
    /// <param name="floatingDocuments">The new environment floating documents.</param>
    public void SetFloatingDocuments(List<FloatingDocument> floatingDocuments)
    {
        // Sets the environment floating documents to the new floating documents.
        this.floatingDocuments = floatingDocuments;
    }

    /// <summary>
    /// Returns the import list of the environment.
    /// </summary>
    /// <returns>The import list of the environment.</returns>
    public List<string> GetImportList()
    {
        // Returns the import list of the environment.
        return this.importList;
    }

    /// <summary>
    /// Sets the environment import list to the new import list.
    /// </summary>
    /// <param name="importList">The new environment import list.</param>
    public void SetImportList(List<string> importList)
    {
        // Sets the environment import list to the new import list.
        this.importList = importList;
    }

    /// <summary>
    /// Returns the background color of the environment.
    /// </summary>
    /// <returns>The background color of the environment.</returns>
    public Color GetBackgroundColor()
    {
        // Returns the background color of the environment.
        return this.backgroundColor;
    }

    /// <summary>
    /// Sets the environment background color to the new background color.
    /// </summary>
    /// <param name="backgroundColor">The new environment background color.</param>
    public void SetBackgroundColor(Color backgroundColor)
    {
        // Sets the environment background color to the new background color.
        this.backgroundColor = backgroundColor;
    }

    /// <summary>
    /// The awake method, it makes sure that the object will not be destroyed when loading a new scene.
    /// </summary>
    void Awake()
    {
        // Does not destroy this game object when a new scene is loaded.
        DontDestroyOnLoad(this.gameObject);
    }
}