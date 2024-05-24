using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The class containing all information of the environment and
/// makes it such that the game manager will not be destroyed on load.
/// </summary>
public class EnvironmentInformation : MonoBehaviour
{
    /// <summary>
    /// This is the prefab for all the floating documents.
    /// </summary>
    public GameObject docPrefab;

    /// <summary>
    /// This is the path to the images.
    /// </summary>
    private static string imagePath = "Sprites";

    private string environmentName;
    private List<FloatingDocument> floatingDocuments;
    private List<string> importList;
    private Color backgroundColor;

    /// <summary>
    /// This method creates a new floating document in the scene.
    /// </summary>
    /// <param name="pos">The position of the floating document</param>
    /// <param name="rotation">The rotation of the floating document</param>
    /// <param name="scale">The scale of the floating document</param>
    /// <param name="pdfPath">The path where the pdf is stored</param>
    /// <param name="pdfName">The name of the pdf</param>
    /// <param name="pages">The pages as a list of numbers</param>
    /// <returns>A floating document</returns>
    public FloatingDocument CreateDocument(Vector3 pos, Quaternion rotation, Vector3 scale, string pdfPath, string pdfName, int[] pages)
    {
        Debug.Log("Creating document");
        // Create a new canvas
        GameObject instance = Instantiate(this.docPrefab, pos, rotation);
        instance.transform.localScale = scale;

        // Update the child of the prefab to have to the correct first image
        string spritePath = imagePath + "\\" + pdfName + "\\" + pdfName + "-" + pages[0];
        var sprite = Resources.Load<Sprite>(spritePath);
        Debug.Log(sprite);
        instance.transform.GetChild(0).GetComponent<Image>().sprite = sprite;

        // Create a new floating document
        FloatingDocument doc = new FloatingDocument(instance, pdfPath, pdfName, pages);

        return doc;
    }

    /// <summary>
    /// Loads the new environment information on this instance of the class.
    /// </summary>
    /// <param name="newEnvironmentInformation">the information of the new environment.</param>
    public void LoadNewInformation(EnvironmentInfo newEnvironmentInformation)
    {
        // Sets the environment name to the new name.
        this.environmentName = newEnvironmentInformation.environmentName;

        // Sets the floating documents to the new floating documents.
        this.floatingDocuments = new List<FloatingDocument>();
        foreach (FloatingDocumentInfo floatingDocument in newEnvironmentInformation.floatingDocuments)
        {
            this.floatingDocuments.Add(
                this.CreateDocument(
                    floatingDocument.position,
                    floatingDocument.rotation,
                    floatingDocument.scale,
                    floatingDocument.pdfPath,
                    floatingDocument.pdfName,
                    floatingDocument.pages));
        }

        // Sets the import list to the new import list.
        this.importList = newEnvironmentInformation.importList;

        // Sets the beckground color to the new background color.
        this.backgroundColor = newEnvironmentInformation.backgroundColor;
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
    private void Awake()
    {
        // Destroys this game object when a new scene is loaded.
        DontDestroyOnLoad(this.gameObject);
    }
}
