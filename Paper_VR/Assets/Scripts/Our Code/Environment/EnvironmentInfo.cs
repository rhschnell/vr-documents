using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// The class containing all information of the environment and
/// makes it such that th game manager will not be destroyed on load.
/// </summary>
public class EnvironmentInfo
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
    /// Initializes a new instance of the <see cref="EnvironmentInfo"/> class.
    /// The default constructor for the EnvironmentInfo class.
    /// </summary>
    public EnvironmentInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnvironmentInfo"/> class.
    /// </summary>
    /// <param name="environmentInformation">
    /// The environment information to initialize the class with.</param>
    public EnvironmentInfo(EnvironmentInformation environmentInformation)
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
    public static EnvironmentInfo LoadFromJson(string json)
    {
        // Deserialize the json string to an object.
        EnvironmentInfo scene = JsonConvert.DeserializeObject<EnvironmentInfo>(json);

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
