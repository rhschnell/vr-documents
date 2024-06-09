using System.Collections;
using System.Collections.Generic;
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
    /// The menu
    /// </summary>
    public GameObject menu;

    /// <summary>
    /// The button to activate the split menu
    /// </summary>
    public GameObject splitMenuButton;

    /// <summary>
    /// The split menu
    /// </summary>
    public GameObject splitMenu;

    /// <summary>
    /// The PDFCanvas
    /// </summary>
    public GameObject pdfCanvas;

    /// <summary>
    /// The subpanel
    /// </summary>
    public GameObject subPanel;

    /// <summary>
    /// The subsection panel for duplicating
    /// </summary>
    public GameObject duplicateSubsectionPanel;

    /// <summary>
    /// Whether there is hovered.
    /// </summary>
    public bool hover;

    /// <summary>
    /// A method that turns the over variable on when hovered.
    /// </summary>
    /// <param name="hover">The hover boolean.</param>
    public void OnHover(bool hover) {
        this.hover = hover;
    }

    /// <summary>
    /// The function calls the toggle menu.
    /// </summary>
    /// <param name="con">The action needed for when action called.</param>
    public void OpenAndCloseMenu(InputAction.CallbackContext con)
    {
        this.ToggleMenu();
    }

    /// <summary>
    /// This method toggles the menu on and off.
    /// </summary>
    public void ToggleMenu()
    {
        if (this.hover)
        {
            var floatingDocument = this.pdfCanvas.GetComponent<FloatingDocument>();
            if (floatingDocument != null)
            {
                this.splitMenuButton.SetActive(floatingDocument.pages.Count > 1);
                this.subPanel.SetActive(false);
                this.duplicateSubsectionPanel.SetActive(false);
            }
            else
            {
                this.menu.SetActive(!this.menu.activeSelf);
            }

            this.menu.SetActive(!this.menu.activeSelf);
            if (!this.menu.activeSelf)
            {
                this.splitMenu.SetActive(false);
            }
        }
    }

    private void Awake()
    {
        this.inputActionReference.action.started += this.OpenAndCloseMenu;
    }

    private void OnDestroy()
    {
        this.inputActionReference.action.started -= this.OpenAndCloseMenu;
    }
}
