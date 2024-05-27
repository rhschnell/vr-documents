using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains the logic for the save button.
/// </summary>
public class SaveButton : MonoBehaviour
{
    /// <summary>
    /// This method saves the environment information when the save button is clicked.
    /// </summary>
    public void Save()
    {
        // Find the game object called GameManeger
        GameObject gameManager = GameObject.Find("GameManager");

        // Get the GameManager component with the environment script
        EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();

        // Save the environment information
        this.StartCoroutine(environment.SaveEnvironment(new GoogleMethods()));
    }
}
