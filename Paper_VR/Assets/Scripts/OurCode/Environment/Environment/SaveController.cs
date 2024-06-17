using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    /// The button for saving.
    /// </summary>
    public Button saveButton;

    /// <summary>
    /// The TMP_Text containing the save environment text.
    /// </summary>
    public TMP_Text saveMenuName;

    /// <summary>
    /// The TMP_Text containing the confirmation message shown when saving.
    /// </summary>
    public TMP_Text saveMenuConfirmation;

    /// <summary>
    /// An instance of GoogleMethods used for testing.
    /// </summary>
    public GoogleMethods googleMethods;

    /// <summary>
    /// This method saves the environment information when the save button is clicked.
    /// </summary>
    public void Save()
    {
        // Find the game object called GameManager
        GameObject gameManager = GameObject.Find("GameManager");

        // Get the GameManager component with the environment script
        EnvironmentController environment = gameManager.GetComponent<EnvironmentController>();

        // Save the environment information
        this.StartCoroutine(environment.SaveEnvironment(this.googleMethods));
    }

    /// <summary>
    /// Saves the environment when the user clicks the button.
    /// </summary>
    public void OnSaveClick()
    {
        // Save the environment
        this.Save();

        // Toggle the visibility of the save button to prevent spam clicking
        this.StartCoroutine(this.ToggleConfirmation());
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
        // Call the SaveEveryWhile method to enable autosave
        this.StartCoroutine(this.SaveEveryWhile());
        this.googleMethods = new GoogleMethods();
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
        this.StartCoroutine(this.SaveEveryWhile());
    }

    /// <summary>
    /// Shows the confirmation message for 5 seconds to prevent spam clicking.
    /// </summary>
    /// <returns>The IEnumerator.</returns>
    IEnumerator ToggleConfirmation()
    {
        // Show the confirmation message and set the button to inactive
        this.saveButton.interactable = false;
        this.saveMenuName.gameObject.SetActive(false);
        this.saveMenuConfirmation.gameObject.SetActive(true);

        // Wait for 5 seconds
        yield return new WaitForSeconds(5.0f);

        // Set the save button to be active and unshow the confirmation message
        this.saveButton.interactable = true;
        this.saveMenuName.gameObject.SetActive(true);
        this.saveMenuConfirmation.gameObject.SetActive(false);
    }
}