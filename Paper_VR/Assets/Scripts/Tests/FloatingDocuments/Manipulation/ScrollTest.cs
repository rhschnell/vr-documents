using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class containing tests for the scroll functionality.
/// </summary>
public class ScrollTest : MonoBehaviour
{
    private MenuController documentMenu;
    private GameObject originalPdfCanvas;
    private GameObject gameManager;
    private Scroll scroll;

    private FloatingDocument floatingDocument;

    /// <summary>
    /// Set up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        GameObject gameManager = new GameObject("GameManager");
        Assert.NotNull(gameManager, "Could not create gameManager : GameManager");

        // Create a new GameObject to act as the PDFCanvas
        this.originalPdfCanvas = new GameObject("PDFCanvas");
        this.originalPdfCanvas.AddComponent<FloatingDocument>();

        this.scroll = new GameObject().AddComponent<Scroll>();

        this.floatingDocument = this.originalPdfCanvas.AddComponent<FloatingDocument>();
        Assert.NotNull(this.floatingDocument, "Could not create floatingDocument: FloatingDocument");
        this.floatingDocument.mergeButton = new GameObject();
        this.floatingDocument.pdfId = "some/path/to/original.pdf";
        this.floatingDocument.pages = new List<int> { 0, 1, 2, 3, 4 };
        this.floatingDocument.image = new GameObject().AddComponent<Image>();
        string name = "test";
        string id = "123";
        this.floatingDocument.exportPages = new List<Tuple<string, string, int>>
                {
                    new Tuple<string, string, int>(name, id, 1),
                    new Tuple<string, string, int>(name, id, 2),
                    new Tuple<string, string, int>(name, id, 3),
                    new Tuple<string, string, int>(name, id, 4),
                    new Tuple<string, string, int>(name, id, 5),
                };
        this.floatingDocument.sprites = new List<Sprite> {
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
        };

        this.scroll.floatingDocument = this.floatingDocument;
        gameManager.AddComponent<EnvironmentController>();
    }

    /// <summary>
    /// Clean up the test environment.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        // Destroy all GameObjects
        Destroy(this.gameManager);
        Destroy(this.documentMenu);
        Destroy(this.scroll);
        Destroy(this.originalPdfCanvas);
        Destroy(this.floatingDocument);
    }

    /// <summary>
    /// When called, we scroll up.
    /// </summary>
    [Test]
    public void ScrollUpTest()
    {
        // check if page index changes
        this.floatingDocument.currentPageIndex = 2;
        this.scroll.ScrollUp();
        Assert.AreEqual(1, this.floatingDocument.currentPageIndex);
    }

    /// <summary>
    /// When called, we scroll down.
    /// </summary>
    [Test]
    public void ScrollDownTest()
    {
        // check if page index changes
        this.floatingDocument.currentPageIndex = 2;
        this.scroll.ScrollDown();
        Assert.AreEqual(3, this.floatingDocument.currentPageIndex);
    }

    /// <summary>
    /// When called, is hovering.
    /// </summary>
    [Test]
    public void IsHoveringTest()
    {
        // check if hovering changes
        this.floatingDocument.currentPageIndex = 2;
        this.scroll.IsHovering(true);
        Assert.IsTrue(this.scroll.isHovering);
    }

    /// <summary>
    /// When called, is holding.
    /// </summary>
    [Test]
    public void IsHoldingTest()
    {
        // check if holding changes
        this.floatingDocument.currentPageIndex = 2;
        this.scroll.IsHolding(true);

        Assert.IsTrue(this.scroll.isHolding);
    }

    /// <summary>
    /// When called, update with scroll up.
    /// </summary>
    [Test]
    public void UpdateTest()
    {
        this.floatingDocument.currentPageIndex = 2;
        this.scroll.joystickValue = new Vector2(0, 1);
        this.scroll.available = true;
        this.scroll.isHovering = true;
        this.scroll.isHolding = false;

        // using the update method in the class
        var methodInfo = typeof(Scroll).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
        object result = methodInfo.Invoke(this.scroll, null);

        Assert.AreEqual(1, this.floatingDocument.currentPageIndex);
    }

    /// <summary>
    /// When called, update with scroll down.
    /// </summary>
    [Test]
    public void UpdateTest2()
    {
        // using the update method in the class
        this.floatingDocument.currentPageIndex = 2;
        this.scroll.joystickValue = new Vector2(0, -1);
        this.scroll.available = true;
        this.scroll.isHovering = true;
        this.scroll.isHolding = false;

        // using the update method in the class
        var methodInfo = typeof(Scroll).GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Instance);
        methodInfo.Invoke(this.scroll, null);

        Assert.AreEqual(3, this.floatingDocument.currentPageIndex);
    }
}