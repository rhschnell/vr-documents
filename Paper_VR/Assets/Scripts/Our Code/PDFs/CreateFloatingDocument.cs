using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is used to create a floating document in the scene.
/// </summary>
public class CreateFloatingDocument : MonoBehaviour
{
    /// <summary>
    /// The prefab for the document.
    /// </summary>
    public GameObject docPrefab;

    private string imagePath = "Sprites";

    /// <summary>
    /// This method creates a new floating document in the scene.
    /// </summary>
    /// <param name="pos">The position of the floating document</param>
    /// <param name="pdfPath">The path where the pdf is stored</param>
    /// <param name="pdfName">The name of the pdf</param>
    /// <param name="pages">The pages as a list of numbers</param>
    /// <returns>A floating document</returns>
    public FloatingDocument CreateDocument(Vector3 pos, string pdfPath, string pdfName, int[] pages)
    {
        Debug.Log("Creating document");
        // Create a new canvas
        GameObject instance = Instantiate(this.docPrefab, pos, Quaternion.identity);

        // Update the child of the prefab to have to the correct first image
        string imagePath = this.imagePath + "\\" + pdfName + "\\" + pdfName + "-" + pages[0];
        var sprite = Resources.Load<Sprite>(imagePath);
        Debug.Log(sprite);
        instance.transform.GetChild(0).GetComponent<Image>().sprite = sprite;

        // Create a new floating document
        FloatingDocument doc = new FloatingDocument(instance, pdfPath, pdfName, pages);

        return doc;
    }
}
