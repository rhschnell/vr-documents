using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class containing all information of the environment and
/// makes it such that th game manager will not be destroyed on load.
/// </summary>
public class EnvironmentInformation : MonoBehaviour
{
    private string environmentName;
    private List<FloatingDocument> floatingDocuments = new List<FloatingDocument>();
    private List<string> importList;
    private Color backgroundColor;

    /// <summary>
    /// Loads the new environment information on this instance of the class.
    /// </summary>
    /// <param name="newEnvironmentInformation">the information of the new environment.</param>
    public void LoadNewInformation(EnvironmentInformation newEnvironmentInformation)
    {
        // Sets the environment name to the new name.
        this.environmentName = newEnvironmentInformation.GetName();

        // Sets the floating documents to the new floating documents.
        this.floatingDocuments = newEnvironmentInformation.GetFloatingDocuments();

        // Sets the import list to the new import list.
        this.importList = newEnvironmentInformation.GetImportList();

        // Sets the beckground color to the new background color.
        this.backgroundColor = newEnvironmentInformation.GetBackgroundColor();
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
