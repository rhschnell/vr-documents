using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// This class exports a document to the google drive as a PDF file.
/// </summary>
public class ExportDocument : MonoBehaviour
{
    /// <summary>
    /// The menu which appears under the floating document
    /// </summary>
    public GameObject menu;

    /// <summary>
    /// The floating document that is being exported
    /// </summary>
    public GameObject floatingDocument;

    /// <summary>
    /// This method handles the click of the duplicate button; duplicating the whole floating document
    /// </summary>
    public void OnExportClick()
    {
        // Get the BackendPDF component and call the ExtractPDF method
        GameObject pdfConvert = GameObject.Find("PDFConvert");
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
            yield return PDFoperations.GetPDF(pages, convertPDF);

            byte[] content = PDFoperations.pdfContent;

            // Upload the PDF file to the google drive
            yield return convertPDF.ExportPDFToDrive(EnvironmentInformation.saveFolderId, name, content, null, false);
        }
        else
        {
            // Log an error if the floating document is null
            Debug.LogError("The floating document is null");
        }
    }
}