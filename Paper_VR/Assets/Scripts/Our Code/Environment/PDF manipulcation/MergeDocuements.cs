using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that controls the logic of the merge menu, along with the merging of two PDFs.
/// </summary>
public class MergeDocuements : MonoBehaviour
{
    /// <summary>
    /// The canvas of the PDF containing the floating document
    /// </summary>
    public GameObject pdfPrefab;

    /// <summary>
    /// The menu controller that is used to get the start and end page number.
    /// </summary>
    public MenuController menuController;

    /// <summary>
    /// The game manager that is used to get the floating documents.
    /// </summary>
    public GameObject GameManager;

    /// <summary>
    /// the selected pdf name.
    /// </summary>
    public TMPro.TMP_Text selected;

    /// <summary>
    /// The error text that is displayed when the user tries to merge the same document.
    /// </summary>
    public TMPro.TMP_Text error;

    /// <summary>
    /// The merge menu that is displayed when the user wants to merge two documents.
    /// </summary>
    public GameObject mergeMenu;

    /// <summary>
    /// The index of the current floating document in the dropdown menu.
    /// </summary>
    public int self;

    /// <summary>
    /// Method that deals with updating the list of floating documents in the dropdown menu and setting the menu active.
    /// </summary>
    public void UpdateList()
    {
        if (!this.mergeMenu.activeSelf)
        {
            this.mergeMenu.SetActive(true);

            // Find the GameManager object
            if (this.GameManager == null)
            {
                this.GameManager = GameObject.Find("GameManager");
            }

            var environmentInformation = this.GameManager.GetComponent<EnvironmentInformation>();
            var floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();
            var list = environmentInformation.GetFloatingDocuments();
            foreach (FloatingDocument t in list)
            {
                t.mergeButton.SetActive(true);
                if (t == floatingDocument)
                {
                    t.mergeButton.SetActive(false);
                }
            }
        }
        else
        {
            this.mergeMenu.SetActive(false);
        }
    }

    /// <summary>
    /// Set the floating document to the current document.
    /// </summary>
    public void SetFloatingDocument()
    {
        if (this.GameManager == null)
        {
            this.GameManager = GameObject.Find("GameManager");
        }

        MergeManager mergeManager = this.GameManager.GetComponent<MergeManager>();
        mergeManager.floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();

        // Set all merge buttons to inactive
        var environmentInformation = this.GameManager.GetComponent<EnvironmentInformation>();
        var list = environmentInformation.GetFloatingDocuments();
        foreach (FloatingDocument t in list)
        {
            t.mergeButton.SetActive(false);
        }
    }

    /// <summary>
    /// The action that is executed after clicking on the split button in the split menu.
    /// </summary>
    public void OnClickMerge()
    {
        if (this.GameManager == null)
        {
            this.GameManager = GameObject.Find("GameManager");
        }

        this.mergeMenu.SetActive(false);

        var floatingDocument = this.pdfPrefab.GetComponent<FloatingDocument>();

        MergeManager mergeManager = this.GameManager.GetComponent<MergeManager>();

        var other = mergeManager.floatingDocument;
        if (other == floatingDocument)
        {
            this.error.gameObject.SetActive(true);
            return;
        }

        this.error.gameObject.SetActive(false);
        mergeManager.floatingDocument = null;

        if (floatingDocument != null)
        {
            // floatingDocument.sprites
            // environmentInfo.ConvertPdf()
            // Create a new floating document
            Transform transform = this.pdfPrefab.transform;
            Vector3 newPos = transform.position + (transform.right * 1.0f);
            Vector3 offset = new Vector3(5.0f, 0.0f, 0.0f);

            this.menuController.Menu.SetActive(false);
            this.menuController.subsectionMenu.SetActive(false);

            var pdf = Instantiate(this.pdfPrefab, newPos, transform.rotation);
            var newFloatingDocument = pdf.GetComponent<FloatingDocument>();

            var mergePdf = new MergePDF(floatingDocument, other, newFloatingDocument);
            mergePdf.Merge();

            // Find the game object called GameManager
            // GameObject gameManager = GameObject.Find("GameManager");
            // Get the GameManager component with the environment script
            EnvironmentInformation environment = this.GameManager.GetComponent<EnvironmentInformation>();
            environment.GetFloatingDocuments().Add(newFloatingDocument);

            // destroy the old floating documents
            var l = environment.GetFloatingDocuments();
            for (int j = l.Count - 1; j >= 0; j--) {
                FloatingDocument t = l[j];
                if (t == floatingDocument || t == other)
                {
                    environment.GetFloatingDocuments().Remove(t);
                    Destroy(t.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// The update method, which is called every frame. It updates the selected text depending on the selected document.
    /// </summary>
    void Update()
    {
        if (this.GameManager == null)
        {
            this.GameManager = GameObject.Find("GameManager");
        }

        var mergeManager = this.GameManager.GetComponent<MergeManager>();
        if (mergeManager.floatingDocument != null)
        {
            this.selected.text = "Selected: " + mergeManager.floatingDocument.pdfName;
        }
        else
        {
            this.selected.text = "Selected: Nothing";
        }
    }
}
