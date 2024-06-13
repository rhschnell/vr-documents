using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that controls the logic of the subsection menu.
/// </summary>
public class MenuController : MonoBehaviour
{
    /// <summary>
    /// The canvas of the PDF containing the floating document.
    /// </summary>
    public GameObject pdfPrefab;

    /// <summary>
    /// The manipulation menu.
    /// </summary>
    public GameObject Menu;

    /// <summary>
    /// The splitting menu.
    /// </summary>
    public GameObject subsectionMenu;

    /// <summary>
    /// Text field containing the currently selected start page from which to split.
    /// </summary>
    public TMP_Text StartPageNumberText;

    /// <summary>
    /// Text field containing the currently selected end page at which to split.
    /// </summary>
    public TMP_Text EndPageNumberText;

    /// <summary>
    /// The plus button for increasing the start page number.
    /// </summary>
    public UnityEngine.UI.Button StartPlusButton;

    /// <summary>
    /// The minus button for decreasing the start page number.
    /// </summary>
    public UnityEngine.UI.Button StartMinusButton;

    /// <summary>
    /// THis is the start slider for changing the start page number.
    /// </summary>
    public UnityEngine.UI.Slider StartSlider;

    /// <summary>
    /// The plus button for increasing the end page number.
    /// </summary>
    public UnityEngine.UI.Button EndPlusButton;

    /// <summary>
    /// The minus button for decreasing the end page number.
    /// </summary>
    public UnityEngine.UI.Button EndMinusButton;

    /// <summary>
    /// The counter for the start page number.
    /// </summary>
    public UnityEngine.UI.Slider EndSlider;

    /// <summary>
    /// Gets or sets the start page number.
    /// </summary>
    public int StartPageNumber = 1;

    /// <summary>
    /// Gets or sets the end page number.
    /// </summary>
    public int EndPageNumber = 1;

    /// <summary>
    /// Gets or sets the max page number.
    /// </summary>
    public int MaxPage;

    /// <summary>
    /// This method sets up the listeners for the sliders.
    /// </summary>
    public void SetupSliderListeners()
    {
        // Set up the slider which indicates the start page
        if (this.StartSlider != null)
        {
            this.StartSlider.onValueChanged.AddListener((v) =>
            {
                // Ensure the StartPageNumber does not exceed the current value of EndPageNumber
                var value = v * (this.EndPageNumber - 1);
                this.StartPageNumber = Mathf.RoundToInt(value + 1);
                this.UpdateLabel();
            });
        }

        // Set up the slider which indicates the end page
        if (this.EndSlider != null)
        {
            this.EndSlider.onValueChanged.AddListener((v) =>
            {
                // Ensure the EndPageNumber does not exceed the MaxPage value and is not less than StartPageNumber
                var value = v * (this.MaxPage - this.StartPageNumber);
                this.EndPageNumber = Mathf.RoundToInt(value + this.StartPageNumber);
                this.UpdateLabel();
            });
        }
    }

    /// <summary>
    /// This method toggles the menu on and off.
    /// </summary>
    public void ToggleMenu()
    {
        // Toggles the subsection menu on or off, depending on its current state
        this.subsectionMenu.SetActive(!this.subsectionMenu.activeSelf);
        this.UpdateDocumentPageCount();
    }

    /// <summary>
    /// The action that is executed after clicking on the start plus button
    /// </summary>
    public void ClickOnStartPlusButton()
    {
        // Add one to the StartPageNumber, iff the start page number is less then the EndPageNumber
        this.StartPageNumber = this.StartPageNumber < this.EndPageNumber ? this.StartPageNumber + 1 : this.StartPageNumber;
        this.UpdateLabel();
    }

    /// <summary>
    /// The action that is executed after clicking on the start minus button
    /// </summary>
    public void ClickOnStartMinButton()
    {
        // Subtracts one from the StartPageNumber, iff the start page number is greater then one
        this.StartPageNumber = this.StartPageNumber > 1 ? this.StartPageNumber - 1 : this.StartPageNumber;
        this.UpdateLabel();
    }

    /// <summary>
    /// The action that is executed after clicking on the end plus button
    /// </summary>
    public void ClickOnEndPlusButton()
    {
        // Add one to the EndPageNumber, iff the start page number is less then the number of pages in the document
        this.EndPageNumber = this.EndPageNumber < this.MaxPage ? this.EndPageNumber + 1 : this.EndPageNumber;
        this.UpdateLabel();
    }

    /// <summary>
    /// The action that is executed after clicking on the end minus button
    /// </summary>
    public void ClickOnEndMinButton()
    {
        // Subtracts one from the EndPageNumber, iff the EndPageNubmer is greater then the StartPageNumber
        this.EndPageNumber = this.EndPageNumber > this.StartPageNumber ? this.EndPageNumber - 1 : this.EndPageNumber;
        this.UpdateLabel();
    }

    /// <summary>
    /// Returns the current start page number.
    /// </summary>
    /// <returns>The start number.</returns>
    public int GetStartPageNumber()
    {
        // Return the StartPageNumber
        return this.StartPageNumber;
    }

    /// <summary>
    /// Returns the current end page number.
    /// </summary>
    /// <returns>The end number.</returns>
    public int GetEndPageNumber()
    {
        // Return the EndPageNumber
        return this.EndPageNumber;
    }

    /// <summary>
    /// Returns the maximum number of pages the floating document contains.
    /// </summary>
    /// <returns>The max number.</returns>
    public int GetMaxPage()
    {
        // Return the MaxPage
        return this.MaxPage;
    }

    /// <summary>
    /// Updates the counter for the maximum number of pages the floating document contains.
    /// This changes the end page number.
    /// </summary>
    private void UpdateDocumentPageCount()
    {
        // Get the current floating document
        var floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();
        if (floatingDocument == null || floatingDocument.pages.Count <= 0)
        {
            return;
        }

        // Set the MaxPage and EndPageNumber variables
        this.MaxPage = floatingDocument.pages.Count;
        this.EndPageNumber = floatingDocument.pages.Count;

        this.UpdateLabel();
    }

    /// <summary>
    /// This is the start method and is immediately executed.
    /// </summary>
    void Start()
    {
        // Set up the slider listeners
        this.SetupSliderListeners();
    }

    /// <summary>
    /// Updates the start page number and the end page number of
    /// the text to what currently the counters contain.
    /// </summary>
    void UpdateLabel()
    {
        if (this.StartPageNumberText != null)
        {
            // Set the value of the StartPageNumberText in the environment
            this.StartPageNumberText.text = this.StartPageNumber.ToString();
        }

        if (this.EndPageNumberText != null)
        {
            // Set the value of the EndPageNumberText in the environment
            this.EndPageNumberText.text = this.EndPageNumber.ToString();
        }
    }
}
