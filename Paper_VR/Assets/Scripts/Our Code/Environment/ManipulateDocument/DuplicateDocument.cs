using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles the duplication of a document
/// </summary>
public class DuplicateDocument : MonoBehaviour
{
    /// <summary>
    /// The panel which appears when clicking the duplication button
    /// </summary>
    public GameObject subPanel;

    /// <summary>
    /// The menu which appears under the floating document
    /// </summary>
    public GameObject menu;

    /// <summary>
    /// The PDFCanvas on which this menu appears
    /// </summary>
    public GameObject pdfCanvasPrefab;

    /// <summary>
    /// The duplication menu
    /// </summary>
    public GameObject duplicateMenu;

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
    /// This method handles the click of the duplicate button; showing the subpanel with duplication options
    /// </summary>
    public void OnDuplicationClick()
    {
        // Toggle the visibility of the subPanel
        this.subPanel.SetActive(true);

        // Hide the main panel
        this.menu.SetActive(false);
    }

    /// <summary>
    /// This method handles the click of the duplicate full document button; duplicating the whole floating document
    /// </summary>
    public void OnDuplicationDocumentClick()
    {
        this.DuplicatePDFCanvas();
    }

    /// <summary>
    /// This method handles the click of the duplicate button; duplicating the shown page of a floating document
    /// </summary>
    public void OnDuplicationPageClick()
    {
        this.DuplicatePagePDF();
    }

    /// <summary>
    /// Handles the click of the duplicate button; duplicating a subsection of a floating document
    /// </summary>
    public void OnDuplicationSubsectionClick()
    {
        this.DuplicateSubsectionPDF();
    }

    /// <summary>
    /// Handles the click of the duplicate subsection button; opening the subsection menu
    /// </summary>
    public void OnDuplicationSubsectionMenuClick()
    {
        this.duplicateMenu.SetActive(!this.duplicateMenu.activeSelf);
        this.UpdateDocumentPageCount();
    }

    /// <summary>
    /// This method handles the click of the return button; hiding the subpanel
    /// </summary>
    public void OnReturnClick()
    {
        // Toggle the visibility of the subPanel
        this.subPanel.SetActive(false);
        this.duplicateMenu.SetActive(false);

        // Show the main panel
        this.menu.SetActive(true);
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

    private void DuplicatePDFCanvas()
    {
        if (this.pdfCanvasPrefab != null)
        {
            // Calculate the position of the new pdf
            Transform transform = this.pdfCanvasPrefab.transform;
            Vector3 newPos = transform.position + (transform.right * 1.0f);

            // Instantiate a new PDFCanvas in the scene
            GameObject newPdf = Instantiate(this.pdfCanvasPrefab, newPos, transform.rotation);
            FloatingDocument script = newPdf.GetComponent<FloatingDocument>();

            this.ToggleNewPanels(script);

            GameObject gameManager = GameObject.Find("GameManager");

            // Adds the new duplicated pdf to the environment information
            EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();
            environment.GetFloatingDocuments().Add(script);

            // Toggle the visibility of the menu and the subPanel
            this.ToggleOldPanels();
        }
        else
        {
            Debug.LogError("The PDFCanvas prefab is null");
        }
    }

    private void DuplicatePagePDF()
    {
        if (this.pdfCanvasPrefab != null)
        {
            FloatingDocument floatingDocument = this.pdfCanvasPrefab.GetComponent<FloatingDocument>();

            Sprite currentPageSprite = floatingDocument.sprites[floatingDocument.currentPageIndex];

            Transform transform = this.pdfCanvasPrefab.transform;
            Vector3 newPos = transform.position + (transform.right * 1.0f);

            GameObject newPdf = Instantiate(this.pdfCanvasPrefab, newPos, transform.rotation);
            FloatingDocument newFloatingDocument = newPdf.GetComponent<FloatingDocument>();

            this.ToggleNewPanels(newFloatingDocument);

            newFloatingDocument.pdfId = floatingDocument.pdfId;
            newFloatingDocument.pages = new List<int> { floatingDocument.pages[floatingDocument.currentPageIndex] };
            newFloatingDocument.currentPageIndex = 0;
            newFloatingDocument.sprites = new List<Sprite> { currentPageSprite };

            GameObject gameManager = GameObject.Find("GameManager");

            EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();
            environment.GetFloatingDocuments().Add(newFloatingDocument);

            // Toggle the visibility of the menu and the subPanel
            this.ToggleOldPanels();
        }
        else
        {
            Debug.LogError("The PDFCanvas prefab is null");
        }
    }

    private void DuplicateSubsectionPDF()
    {
        if (this.pdfCanvasPrefab != null)
        {
            int startIndex = this.StartPageNumber - 1;
            int endIndex = this.EndPageNumber - 1;

            FloatingDocument floatingDocument = this.pdfCanvasPrefab.GetComponent<FloatingDocument>();

            if (startIndex >= 0 && endIndex < floatingDocument.sprites.Count && startIndex <= endIndex)
            {
                List<Sprite> subsectionSprites = new List<Sprite>();

                for (int i = startIndex; i <= endIndex; i++)
                {
                    subsectionSprites.Add(floatingDocument.sprites[i]);
                }

                Transform transform = this.pdfCanvasPrefab.transform;
                Vector3 newPos = transform.position + (transform.right * 1.0f);

                GameObject newPdf = Instantiate(this.pdfCanvasPrefab, newPos, transform.rotation);
                FloatingDocument newFloatingDocument = newPdf.GetComponent<FloatingDocument>();

                this.ToggleNewPanels(newFloatingDocument);

                newFloatingDocument.pdfId = floatingDocument.pdfId;
                newFloatingDocument.pages = new List<int>();
                for (int i = 0; i <= endIndex - startIndex; i++)
                {
                    newFloatingDocument.pages.Add(i);
                }

                newFloatingDocument.currentPageIndex = 0;
                newFloatingDocument.sprites = subsectionSprites;
                newFloatingDocument.SetSprite();

                GameObject gameManager = GameObject.Find("GameManager");
                EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();
                environment.GetFloatingDocuments().Add(newFloatingDocument);

                // Toggle the visibility of the menu and the subPanel
                this.ToggleOldPanels();
            }
            else
            {
                Debug.LogError("Invalid subsection range. Startindex must be above 0 and endindex should be higher then startindex");
            }
        }
        else
        {
            Debug.LogError("The PDFCanvas prefab is null");
        }
    }

    /// <summary>
    /// Updates the counter for the maximum number of pages the floating document contains.
    /// This changes the end page number.
    /// </summary>
    private void UpdateDocumentPageCount()
    {
        var floatingDocument = this.pdfCanvasPrefab.GetComponent<FloatingDocument>();
        if (floatingDocument == null || floatingDocument.pages.Count <= 0)
        {
            Debug.LogError("Floating document is null or it has 0 or less pages");
        }

        this.MaxPage = floatingDocument.pages.Count;
        this.EndPageNumber = floatingDocument.pages.Count;

        this.UpdateLabel();
    }

    private void ToggleNewPanels(FloatingDocument floatingDocument)
    {
        GameObject subPanel = floatingDocument.transform.Find(this.subPanel.name).gameObject;
        subPanel.SetActive(false);
        GameObject menuPanel = floatingDocument.transform.Find(this.menu.name).gameObject;
        menuPanel.SetActive(false);
        GameObject duplicateMenu = floatingDocument.transform.Find(this.duplicateMenu.name).gameObject;
        duplicateMenu.SetActive(false);
    }

    private void ToggleOldPanels()
    {
        this.menu.SetActive(false);
        this.subPanel.SetActive(false);
        this.duplicateMenu.SetActive(false);
    }

    /// <summary>
    /// This is the start method and immediately executed.
    /// </summary>
    void Start()
    {
        this.SetupSliderListeners();
    }
}