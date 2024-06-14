using UnityEngine;

/// <summary>
/// Logic for deleting subsections of the document.
/// </summary>
public class DeleteDocument : MonoBehaviour
{
    /// <summary>
    /// The canvas of the PDF containing the floating document.
    /// </summary>
    public GameObject pdfPrefab;

    /// <summary>
    /// The menu controller that is used to get the start and end page number.
    /// </summary>
    public MenuController MenuController;

    /// <summary>
    /// The action that is executed after clicking on the split button in the split menu.
    /// </summary>
    public void OnClickDeleteSubsection()
    {
        var floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();
        int end = this.MenuController.GetEndPageNumber();
        int start = this.MenuController.GetStartPageNumber();

        // If the document has only one page, we are going to delete the whole document
        if (floatingDocument.exportPages.Count == 1)
        {
            this.OnClickDeleteDocument();
            return;
        }

        // Check if the end and start variables are less then the number of pages in the document
        if (floatingDocument != null && ((end - start + 1) < floatingDocument.pages.Count))
        {
            this.MenuController.Menu.SetActive(false);
            this.MenuController.subsectionMenu.SetActive(false);

            // Delete the subsection
            var deletePDF = new DeletePDF(floatingDocument);
            var newPage = deletePDF.DeleteDocument(start, end);
        }
    }

    /// <summary>
    /// Deletes the current page of the floating document.
    /// </summary>
    public void OnClickDeletePage()
    {
        // Get the currently shown page of the floating document
        var floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();

        // If the document has only one page, we can call the method to delete the whole document
        if (floatingDocument.exportPages.Count == 1)
        {
            this.OnClickDeleteDocument();
            return;
        }

        int currentPage = floatingDocument.currentPageIndex + 1;

        if (floatingDocument != null)
        {
            this.MenuController.Menu.SetActive(false);
            this.MenuController.subsectionMenu.SetActive(false);

            // Delete the shown page from the floating document
            var deletePDF = new DeletePDF(floatingDocument);
            deletePDF.DeleteDocument(currentPage, currentPage);
        }
    }

    /// <summary>
    /// Deletes the floating document from the environment
    /// </summary>
    public void OnClickDeleteDocument()
    {
        var floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();
        GameObject gamemanager = GameObject.Find("GameManager");
        EnvironmentInformation environment = gamemanager.GetComponent<EnvironmentInformation>();
        environment.GetFloatingDocuments().Remove(floatingDocument);

        Destroy(this.pdfPrefab);
        this.pdfPrefab = null;
    }
}
