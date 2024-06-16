using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains the operations for the PDFs.
/// Like retriving the content of a floating document.
/// </summary>
public class PDFOperations {
    /// <summary>
    /// The content of the PDF
    /// </summary>
    public static byte[] pdfContent;

    /// <summary>
    /// This method gets the PDF content of the floating document.
    /// </summary>
    /// <param name="pages">The pages of the pdfs to get the content of</param>
    /// <param name="convertPDF">and instance of the class backendserver</param>
    /// <returns>IEnumerator</returns>
    public static IEnumerator GetPDF(List<Tuple<string, string, int>> pages, BackendPDF convertPDF)
    {
        // Set the current PDF name, id, and pages
        string currentPDFName = pages[0].Item1;
        string currentPDFId = pages[0].Item2;
        List<int> currentPDFPages = new List<int>();
        int currentPageIndex = 0;

        byte[] content = new byte[0];

        // Get the pages of the current PDF until the next PDF is reached
        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i].Item2 == currentPDFId)
            {
                currentPDFPages.Add(pages[i].Item3);
            }
            else
            {
                // Extract those pages
                yield return convertPDF.ExtractPDF(currentPDFId, currentPDFName, currentPDFPages);
                content = convertPDF.extractedPDFcontent;
                break;
            }

            // Increment the current page index
            currentPageIndex++;
        }

        // If all the pages are of the same PDF
        if (currentPageIndex == pages.Count)
        {
            yield return convertPDF.ExtractPDF(currentPDFId, currentPDFName, currentPDFPages);
            content = convertPDF.extractedPDFcontent;
            pdfContent = content;
            yield break;
        }

        string oldPDFName = currentPDFName;

        // Set the the current PDF name, id, and pages
        currentPDFName = pages[currentPageIndex].Item1;
        currentPDFId = pages[currentPageIndex].Item2;
        currentPDFPages = new List<int>();

        Debug.Log("Current PDF Name: " + currentPDFName);
        Debug.Log("Current PDF ID: " + currentPDFId);
        Debug.Log("Current PDF Pages: " + currentPDFPages);
        Debug.Log("Current Page Index: " + currentPageIndex);
        Debug.Log("pdfcontent: " + pdfContent);
        Debug.Log("number of pages: " + pages.Count);

        // Start Loop for the rest of the pages
        for (int i = currentPageIndex; i < pages.Count; i++)
        {
            Debug.Log("Current PDF Name: " + currentPDFName);
            Debug.Log("Current PDF ID: " + currentPDFId);
            Debug.Log("List id: " + pages[i].Item2);
            Debug.Log("Current PDF Pages: " + currentPDFPages.Count);
            Debug.Log("Current Page Index: " + i);
            if (pages[i].Item2 == currentPDFId)
            {
                currentPDFPages.Add(pages[i].Item3);
                Debug.Log("Yes");
            }
            else
            {
                Debug.Log("No");
                // Extract those pages
                yield return convertPDF.ExtractPDF(currentPDFId, currentPDFName, currentPDFPages);

                // Merge the content of the current PDF with the content of the extracted PDF
                byte[] newContent = convertPDF.extractedPDFcontent;
                yield return convertPDF.MergePDF(oldPDFName, content, currentPDFName, newContent);
                content = convertPDF.mergedPDFcontent;

                // Set the old PDF name to the current PDF name
                oldPDFName = currentPDFName;

                // Set the current PDF name, id, and pages
                currentPDFName = pages[i].Item1;
                currentPDFId = pages[i].Item2;
                currentPDFPages = new List<int>();

                // Subtract 1 from i to recheck the current page
                i--;
            }
        }

        // Extract the last PDF
        Debug.Log("Current PDF Name: " + currentPDFName);
        Debug.Log("Current PDF ID: " + currentPDFId);
        Debug.Log("Current PDF Pages: " + currentPDFPages.Count);
        yield return convertPDF.ExtractPDF(currentPDFId, currentPDFName, currentPDFPages);
        byte[] lastContent = convertPDF.extractedPDFcontent;

        // Merge the content of the last PDF with the content of the extracted PDF
        yield return convertPDF.MergePDF(oldPDFName, content, currentPDFName, lastContent);
        content = convertPDF.mergedPDFcontent;

        // Set the content of the PDF
        pdfContent = content;
    }
}
