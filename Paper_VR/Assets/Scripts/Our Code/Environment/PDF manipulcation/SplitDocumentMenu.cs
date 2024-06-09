using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The SplitDocumentMenu is doing the basic behavior for the menu pop up, when splitting a document
/// </summary>
public class SplitDocumentMenu : MonoBehaviour
{
    /// <summary>
    /// The canvas of the PDF containing the floating document
    /// </summary>
    public GameObject PdfPrefab;

    /// <summary>
    /// The manipulation menu
    /// </summary>
    public GameObject Menu;

    /// <summary>
    /// The splitting menu
    /// </summary>
    public GameObject SplitMenu;

    /// <summary>
    /// Text field containing the currently selected start page from which to split
    /// </summary>
    public TMP_Text StartPageNumberText;

    /// <summary>
    /// Text field containing the currently selected end page at which to split
    /// </summary>
    public TMP_Text EndPageNumberText;

    /// <summary>
    /// The plus button for increasing the start page number
    /// </summary>
    public UnityEngine.UI.Button StartPlusButton;

    /// <summary>
    /// The minus button for decreasing the start page number
    /// </summary>
    public UnityEngine.UI.Button StartMinusButton;

    /// <summary>
    /// THis is the start slider for changing the start page number
    /// </summary>
    public UnityEngine.UI.Slider StartSlider;

    /// <summary>
    /// The plus button for increasing the end page number
    /// </summary>
    public UnityEngine.UI.Button EndPlusButton;

    /// <summary>
    /// The minus button for decreasing the end page number
    /// </summary>
    public UnityEngine.UI.Button EndMinusButton;

    /// <summary>
    ///  This is the end slider for changing the end page number
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
        if (this.StartSlider != null)
        {
            this.StartSlider.onValueChanged.AddListener((v) =>
            {
                // Ensure the startPageNumber does not exceed the current value of endPageNumber
                var value = v * (this.EndPageNumber - 1);
                this.StartPageNumber = Mathf.RoundToInt(value + 1);
                this.UpdateLabel();
            });
        }

        if (this.EndSlider != null)
        {
            this.EndSlider.onValueChanged.AddListener((v) =>
            {
                // Ensure the endPageNumber does not exceed the maxPage value and is not less than startPageNumber
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
        this.SplitMenu.SetActive(!this.SplitMenu.activeSelf);
        this.UpdateDocumentPageCount();
    }

    /// <summary>
    /// The action that is executed after clicking on the start plus button
    /// </summary>
    public void ClickOnStartPlusButton()
    {
        this.StartPageNumber = this.StartPageNumber < this.EndPageNumber ? this.StartPageNumber + 1 : this.StartPageNumber;
        this.UpdateLabel();
    }

    /// <summary>
    /// The action that is executed after clicking on the start minus button
    /// </summary>
    public void ClickOnStartMinButton()
    {
        this.StartPageNumber = this.StartPageNumber > 1 ? this.StartPageNumber - 1 : this.StartPageNumber;
        this.UpdateLabel();
    }

    /// <summary>
    /// The action that is executed after clicking on the end plus button
    /// </summary>
    public void ClickOnEndPlusButton()
    {
        this.EndPageNumber = this.EndPageNumber < this.MaxPage ? this.EndPageNumber + 1 : this.EndPageNumber;
        this.UpdateLabel();
    }

    /// <summary>
    /// The action that is executed after clicking on the end minus button
    /// </summary>
    public void ClickOnEndMinButton()
    {
        this.EndPageNumber = this.EndPageNumber > this.StartPageNumber ? this.EndPageNumber - 1 : this.EndPageNumber;
        this.UpdateLabel();
    }

    /// <summary>
    /// Updates the start page number and the end page number of
    /// the text to what currently the counters contain.
    /// </summary>
    public void UpdateLabel()
    {
        if (this.StartPageNumberText != null)
        {
            this.StartPageNumberText.text = this.StartPageNumber.ToString();
        }

        if (this.EndPageNumberText != null)
        {
            this.EndPageNumberText.text = this.EndPageNumber.ToString();
        }
    }

    /// <summary>
    /// The action that is executed after clicking on the split button in the split menu.
    /// </summary>
    public void OnClickSplit()
    {
        var floatingDocument = this.PdfPrefab.GetComponent<FloatingDocument>();
        if (floatingDocument == null ||
            ((this.EndPageNumber - this.StartPageNumber + 1) >= floatingDocument.pages.Count))
        {
            return;
        }

        // floatingDocument.sprites
        // environmentInfo.ConvertPdf()
        // Create a new floating document
        var pdfPrefabTransform = this.PdfPrefab.transform;
        var newPos = pdfPrefabTransform.position + (pdfPrefabTransform.right * 1.0f);

        this.Menu.SetActive(false);
        this.SplitMenu.SetActive(false);

        var pdf = Instantiate(this.PdfPrefab, newPos, pdfPrefabTransform.rotation);
        var newFloatingDocument = pdf.GetComponent<FloatingDocument>();

        var splitPdf = new SplitPDF(floatingDocument, newFloatingDocument);
        splitPdf.SplitDocument(this.StartPageNumber, this.EndPageNumber);

        // Find the game object called GameManager
        var gameManager = GameObject.Find("GameManager");
        // Get the GameManager component with the environment script
        var environment = gameManager.GetComponent<EnvironmentInformation>();
        environment.GetFloatingDocuments().Add(newFloatingDocument);
    }

    /// <summary>
    /// Splits the current page from the document.
    /// </summary>
    public void OnClickSplitIndividualPage()
    {
        var floatingDocument = this.PdfPrefab.GetComponent<FloatingDocument>();
        this.StartPageNumber = floatingDocument.currentPageIndex + 1;
        this.EndPageNumber = floatingDocument.currentPageIndex + 1;
        this.OnClickSplit();
    }

    /// <summary>
    /// This is the start method and immediately executed.
    /// </summary>
    void Start()
    {
        this.UpdateDocumentPageCount();
        this.SetupSliderListeners();
    }

    /// <summary>
    /// Updates the counter for the maximum number of pages the floating document contains.
    /// This changes the end page number.
    /// </summary>
    private void UpdateDocumentPageCount()
    {
        var floatingDocument = this.PdfPrefab.GetComponent<FloatingDocument>();
        if (floatingDocument == null || floatingDocument.pages.Count <= 0)
        {
            return;
        }

        this.MaxPage = floatingDocument.pages.Count;
        this.EndPageNumber = floatingDocument.pages.Count;

        this.UpdateLabel();
    }
}
