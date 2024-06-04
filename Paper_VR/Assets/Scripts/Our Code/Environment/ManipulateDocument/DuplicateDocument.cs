using System.Collections.Generic;
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
    /// This method handles the click of the return button; hiding the subpanel
    /// </summary>
    public void OnReturnClick()
    {
        // Toggle the visibility of the subPanel
        this.subPanel.SetActive(false);

        // Show the main panel
        this.menu.SetActive(true);
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

            this.TogglePanels(script);

            GameObject gameManager = GameObject.Find("GameManager");

            // Adds the new duplicated pdf to the environment information
            EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();
            environment.GetFloatingDocuments().Add(script);

            // Toggle the visibility of the menu and the subPanel
            this.menu.SetActive(false);
            this.subPanel.SetActive(false);
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

            this.TogglePanels(newFloatingDocument);

            newFloatingDocument.pdfPath = floatingDocument.pdfPath;
            newFloatingDocument.pages = new List<int> { floatingDocument.pages[floatingDocument.currentPageIndex] };
            newFloatingDocument.currentPageIndex = 0;
            newFloatingDocument.sprites = new List<Sprite> { currentPageSprite };

            GameObject gameManager = GameObject.Find("GameManager");

            EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();
            environment.GetFloatingDocuments().Add(newFloatingDocument);

            // Toggle the visibility of the menu and the subPanel
            this.menu.SetActive(false);
            this.subPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("The PDFCanvas prefab is null");
        }
    }

    private void TogglePanels(FloatingDocument floatingDocument)
    {
        GameObject subPanel = floatingDocument.transform.Find(this.subPanel.name).gameObject;
        subPanel.SetActive(false);
        GameObject menuPanel = floatingDocument.transform.Find(this.menu.name).gameObject;
        menuPanel.SetActive(false);
    }
}