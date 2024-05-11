using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class adds environments to documents
/// </summary>
public class AddEnvironment : MonoBehaviour
{
    /// <summary>
    /// The input field where the environment name should be inserted.
    /// </summary>
    public TMP_InputField InputField;

    /// <summary>
    /// Adds an environment
    /// </summary>
    public void AddEnvironmentButton()
    {
        string environmentName = InputField.text;
        // (environmentName == "") 

    }
}
