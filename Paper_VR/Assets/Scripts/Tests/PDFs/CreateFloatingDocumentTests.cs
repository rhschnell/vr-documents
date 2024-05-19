using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class contains tests for creating the FloatingDocuments in the scene.
/// </summary>
public class CreateFloatingDocumentTests : MonoBehaviour
{
    /// <summary>
    /// This is a simple test that asserts true.
    /// </summary>
    [Test]
    public void CreateFloatingDocument()
    {
        // Arrange
        CreateFloatingDocument createFloatingDocument = new GameObject().AddComponent<CreateFloatingDocument>();

        // Create a new prefab for the document
        GameObject docPrefab = new GameObject();
        docPrefab.AddComponent<Canvas>();

        // Add an image to the canvas as a child
        GameObject image = new GameObject();
        image.AddComponent<Image>();
        image.transform.SetParent(docPrefab.transform);

        // Set the prefab for the document
        createFloatingDocument.docPrefab = docPrefab;

        // Act
        FloatingDocument doc = createFloatingDocument.CreateDocument(new Vector3(0, 0, 0), "googledrive", "exam_noanswers2", new int[3] { 1, 2, 3 });

        // Assert that the document was created
        Assert.AreEqual("googledrive", doc.pdfPath);
        Assert.AreEqual("exam_noanswers2", doc.pdfName);
        Assert.AreEqual(3, doc.pages.Length);
        Assert.AreEqual(210, doc.width);
        Assert.AreEqual(297, doc.height);
        Assert.AreEqual(1, doc.currentPage);

        // Assert that the document was created in the scene
        Assert.IsNotNull(doc.canvas);
        // Assert that the image of the document is set correctly
        Assert.AreEqual("exam_noanswers2-1", doc.canvas.transform.GetChild(0).GetComponent<Image>().sprite.name);
    }
}
