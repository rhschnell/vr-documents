using UnityEngine;

/// <summary>
/// Handles the duplication of a document
/// </summary>
public class DuplicateDocument : MonoBehaviour
{
    /// <summary>
    /// The menu which appears under the floating document
    /// </summary>
    public GameObject menu;

    /// <summary>
    /// The PDFCanvas on which this menu appears
    /// </summary>
    public GameObject pdfCanvasPrefab;

    /// <summary>
    /// This method handles the click of the duplicate button; duplicating the whole floating document
    /// </summary>
    public void OnDuplicationClick()
    {
        this.DuplicatePDFCanvas();
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

            GameObject gameManager = GameObject.Find("GameManager");

            // Adds the new duplicated pdf to the environment information
            EnvironmentInformation environment = gameManager.GetComponent<EnvironmentInformation>();
            environment.GetFloatingDocuments().Add(script);

            //foreach (FloatingDocument fd in environment.GetFloatingDocuments())
            //{
            //    print(fd.ToString());
            //}
            // Unshow the menu of the original pdf
            this.menu.SetActive(false);
        }
        else
        {
            Debug.LogError("The PDFCanvas prefab is null");
        }
    }
}