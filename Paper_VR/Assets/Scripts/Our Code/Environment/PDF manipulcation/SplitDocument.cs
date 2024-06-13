using UnityEngine;

/// <summary>
/// The SplitDocumentMenu is doing the basic behavior for the menu pop up, when splitting a document.
/// </summary>
public class SplitDocument : MonoBehaviour
{
    /// <summary>
    /// The canvas of the PDF containing the floating document.
    /// </summary>
    public GameObject pdfPrefab;

    /// <summary>
    /// The menu controller that is used to get the start and end page number.
    /// </summary>
    public MenuController menuController;

    /// <summary>
    /// The action that is executed after clicking on the split button in the split menu.
    /// </summary>
    public void OnClickSplit()
    {
        // Get the current floating document and the start- and endpage numbers
        var floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();
        int end = this.menuController.GetEndPageNumber();
        int start = this.menuController.GetStartPageNumber();

        if (floatingDocument != null && ((end - start + 1) < floatingDocument.pages.Count))
        {
            // Calculate the position of the new pdf
            Transform transform = this.pdfPrefab.transform;
            Vector3 newPos = transform.position + (transform.right * 1.0f);

            // Toggle the visibility of the (subsection) menu
            this.menuController.Menu.SetActive(false);
            this.menuController.subsectionMenu.SetActive(false);

            // Instantiate a new PDFCanvas in the scene
            var pdf = Instantiate(this.pdfPrefab, newPos, transform.rotation);
            var newFloatingDocument = pdf.GetComponent<FloatingDocument>();

            var splitPdf = new SplitPDF(floatingDocument, newFloatingDocument);
            var newPage = splitPdf.SplitDocument(start, end);

            // Find the game object called GameManager
            GameObject gameManager = GameObject.Find("GameManager");

            // Get the GameManager component with the environment script
            EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();
            environment.GetFloatingDocuments().Add(newFloatingDocument);
        }
    }

    /// <summary>
    /// Splits the current page from the document.
    /// </summary>
    public void OnClickSplitIndividualPage()
    {
        // Get the current floating document
        var floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();

        // Set the StartPageNumber and EndPageNumber to the current page so we only split that page
        this.menuController.StartPageNumber = floatingDocument.currentPageIndex + 1;
        this.menuController.EndPageNumber = floatingDocument.currentPageIndex + 1;
        this.OnClickSplit();
    }
}
