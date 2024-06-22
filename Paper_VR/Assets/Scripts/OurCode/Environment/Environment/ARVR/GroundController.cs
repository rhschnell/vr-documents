using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles toggling the visibility of the ground on or off.
/// </summary>
public class GroundController : MonoBehaviour
{
    /// <summary>
    /// The ground that the player stands on.
    /// </summary>
    public GameObject[] ground;

    /// <summary>
    /// The left controller that the player uses to interact with the environment.
    /// </summary>
    public GameObject leftController;

    /// <summary>
    /// The right controller that the player uses to interact with the environment.
    /// </summary>
    public GameObject rightController;

    /// <summary>
    /// The button which switches the ground on or off.
    /// </summary>
    public Button switchGroundButton;

    /// <summary>
    /// The text which contains if the ground is showing or not.
    /// </summary>
    public TMP_Text switchGroundText;

    /// <summary>
    /// The sprite containing the off-button.
    /// </summary>
    public Sprite offButtonSprite;

    /// <summary>
    /// The sprite containing the on-button.
    /// </summary>
    public Sprite onButtonSprite;

    /// <summary>
    /// Class containing the current visibility state of the ground.
    /// </summary>
    public ARstatus arStatus;

    /// <summary>
    /// Turns the ground either on or off, depending on the current state of it.
    /// </summary>
    public void OnSwitchClick()
    {
        this.arStatus.isAR = !this.arStatus.isAR;

        // Switch the visibility of the ground
        this.SetGroundVisibility(!this.arStatus.isAR);

        // Update the sprite and text of the button
        this.UpdateButtonAndTextState();
    }

    /// <summary>
    /// Toggles the visibility of the ground platform
    /// </summary>
    /// <param name="isVisible">Whether or not the ground should be visible</param>
    private void SetGroundVisibility(bool isVisible)
    {
        // Loop through all the ground objects and set them to disabled
        foreach (GameObject groundObject in this.ground)
        {
            Renderer groundRenderer = groundObject.GetComponent<Renderer>();
            if (groundRenderer != null)
            {
                groundRenderer.enabled = isVisible;
            }
        }

        // Get the child transform of the left and right controllers
        GameObject leftChild = this.leftController.transform.GetChild(0).gameObject;
        GameObject rightChild = this.rightController.transform.GetChild(0).gameObject;

        // Set the controllers to be visible if the ground is visible
        // Otherwise, set them to be invisible
        leftChild.SetActive(isVisible);
        rightChild.SetActive(isVisible);

        // Change the background color of the camera
        if (isVisible)
        {
            Camera.main.clearFlags = CameraClearFlags.Skybox;
            Camera.main.backgroundColor = new Color(7f / 255f, 89f / 255f, 85f / 255f, 1f);
        }
        else
        {
            Camera.main.clearFlags = CameraClearFlags.SolidColor;
            Camera.main.backgroundColor = new Color(0, 0, 0, 0);
        }
    }

    /// <summary>
    /// Updates the button sprite based on whether the ground is visible or not.
    /// </summary>
    private void UpdateButtonAndTextState()
    {
        // Set the sprite and text to off if the ground is off, vice versa otherwise
        if (!this.arStatus.isAR)
        {
            this.switchGroundButton.image.sprite = this.onButtonSprite;
            this.switchGroundText.text = "Ground ON (VR)";
        }
        else
        {
            this.switchGroundButton.image.sprite = this.offButtonSprite;
            this.switchGroundText.text = "Ground OFF (AR)";
        }
    }

    /// <summary>
    /// Called on startup, sets the ground to be visible and updates the button state.
    /// </summary>
    void Start()
    {
        // Get the AR status
        this.arStatus = GameObject.FindObjectOfType<ARstatus>();

        // Set the ground to be visible
        this.SetGroundVisibility(!this.arStatus.isAR);

        // Update the state of the switch button
        this.UpdateButtonAndTextState();
    }
}