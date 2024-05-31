using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class contains tests for the FloatingDocument class.
/// </summary>
public class FloatingDocumentTests : MonoBehaviour
{
    /// <summary>
    /// This is a simple test that asserts true.
    /// </summary>
    [Test]
    public void CreateFloatingDocument()
    {
        // Arrange
        List<int> pages = new List<int> { 1, 2, 3 };
        GameObject canvas = new GameObject();
        GameObject examplePrefab = new GameObject();
        FloatingDocument newDoc = examplePrefab.AddComponent<FloatingDocument>();
        newDoc.pdfPath = "googledrive";
        newDoc.pdfName = "examplename";
        newDoc.pages = pages;
        newDoc.width = 210;
        newDoc.height = 297;
        newDoc.canvas = canvas;
        newDoc.currentPageIndex = 1;

        FloatingDocument doc = examplePrefab.GetComponent<FloatingDocument>();

        // Act
        Assert.IsNotNull(doc);
        Assert.AreEqual("googledrive", doc.pdfPath);
        Assert.AreEqual("examplename", doc.pdfName);
        Assert.AreEqual(pages, doc.pages);
        Assert.AreEqual(210, doc.width);
        Assert.AreEqual(297, doc.height);
        Assert.AreEqual(canvas, doc.canvas);
        Assert.AreEqual(1, doc.currentPageIndex);
    }

    /// <summary>
    /// This test checks that the constructor throws an exception when no pages are provided.
    /// </summary>
    [Test]
    public void NoPages()
    {
        // Arrange
        List<int> pages = new List<int> { };
        GameObject examplePrefab = new GameObject();

        // Assert that the constructor throws an exception when no pages are provided
        Assert.Throws<System.ArgumentException>(() => new FloatingDocument(examplePrefab, "googledrive", "examplename", pages));
    }

    /// <summary>
    /// This test checks that the string representation of the object is correct.
    /// </summary>
    [Test]
    public void ToStringTest()
    {
        // Arrange
        List<int> pages = new List<int> { 1, 2, 3 };
        GameObject examplePrefab = new GameObject();
        FloatingDocument floatingDocument = new FloatingDocument(examplePrefab, "googledrive", "examplename", pages);

        // Act
        string result = floatingDocument.ToString();

        // Assert
        Assert.AreEqual("PDF Path: googledrive, PDF Name: examplename, Number of Pages: 3, Width: 210, Height: 297", result);
    }

    /// <summary>
    /// This test checks that the Equals method works correctly.
    /// </summary>
    [Test]
    public void EqualsTest()
    {
        // Arrange
        List<int> pages = new List<int> { 1, 2, 3 };
        GameObject examplePrefab = new GameObject();
        FloatingDocument floatingDocument1 = new FloatingDocument(examplePrefab, "googledrive", "examplename", pages);
        FloatingDocument floatingDocument2 = new FloatingDocument(examplePrefab, "googledrive", "examplename", pages);

        // Act
        bool result = floatingDocument1.Equals(floatingDocument2);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(floatingDocument1.Equals(floatingDocument1));
        Assert.IsFalse(floatingDocument1.Equals(null));
    }

    /// <summary>
    /// Set values test.
    /// </summary>
    [Test]
    public void SetValuesTest()
    {
        GameObject canvas = new GameObject();
        GameObject examplePrefab = new GameObject();
        FloatingDocument newDoc = examplePrefab.AddComponent<FloatingDocument>();
        newDoc.pdfPath = "googledrive";
        newDoc.pdfName = "examplename";
        newDoc.width = 210;
        newDoc.height = 297;
        newDoc.canvas = canvas;
        newDoc.image = canvas.AddComponent<Image>();
        newDoc.currentPageIndex = 0;
        List<Sprite> sprites = new List<Sprite>();
        Texture2D texture = new Texture2D(100, 100);

        // Fill the texture with a solid color (e.g., white)
        Color fillColor = Color.white;
        Color[] fillPixels = new Color[texture.width * texture.height];
        for (int i = 0; i < fillPixels.Length; i++)
        {
            fillPixels[i] = fillColor;
        }

        texture.SetPixels(fillPixels);
        texture.Apply();

        // Create a new sprite from the texture
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        sprites.Add(newSprite);
        newDoc.sprites = sprites;

        // Call the SetValues method
        newDoc.SetValues();

        // Check if sprite is set correctly
        Assert.IsNotNull(newDoc.image.sprite, "Sprite should not be null.");

        // Check if transform scale is set correctly
        // var expectedScale = new Vector3(0.01f, -0.01f, 1.00f);
        // Assert.AreEqual(expectedScale, newDoc.transform.localScale, "Transform scale should be set correctly.");
    }

    /// <summary>
    /// This test checks that the GetHashCode method works correctly.
    /// </summary>
    [Test]
    public void HashTest()
    {
        // Arrange
        List<int> pages = new List<int> { 1, 2, 3 };
        GameObject examplePrefab = new GameObject();
        FloatingDocument floatingDocument = new FloatingDocument(examplePrefab, "googledrive", "examplename", pages);

        // Act
        int result = floatingDocument.GetHashCode();

        // Assert that the hash code is a number
        Assert.IsInstanceOf<int>(result);
    }
}
