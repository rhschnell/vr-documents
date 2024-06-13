using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The testing class of the DocumentMenu class.
/// </summary>
public class DocumentMenu : MonoBehaviour
{
    /// <summary>
    /// The input needed for activating the menu.
    /// </summary>
    public InputActionReference inputActionReference;

    /// <summary>
    /// The menu.
    /// </summary>
    public GameObject menu;

    /// <summary>
    /// The subsection menu.
    /// </summary>
    public GameObject subsectionMenu;

    /// <summary>
    /// The merge menu.
    /// </summary>
    public GameObject mergeMenu;

    /// <summary>
    /// The PDFCanvas.
    /// </summary>
    public GameObject pdfCanvas;

    /// <summary>
    /// Whether there is hovered.
    /// </summary>
    public bool hover;

    /// <summary>
    /// A method that turns the over variable on when hovered.
    /// </summary>
    /// <param name="hover">The hover boolean.</param>
    public void OnHover(bool hover) {
        // Sets the hover boolean
        this.hover = hover;
    }

    /// <summary>
    /// The function calls the toggle menu.
    /// </summary>
    /// <param name="con">The action needed for when action called.</param>
    public void OpenAndCloseMenu(InputAction.CallbackContext con)
    {
        // Calls the ToggleMenu function to open the menu
        this.ToggleMenu();
    }

    /// <summary>
    /// This method toggles the menu on and off.
    /// </summary>
    public void ToggleMenu()
    {
        if (this.hover)
        {
            var gameManager = GameObject.Find("GameManager");
            var list = gameManager.GetComponent<EnvironmentInformation>().GetFloatingDocuments();
            foreach (FloatingDocument t in list)
            {
                t.mergeButton.SetActive(false);
            }

            this.menu.SetActive(!this.menu.activeSelf);

            // If the menu is closed, the subsection menu should also close
            if (!this.menu.activeSelf)
            {
                this.mergeMenu.SetActive(false);
                this.subsectionMenu.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Registers event handler for input actions.
    /// </summary>
    private void Awake()
    {
        // Add the OpenAndCloseMenu method to the input action's started event
        this.inputActionReference.action.started += this.OpenAndCloseMenu;
    }

    /// <summary>
    /// Unregisters event handler for input actions.
    /// </summary>
    private void OnDestroy()
    {
        // Remove the OpenAndCloseMenu method from the input action's started event
        this.inputActionReference.action.started -= this.OpenAndCloseMenu;
    }
}
