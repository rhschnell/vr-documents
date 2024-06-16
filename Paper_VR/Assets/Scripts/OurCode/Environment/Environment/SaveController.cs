using System.Collections;
using UnityEngine;

/// <summary>
/// This class contains the logic for the save button.
/// </summary>
public class SaveController : MonoBehaviour
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
        EnvironmentController environment = gameManager.GetComponent<EnvironmentController>();

        // Save the environment information
        this.StartCoroutine(environment.SaveEnvironment(new GoogleMethods()));
    }

    /// <summary>
    /// This method saves when the application is quit.
    /// </summary>
    public void OnApplicationQuit()
    {
        // Save the environment
        this.Save();
    }

    /// <summary>
    /// Starts the code by calling the SaveEveryWhile routine.
    /// </summary>
    public void Start()
    {
        // Cal lthe SaveEveryWhile method to enable autosave
        this.StartCoroutine(this.SaveEveryWhile());
    }

    /// <summary>
    /// Saves the environment every 45 seconds (autosave).
    /// </summary>
    /// <returns>The IEnumerator</returns>
    IEnumerator SaveEveryWhile()
    {
        // Wait 45 seconds before saving the environment
        yield return new WaitForSeconds(this.savingTime);
        this.Save();

        // Makes a call to itself
        this.StartCoroutine(this.SaveEveryWhile());
    }
}
