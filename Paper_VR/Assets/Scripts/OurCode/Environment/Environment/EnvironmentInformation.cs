using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// The class containing all information of the environment and
/// makes it such that th game manager will not be destroyed on load.
/// </summary>
public class EnvironmentInformation
{
    /// <summary>
    /// The name of the environment.
    /// </summary>
    public string environmentName;

    /// <summary>
    /// The list of floating documents in the environment.
    /// </summary>
    public List<FloatingDocumentInfo> floatingDocuments = new List<FloatingDocumentInfo>();

    /// <summary>
    /// The list of all pdfs that it can import.
    /// </summary>
    public List<string> importList;

    /// <summary>
    /// The background color of the environment.
    /// </summary>
    public Color backgroundColor;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentInformation"/> class.
    /// The default constructor for the EnvironmentInformation class.
    /// </summary>
    public EnvironmentInformation()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentInformation"/> class.
    /// </summary>
    /// <param name="environmentInformation">
    /// The environment information to initialize the class with.</param>
    public EnvironmentInformation(EnvironmentController environmentInformation)
    {
        // Set the fields of the class to the fields of the environment information.
        this.environmentName = environmentInformation.GetName();
        this.floatingDocuments = new List<FloatingDocumentInfo>();
        foreach (FloatingDocument floatingDocument in environmentInformation.GetFloatingDocuments())
        {
            this.floatingDocuments.Add(new FloatingDocumentInfo(floatingDocument));
        }

        this.importList = environmentInformation.GetImportList();
        this.backgroundColor = environmentInformation.GetBackgroundColor();
    }

    /// <summary>
    /// Loads the environment information from a json file.
    /// </summary>
    /// <param name="json">The json string to load the environment information from.</param>
    /// <returns>
    /// The environment information loaded from the json file.
    /// </returns>
    public static EnvironmentInformation LoadFromJson(string json)
    {
        // Deserialize the json string to an object.
        EnvironmentInformation scene = JsonConvert.DeserializeObject<EnvironmentInformation>(json);

        // Return the environment information.
        return scene;
    }

    /// <summary>
    /// Saves the environment information to a json file.
    /// </summary>
    /// <param name="obj">The environment information to save.</param>
    /// <returns>The json string of the environment information.</returns>
    public string SaveToJson()
    {
        // Serialize the object to a json string.
        string json = JsonConvert.SerializeObject(this);
        return json;
    }
}
