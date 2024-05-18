using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is resposible for loading in the scene, using the environment information.
/// </summary>
public class LoadEnvironmentInformation : MonoBehaviour
{
    /// <summary>
    /// This method subscribes to the sceneLoaded event at the start
    /// </summary>
    private void Start()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }

    /// <summary>
    /// This method unsubscribes to the sceneLoaded event when destroyed
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        this.Unsubscribe();
    }

    private void Unsubscribe()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= this.OnSceneLoaded;
    }

    /// <summary>
    /// When a new scene is loaded, the environment information is fetched and the scene will be loaded.
    /// </summary>
    /// <param name="scene">The scene that is loaded.</param>
    /// <param name="mode">The load mode of the scene.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the scene loaded is the environment, laod the scene
        if (scene.name.Equals("Environment"))
        {
            // Get the environment information from the selected scene
            EnvironmentInformation environmentInformation = this.gameObject.GetComponent<EnvironmentInformation>();
        }
    }
}
