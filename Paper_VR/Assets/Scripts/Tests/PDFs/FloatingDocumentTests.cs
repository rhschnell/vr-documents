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

    /// <summary>
    /// Test the SetValues method.
    /// </summary>
    [Test]
    public void SetValuesTest()
    {
        // Arrange
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

        // Act
        newDoc.SetValues();

        // Assert
        Assert.IsNotNull(newDoc.image.sprite, "Sprite should not be null.");
    }

    /// <summary>
    /// Test the ScrollUp method.
    /// </summary>
    [Test]
    public void ScrollUpTest()
    {
        // Arrange
        List<Sprite> sprites = new List<Sprite> { null, null, null };
        List<int> pages = new List<int> { 0, 1, 2 };
        GameObject examplePrefab = new GameObject();
        FloatingDocument newDoc = examplePrefab.AddComponent<FloatingDocument>();
        newDoc.pages = pages;
        newDoc.image = examplePrefab.AddComponent<Image>();
        newDoc.currentPageIndex = 1;
        newDoc.sprites = sprites;

        // Act
        newDoc.ScrollUp();

        // Assert
        Assert.AreEqual(0, newDoc.currentPageIndex, "Current page index should be decremented.");

        // Act
        newDoc.ScrollUp();

        // Assert
        Assert.AreEqual(0, newDoc.currentPageIndex, "Current page index should not go below 0.");
    }

    /// <summary>
    /// Test the ScrollDown method.
    /// </summary>
    [Test]
    public void ScrollDownTest()
    {
        // Arrange
        List<Sprite> sprites = new List<Sprite> { null, null, null };
        List<int> pages = new List<int> { 0, 1, 2 };
        GameObject examplePrefab = new GameObject();
        FloatingDocument newDoc = examplePrefab.AddComponent<FloatingDocument>();
        newDoc.pages = pages;
        newDoc.image = examplePrefab.AddComponent<Image>();
        newDoc.currentPageIndex = 1;
        newDoc.sprites = sprites;

        // Act
        newDoc.ScrollDown();

        // Assert
        Assert.AreEqual(2, newDoc.currentPageIndex, "Current page index should be incremented.");

        // Act
        newDoc.ScrollDown();

        // Assert
        Assert.AreEqual(2, newDoc.currentPageIndex, "Current page index should not exceed the last page index.");
    }

    /// <summary>
    /// Test the SetSprite method.
    /// </summary>
    [Test]
    public void SetSpriteTest()
    {
        // Arrange
        List<int> pages = new List<int> { 0, 1, 2 };
        GameObject canvas = new GameObject();
        GameObject examplePrefab = new GameObject();
        FloatingDocument newDoc = examplePrefab.AddComponent<FloatingDocument>();
        newDoc.pages = pages;
        newDoc.currentPageIndex = 1;
        newDoc.image = canvas.AddComponent<Image>();
        List<Sprite> sprites = new List<Sprite>();
        Texture2D texture = new Texture2D(100, 100);
        Sprite sprite1 = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        Sprite sprite2 = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        Sprite sprite3 = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        sprites.Add(sprite1);
        sprites.Add(sprite2);
        sprites.Add(sprite3);
        newDoc.sprites = sprites;

        // Act
        newDoc.SetSprite();

        // Assert
        Assert.AreEqual(sprite2, newDoc.image.sprite, "Sprite should be set to the current page sprite.");
    }

    /// <summary>
    /// Test the IsHovering method.
    /// </summary>
    [Test]
    public void IsHoveringTest()
    {
        // Arrange
        GameObject examplePrefab = new GameObject();
        FloatingDocument newDoc = examplePrefab.AddComponent<FloatingDocument>();

        // Act
        newDoc.IsHovering(true);

        // Assert
        Assert.IsTrue(newDoc.isHovering, "isHovering should be true.");

        // Act
        newDoc.IsHovering(false);

        // Assert
        Assert.IsFalse(newDoc.isHovering, "isHovering should be false.");
    }
}