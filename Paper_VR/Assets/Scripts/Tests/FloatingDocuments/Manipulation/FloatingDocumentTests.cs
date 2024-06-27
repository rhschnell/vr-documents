using System;
using System.Collections.Generic;
using System.Reflection;
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
        newDoc.pdfId = "googledrive";
        newDoc.pdfName = "examplename";
        newDoc.pages = pages;
        newDoc.currentPageIndex = 1;

        FloatingDocument doc = examplePrefab.GetComponent<FloatingDocument>();

        // Act
        Assert.AreEqual("googledrive", doc.pdfId);
        Assert.AreEqual("examplename", doc.pdfName);
        Assert.AreEqual(pages, doc.pages);
        Assert.AreEqual(1, doc.currentPageIndex);
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
        FloatingDocument floatingDocument = examplePrefab.AddComponent<FloatingDocument>();
        floatingDocument.pages = pages;
        floatingDocument.pdfId = "googledrive";
        floatingDocument.pdfName = "examplename";

        // Act
        string result = floatingDocument.ToString();

        // Assert
        Assert.AreEqual("PDF Path: googledrive, PDF Name: examplename, Number of Pages: 3", result);
    }

    /// <summary>
    /// This test checks that the Equals method works correctly.
    /// </summary>
    [Test]
    public void EqualsTest()
    {
        // Arrange
        List<int> pages = new List<int> { 1, 2, 3 };
        List<int> pages2 = new List<int> { 1, 2, 3 };
        List<Tuple<string, string, int>> exportPages =
            new List<Tuple<string, string, int>> { new Tuple<string, string, int>("googledrive", "examplename", 1) };
        List<Tuple<string, string, int>> exportPages2 =
            new List<Tuple<string, string, int>> { new Tuple<string, string, int>("googledrive", "examplename", 1) };
        GameObject examplePrefab1 = new GameObject();
        FloatingDocument floatingDocument1 = examplePrefab1.AddComponent<FloatingDocument>();
        floatingDocument1.pages = pages;
        floatingDocument1.pdfId = "googledrive";
        floatingDocument1.pdfName = "examplename";
        floatingDocument1.exportPages = exportPages;

        GameObject examplePrefab2 = new GameObject();
        FloatingDocument floatingDocument2 = examplePrefab2.AddComponent<FloatingDocument>();
        floatingDocument2.pages = pages2;
        floatingDocument2.pdfId = "googledrive";
        floatingDocument2.pdfName = "examplename";
        floatingDocument2.exportPages = exportPages2;

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
        FloatingDocument floatingDocument = examplePrefab.AddComponent<FloatingDocument>();
        floatingDocument.pages = pages;
        floatingDocument.pdfId = "googledrive";
        floatingDocument.pdfName = "examplename";

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
        examplePrefab.AddComponent<RectTransform>();
        newDoc.pdfId = "googledrive";
        newDoc.pdfName = "examplename";
        newDoc.image = canvas.AddComponent<Image>();
        newDoc.currentPageIndex = 0;
        newDoc.scale = new Vector3(0.1f, 0.1f, 0.1f);
        newDoc.subMenu = new GameObject("MainMenu");
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

    /// <summary>
    /// Test the IsHovering method.
    /// </summary>
    [Test]
    public void IsHoldingTest()
    {
        // Arrange
        GameObject examplePrefab = new GameObject();
        FloatingDocument newDoc = examplePrefab.AddComponent<FloatingDocument>();

        // Act
        newDoc.IsHolding(true);

        // Assert
        Assert.IsTrue(newDoc.isHolding, "isHolding should be true.");

        // Act
        newDoc.IsHolding(false);

        // Assert
        Assert.IsFalse(newDoc.isHolding, "isHolding should be false.");
    }

    /// <summary>
    /// When called, update with scroll up.
    /// </summary>
    [Test]
    public void UpdateTest()
    {
        GameObject gameManager = new GameObject("GameManager");
        Assert.NotNull(gameManager, "Could not create gameManager : GameManager");

        // Create a new GameObject to act as the PDFCanvas
        var originalPdfCanvas = new GameObject("PDFCanvas");
        originalPdfCanvas.AddComponent<FloatingDocument>();

        var floatingDocument = originalPdfCanvas.AddComponent<FloatingDocument>();
        Assert.NotNull(floatingDocument, "Could not create floatingDocument: FloatingDocument");
        floatingDocument.mergeButton = new GameObject();
        floatingDocument.pdfId = "some/path/to/original.pdf";
        floatingDocument.pages = new List<int> { 0, 1, 2, 3, 4 };
        floatingDocument.image = new GameObject().AddComponent<UnityEngine.UI.Image>();
        string name = "test";
        string id = "123";
        floatingDocument.exportPages = new List<Tuple<string, string, int>>
        {
            new Tuple<string, string, int>(name, id, 1),
            new Tuple<string, string, int>(name, id, 2),
            new Tuple<string, string, int>(name, id, 3),
            new Tuple<string, string, int>(name, id, 4),
            new Tuple<string, string, int>(name, id, 5),
        };
        floatingDocument.sprites = new List<Sprite> {
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
        };

        gameManager.AddComponent<EnvironmentController>();

        floatingDocument.currentPageIndex = 2;
        floatingDocument.joystickValue = new Vector2(0, 1);
        floatingDocument.available = true;
        floatingDocument.isHovering = true;
        floatingDocument.isHolding = false;

        Vector2 testJoystickValue = new Vector2(0.5f, 0.5f);
        var methodInfo = typeof(FloatingDocument).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
        methodInfo.Invoke(floatingDocument, null);

        Assert.AreEqual(1, floatingDocument.currentPageIndex);

        // Destroy all GameObjects
        Destroy(gameManager);
        Destroy(originalPdfCanvas);
        Destroy(floatingDocument);
    }

    /// <summary>
    /// When called, update with scroll down.
    /// </summary>
    [Test]
    public void UpdateTest2()
    {
        GameObject gameManager = new GameObject("GameManager");
        Assert.NotNull(gameManager, "Could not create gameManager : GameManager");

        // Create a new GameObject to act as the PDFCanvas
        var originalPdfCanvas = new GameObject("PDFCanvas");
        originalPdfCanvas.AddComponent<FloatingDocument>();

        var floatingDocument = originalPdfCanvas.AddComponent<FloatingDocument>();
        Assert.NotNull(floatingDocument, "Could not create floatingDocument: FloatingDocument");
        floatingDocument.mergeButton = new GameObject();
        floatingDocument.pdfId = "some/path/to/original.pdf";
        floatingDocument.pages = new List<int> { 0, 1, 2, 3, 4 };
        floatingDocument.image = new GameObject().AddComponent<UnityEngine.UI.Image>();
        string name = "test";
        string id = "123";
        floatingDocument.exportPages = new List<Tuple<string, string, int>>
        {
            new Tuple<string, string, int>(name, id, 1),
            new Tuple<string, string, int>(name, id, 2),
            new Tuple<string, string, int>(name, id, 3),
            new Tuple<string, string, int>(name, id, 4),
            new Tuple<string, string, int>(name, id, 5),
        };
        floatingDocument.sprites = new List<Sprite> {
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
        };

        gameManager.AddComponent<EnvironmentController>();

        floatingDocument.currentPageIndex = 2;
        floatingDocument.joystickValue = new Vector2(0, -1);
        floatingDocument.available = true;
        floatingDocument.isHovering = true;
        floatingDocument.isHolding = false;

        Vector2 testJoystickValue = new Vector2(0.5f, 0.5f);
        var methodInfo = typeof(FloatingDocument).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
        object result = methodInfo.Invoke(floatingDocument, null);

        Assert.AreEqual(3, floatingDocument.currentPageIndex);

        // Destroy all GameObjects
        Destroy(gameManager);
        Destroy(originalPdfCanvas);
        Destroy(floatingDocument);
    }
}