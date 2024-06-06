using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains the logic for the save button.
/// </summary>
public class SaveButton : MonoBehaviour
{
    /// <summary>
    /// The time between saves.
    /// </summary>
    public float savingTime = 45f;

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

    /// <summary>
    /// This method saves when the application is quit.
    /// </summary>
    public void OnApplicationQuit()
    {
        this.Save();
    }

    /// <summary>
    /// Starts the code by calling the SaveEveryWhile routine.
    /// </summary>
    public void Start()
    {
        this.StartCoroutine(this.SaveEveryWhile());
    }

    IEnumerator SaveEveryWhile()
    {
        yield return new WaitForSeconds(this.savingTime);
        this.Save();

        this.StartCoroutine(this.SaveEveryWhile());
        // NEW CODE
    }
}
