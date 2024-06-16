using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the duplication of a document.
/// </summary>
public class DuplicateController : MonoBehaviour
{
    /// <summary>
    /// The PDFCanvas on which this menu appears.
    /// </summary>
    public GameObject pdfPrefab;

    /// <summary>
    /// The menu which appears under the floating document.
    /// </summary>
    public MenuController MenuController;

    /// <summary>
    /// This method handles the click of the duplicate button; duplicating the whole floating document.
    /// </summary>
    public void OnDuplicationClick()
    {
        // Calls the DuplicatePDFCanvas method to duplicate the full document
        this.DuplicatePDFCanvas();
    }

    /// <summary>
    /// This method handles the click of the duplicate full button; duplicating the whole floating document.
    /// </summary>
    public void DuplicatePDFCanvas()
    {
        if (this.pdfPrefab != null)
        {
            // Toggle the visibility of the menu and the subPanel
            this.MenuController.Menu.SetActive(false);
            this.MenuController.subsectionMenu.SetActive(false);

            // Calculate the position of the new pdf
            Transform transform = this.pdfPrefab.transform;
            Vector3 newPos = transform.position + (transform.right * 1.0f);

            // Instantiate a new PDFCanvas in the scene
            GameObject newPdf = Instantiate(this.pdfPrefab, newPos, transform.rotation);
            FloatingDocument newFloatingDocument = newPdf.GetComponent<FloatingDocument>();

            // Duplicate the document
            DuplicatePDF duplicatePDF = new DuplicatePDF(this.pdfPrefab.GetComponent<FloatingDocument>(), newFloatingDocument);
            duplicatePDF.DuplicateDocument(0, this.pdfPrefab.GetComponent<FloatingDocument>().sprites.Count - 1);

            GameObject gameManager = GameObject.Find("GameManager");

            // Adds the new duplicated pdf to the environment information
            EnvironmentController environment = gameManager.GetComponent<EnvironmentController>();
            environment.GetFloatingDocuments().Add(newFloatingDocument);
        }
        else
        {
            Debug.LogError("The PDFCanvas prefab is null");
        }
    }

    /// <summary>
    /// This method handles the click of the duplicate page button; duplicating the current page of the floating document.
    /// </summary>
    public void DuplicatePagePDF()
    {
        if (this.pdfPrefab != null)
        {
            var floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();

            this.MenuController.StartPageNumber = floatingDocument.currentPageIndex + 1;
            this.MenuController.EndPageNumber = floatingDocument.currentPageIndex + 1;
            this.DuplicateSubsectionPDF();
        }
        else
        {
            Debug.LogError("The PDFCanvas prefab is null");
        }
    }

    /// <summary>
    /// This method handles the click of the duplicate subsection button; duplicating the subsedction of the floating document.
    /// </summary>
    public void DuplicateSubsectionPDF()
    {
        if (this.pdfPrefab != null)
        {
            // Get the start- and endpage for duplicating a subsection
            int startIndex = this.MenuController.GetStartPageNumber() - 1;
            int endIndex = this.MenuController.GetEndPageNumber() - 1;

            FloatingDocument floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();

            // Check whether the start- and endpage make sense
            if (startIndex >= 0 && endIndex < floatingDocument.sprites.Count && startIndex <= endIndex)
            {
                // Toggle the visibility of the menu and the subPanel
                this.MenuController.Menu.SetActive(false);
                this.MenuController.subsectionMenu.SetActive(false);

                // Calculate the position of the new pdf
                Transform transform = this.pdfPrefab.transform;
                Vector3 newPos = transform.position + (transform.right * 1.0f);

                // Instantiate a new PDFCanvas in the scene
                GameObject newPdf = Instantiate(this.pdfPrefab, newPos, transform.rotation);
                FloatingDocument newFloatingDocument = newPdf.GetComponent<FloatingDocument>();

                DuplicatePDF duplicatePDF = new DuplicatePDF(floatingDocument, newFloatingDocument);
                var newDoc = duplicatePDF.DuplicateDocument(startIndex, endIndex);

                GameObject gameManager = GameObject.Find("GameManager");

                // Adds the new duplicated pdf to the environment information
                EnvironmentController environment = gameManager.GetComponent<EnvironmentController>();
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