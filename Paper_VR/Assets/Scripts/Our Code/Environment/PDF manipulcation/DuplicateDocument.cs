using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the duplication of a document
/// </summary>
public class DuplicateDocument : MonoBehaviour
{
    /// <summary>
    /// The PDFCanvas on which this menu appears
    /// </summary>
    public GameObject pdfCanvasPrefab;

    /// <summary>
    /// The menu which appears under the floating document
    /// </summary>
    public MenuController MenuController;

    /// <summary>
    /// This method handles the click of the duplicate button; duplicating the whole floating document
    /// </summary>
    public void OnDuplicationClick()
    {
        this.DuplicatePDFCanvas();
    }

    /// <summary>
    /// This method handles the click of the duplicate page button; duplicating the current page of the floating document
    /// </summary>
    public void DuplicatePDFCanvas()
    {
        if (this.pdfCanvasPrefab != null)
        {
            // Toggle the visibility of the menu and the subPanel
            this.MenuController.Menu.SetActive(false);
            this.MenuController.subsectionMenu.SetActive(false);

            // Calculate the position of the new pdf
            Transform transform = this.pdfCanvasPrefab.transform;
            Vector3 newPos = transform.position + (transform.right * 1.0f);

            // Instantiate a new PDFCanvas in the scene
            GameObject newPdf = Instantiate(this.pdfCanvasPrefab, newPos, transform.rotation);
            FloatingDocument newFloatingDocument = newPdf.GetComponent<FloatingDocument>();

            GameObject gameManager = GameObject.Find("GameManager");

            // Adds the new duplicated pdf to the environment information
            EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();
            environment.GetFloatingDocuments().Add(newFloatingDocument);
        }
        else
        {
            Debug.LogError("The PDFCanvas prefab is null");
        }
    }

    /// <summary>
    /// this method duplicates the current page of the floating document
    /// </summary>
    public void DuplicatePagePDF()
    {
        if (this.pdfCanvasPrefab != null)
        {
            // Toggle the visibility of the menu and the subPanel
            this.MenuController.Menu.SetActive(false);
            this.MenuController.subsectionMenu.SetActive(false);

            FloatingDocument floatingDocument = this.pdfCanvasPrefab.GetComponent<FloatingDocument>();

            Sprite currentPageSprite = floatingDocument.sprites[floatingDocument.currentPageIndex];

            Transform transform = this.pdfCanvasPrefab.transform;
            Vector3 newPos = transform.position + (transform.right * 1.0f);

            GameObject newPdf = Instantiate(this.pdfCanvasPrefab, newPos, transform.rotation);
            FloatingDocument newFloatingDocument = newPdf.GetComponent<FloatingDocument>();

            newFloatingDocument.pdfId = floatingDocument.pdfId;
            newFloatingDocument.pages = new List<int> { floatingDocument.pages[floatingDocument.currentPageIndex] };
            newFloatingDocument.exportPages = new List<Tuple<string, string, int>> { floatingDocument.exportPages[floatingDocument.currentPageIndex] };
            newFloatingDocument.currentPageIndex = 0;
            newFloatingDocument.sprites = new List<Sprite> { currentPageSprite };

            GameObject gameManager = GameObject.Find("GameManager");

            EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();
            environment.GetFloatingDocuments().Add(newFloatingDocument);
        }
        else
        {
            Debug.LogError("The PDFCanvas prefab is null");
        }
    }

    /// <summary>
    /// this method duplicates the subsection of the floating document
    /// </summary>
    public void DuplicateSubsectionPDF()
    {
        if (this.pdfCanvasPrefab != null)
        {
            int startIndex = this.MenuController.GetStartPageNumber() - 1;
            int endIndex = this.MenuController.GetEndPageNumber() - 1;

            FloatingDocument floatingDocument = this.pdfCanvasPrefab.GetComponent<FloatingDocument>();

            if (startIndex >= 0 && endIndex < floatingDocument.sprites.Count && startIndex <= endIndex)
            {
                // Toggle the visibility of the menu and the subPanel
                this.MenuController.Menu.SetActive(false);
                this.MenuController.subsectionMenu.SetActive(false);

                List<Sprite> subsectionSprites = new List<Sprite>();
                List<Tuple<string, string, int>> exportPages = new List<Tuple<string, string, int>>();

                for (int i = startIndex; i <= endIndex; i++)
                {
                    subsectionSprites.Add(floatingDocument.sprites[i]);
                    exportPages.Add(floatingDocument.exportPages[i]);
                }

                Transform transform = this.pdfCanvasPrefab.transform;
                Vector3 newPos = transform.position + (transform.right * 1.0f);

                GameObject newPdf = Instantiate(this.pdfCanvasPrefab, newPos, transform.rotation);
                FloatingDocument newFloatingDocument = newPdf.GetComponent<FloatingDocument>();

                newFloatingDocument.pdfId = floatingDocument.pdfId;
                newFloatingDocument.pages = new List<int>();
                for (int i = 0; i <= endIndex - startIndex; i++)
                {
                    newFloatingDocument.pages.Add(i);
                }

                newFloatingDocument.currentPageIndex = 0;
                newFloatingDocument.sprites = subsectionSprites;
                newFloatingDocument.exportPages = exportPages;
                newFloatingDocument.SetSprite();

                GameObject gameManager = GameObject.Find("GameManager");
                EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();
                environment.GetFloatingDocuments().Add(newFloatingDocument);
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
}