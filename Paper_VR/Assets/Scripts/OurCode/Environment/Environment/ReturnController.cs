using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class contains the logic for the return button.
/// </summary>
public class ReturnController : MonoBehaviour
{
    /// <summary>
    /// Boolean used for testing.
    /// </summary>
    public bool testing = false;

    /// <summary>
    /// Return to the Environment Menu.
    /// </summary>
    public void OnClickReturn()
    {
        // Destroy the GameManager object
        GameObject.DestroyImmediate(GameObject.Find("GameManager"));

        // Return to the environment menu, if were not in testing mode
        if (!this.testing)
        {
            SceneManager.LoadSceneAsync("EnviromentMenu");
        }
    }
}