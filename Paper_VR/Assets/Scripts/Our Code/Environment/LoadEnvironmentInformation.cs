using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityGoogleDrive;

/// <summary>
/// This class is resposible for loading in the scene, using the environment information.
/// </summary>
public class LoadEnvironmentInformation : MonoBehaviour
{
    /// <summary>
    /// The convert PDF object that will be used to convert the PDF to sprites.
    /// </summary>
    public ConvertPDF convertPDF;

    /// <summary>
    /// The method that adds images to the floating document.
    /// </summary>
    /// <param name="pdf">The pdf we want to get the images from.</param>
    /// <returns>The IEnumerator.</returns>
    public virtual IEnumerator AddImages(FloatingDocument pdf)
    {
        // Download the file from the Google Drive
        GoogleDriveFiles.DownloadRequest request = new GoogleDriveFiles.DownloadRequest(pdf.pdfId);
        yield return request.Send();
        yield return this.convertPDF.AddImagesToFloatingDocument(request.ResponseData, pdf);
    }

    /// <summary>
    /// Loads the scene with indo.
    /// </summary>
    /// <param name="environmentInformation">The information of the environment./param>
    /// <returns>The IEnumerator.</returns>
    public IEnumerator LoadScene(EnvironmentInformation environmentInformation)
    {
        // forloop with index
        for (int i = 0; i < environmentInformation.GetFloatingDocuments().Count; i++)
        {
            FloatingDocument oldDoc = environmentInformation.GetFloatingDocuments()[i];
            // Create a new floating document
            GameObject instance = Instantiate(environmentInformation.docPrefab, oldDoc.position, oldDoc.rotation);
            FloatingDocument doc = instance.GetComponent<FloatingDocument>();

            doc.SetAttributes(
                oldDoc.position,
                oldDoc.rotation,
                oldDoc.scale,
                oldDoc.pdfId,
                oldDoc.pdfName,
                oldDoc.pages);

            // Replace the old floating document with the new one
            environmentInformation.GetFloatingDocuments()[i] = doc;
        }

        // Loop over all the floating documents and add the images
        foreach (FloatingDocument pdf in environmentInformation.GetFloatingDocuments())
        {
            yield return this.AddImages(pdf);
        }
    }

    /// <summary>
    /// This method subscribes to the sceneLoaded event at the start
    /// </summary>
    private void Start()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += this.OnSceneLoaded;
    }

    /// <summary>
    /// This method unsubscribes to the sceneLoaded event when destroyed
    /// </summary>
    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        this.Unsubscribe();
    }

    private void Unsubscribe()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= this.OnSceneLoaded;
    }

    /// <summary>
    /// When a new scene is loaded, the environment information is fetched and the scene will be loaded.
    /// </summary>
    /// <param name="scene">The scene that is loaded.</param>
    /// <param name="mode">The load mode of the scene.</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If the scene loaded is the environment, laod the scene
        if (scene.name.Equals("Environment"))
        {
            // Get the environment information from the selected scene
            EnvironmentInformation environmentInformation = this.gameObject.GetComponent<EnvironmentInformation>();
            this.StartCoroutine(this.LoadScene(environmentInformation));
        }
    }
}