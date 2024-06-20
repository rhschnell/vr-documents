using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;

/// <summary>
/// This class exports a document to the google drive as a PDF file.
/// </summary>
public class ExportController : MonoBehaviour
{
    /// <summary>
    /// The menu which appears under the floating document.
    /// </summary>
    public GameObject menu;

    /// <summary>
    /// The floating document that is being exported.
    /// </summary>
    public GameObject floatingDocument;

    /// <summary>
    /// The panel that appears when the document is exported.
    /// </summary>
    public GameObject panel;

    /// <summary>
    /// The text that appears on the panel when the document is exported.
    /// </summary>
    public TMPro.TextMeshProUGUI text;

    /// <summary>
    /// This method handles the click of the duplicate button; duplicating the whole floating document.
    /// </summary>
    public void OnExportClick()
    {
        // Get the BackendPDF component and call the ExtractPDF method
        GameObject pdfConvert = GameObject.Find("PDFConversionManager");
        BackendPDF convertPDF = pdfConvert.GetComponent<BackendPDF>();
        this.StartCoroutine(this.ExportPDF(convertPDF));
    }

    /// <summary>
    /// This method exports the floating document as a PDF file.
    /// It does this by calling the ExtractPDF method from the BackendPDF component.
    /// </summary>
    /// <param name="convertPDF">The BackendPDF component that will extract the PDF file</param>
    /// <returns>IEnumerator</returns>
    public IEnumerator ExportPDF(BackendPDF convertPDF)
    {
        if (this.floatingDocument != null)
        {
            // Unshow the menu of the original pdf
            this.menu.SetActive(false);

            // Get the id, name, and pages of the floating document
            string name = this.floatingDocument.GetComponent<FloatingDocument>().pdfName;
            List<Tuple<string, string, int>> pages = this.floatingDocument.GetComponent<FloatingDocument>().exportPages;
            // yield return convertPDF.ExtractPDF(id, name, pages);

            // Get the pdf content
            yield return PDFOperations.GetPDF(pages, convertPDF);

            byte[] content = PDFOperations.pdfContent;

            // Upload the PDF file to the google drive
            yield return convertPDF.ExportPDFToDrive(EnvironmentController.saveFolderId, name, content, null, false);
            this.StartCoroutine(this.OnExportSuccess());
        }
        else
        {
            // Log an error if the floating document is null
            Debug.LogError("The floating document is null");
        }
    }

    /// <summary>
    /// Is called when the export is successful. Sets the panel to active and displays a message.
    /// </summary>
    /// <returns>It has to wait, so IEnumerator</returns>
    public IEnumerator OnExportSuccess()
    {
        // find the game manager and get the environment controller
        GameObject gameManager = GameObject.Find("GameManager");
        EnvironmentController environmentController = gameManager.GetComponent<EnvironmentController>();

        // Set the panel to active and display a message
        string envName = environmentController.GetName();
        this.panel.SetActive(true);
        this.text.text = "Document saved in Saved Documents in " + envName + " on your drive!";

        // Wait 5 seconds
        yield return new WaitForSeconds(5.0f);

        // Set the panel to inactive
        this.panel.SetActive(false);
    }
}