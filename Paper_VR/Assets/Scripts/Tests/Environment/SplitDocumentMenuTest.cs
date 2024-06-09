using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GameObject = UnityEngine.GameObject;

public class SplitDocumentMenuTest
{
    private GameObject originalPdfCanvas;
    private GameObject manipulationMenu;
    private GameObject splitMenu;
    private GameObject gameManager;

    private TMP_Text startPageNumber;
    private TMP_Text endPageNumber;
    private EnvironmentInformation envInfo;

    /// <summary>
    /// Set up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // Create a new GameObject to act as the PDFCanvas
        this.originalPdfCanvas = new GameObject("PDFCanvas");
        this.manipulationMenu = new GameObject("Menu");
        this.splitMenu = new GameObject("SplitMenu");
        this.manipulationMenu.transform.SetParent(this.originalPdfCanvas.transform);
        this.splitMenu.transform.SetParent(this.originalPdfCanvas.transform);

        this.startPageNumber = new GameObject("StartPageNumber").AddComponent<TextMeshPro>();
        this.endPageNumber = new GameObject("EndPageNumber").AddComponent<TextMeshPro>();

        var floatingDocument = this.originalPdfCanvas.AddComponent<FloatingDocument>();
        floatingDocument.pdfId = "some/path/to/original.pdf";
        floatingDocument.pages = new List<int> { 0, 1, 2, 3, 4 };

        Texture2D texture = new Texture2D(1, 1);
        floatingDocument.sprites = new List<Sprite>
        {
            Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero),
            Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero),
            Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero),
            Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero),
            Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero),
        };
        floatingDocument.image = originalPdfCanvas.AddComponent<Image>();
        floatingDocument.currentPageIndex = 2;

        this.gameManager = new GameObject("GameManager");
        this.envInfo = this.gameManager.AddComponent<EnvironmentInformation>();
        this.envInfo.GetFloatingDocuments().Add(floatingDocument);
    }

    /// <summary>
    /// Test for start plus button
    /// </summary>
    [Test]
    public void ClickOnStartPlusButtonSimplePass()
    {
        var splitDocumentMenu = new GameObject("SplitMenu").AddComponent<SplitDocumentMenu>();
        splitDocumentMenu.PdfPrefab = this.originalPdfCanvas;
        splitDocumentMenu.Menu = this.manipulationMenu;
        splitDocumentMenu.SplitMenu = this.splitMenu;
        splitDocumentMenu.StartPageNumberText = this.startPageNumber;
        splitDocumentMenu.EndPageNumberText = this.endPageNumber;
        splitDocumentMenu.StartPageNumber = 5;
        splitDocumentMenu.EndPageNumber = 7;

        splitDocumentMenu.ClickOnStartPlusButton();
        Assert.AreEqual(6, splitDocumentMenu.StartPageNumber);
    }

    /// <summary>
    /// Test for start min button
    /// </summary>
    [Test]
    public void ClickOnStartMinButtonSimplePass()
    {
        var splitDocumentMenu = new GameObject("SplitMenu").AddComponent<SplitDocumentMenu>();
        splitDocumentMenu.PdfPrefab = this.originalPdfCanvas;
        splitDocumentMenu.Menu = this.manipulationMenu;
        splitDocumentMenu.SplitMenu = this.splitMenu;
        splitDocumentMenu.StartPageNumberText = this.startPageNumber;
        splitDocumentMenu.EndPageNumberText = this.endPageNumber;
        splitDocumentMenu.StartPageNumber = 5;

        splitDocumentMenu.ClickOnStartMinButton();
        Assert.AreEqual(4, splitDocumentMenu.StartPageNumber);
    }

    /// <summary>
    /// Test for end plus button
    /// </summary>
    [Test]
    public void ClickOnEndPlusButtonSimplePass()
    {
        var splitDocumentMenu = new GameObject("SplitMenu").AddComponent<SplitDocumentMenu>();
        splitDocumentMenu.PdfPrefab = this.originalPdfCanvas;
        splitDocumentMenu.Menu = this.manipulationMenu;
        splitDocumentMenu.SplitMenu = this.splitMenu;
        splitDocumentMenu.StartPageNumberText = this.startPageNumber;
        splitDocumentMenu.EndPageNumberText = this.endPageNumber;
        splitDocumentMenu.StartPageNumber = 2;
        splitDocumentMenu.EndPageNumber = 5;
        splitDocumentMenu.MaxPage = 6;
        splitDocumentMenu.ClickOnEndPlusButton();
        Assert.AreEqual(6, splitDocumentMenu.EndPageNumber);
        splitDocumentMenu.ClickOnEndPlusButton();
        Assert.AreEqual(6, splitDocumentMenu.EndPageNumber);
    }

    /// <summary>
    /// Test for end min button
    /// </summary>
    [Test]
    public void ClickOnEndMinButtonSimplePass()
    {
        var splitDocumentMenu = new GameObject("SplitMenu").AddComponent<SplitDocumentMenu>();
        splitDocumentMenu.PdfPrefab = this.originalPdfCanvas;
        splitDocumentMenu.Menu = this.manipulationMenu;
        splitDocumentMenu.SplitMenu = this.splitMenu;
        splitDocumentMenu.StartPageNumberText = this.startPageNumber;
        splitDocumentMenu.EndPageNumberText = this.endPageNumber;
        splitDocumentMenu.EndPageNumber = 5;

        splitDocumentMenu.ClickOnEndMinButton();
        Assert.AreEqual(4, splitDocumentMenu.EndPageNumber);
    }

    /// <summary>
    /// Test for start slider
    /// </summary>
    [Test]
    public void StartSliderListenersTest()
    {
        var splitDocumentMenu = new GameObject("SplitMenu").AddComponent<SplitDocumentMenu>();
        splitDocumentMenu.PdfPrefab = this.originalPdfCanvas;
        splitDocumentMenu.Menu = this.manipulationMenu;
        splitDocumentMenu.SplitMenu = this.splitMenu;
        splitDocumentMenu.StartPageNumberText = this.startPageNumber;
        splitDocumentMenu.EndPageNumberText = this.endPageNumber;
        splitDocumentMenu.StartPageNumber = 1;
        splitDocumentMenu.EndPageNumber = 5;

        // Simulate changing the value of the end slider
        splitDocumentMenu.StartSlider = new GameObject().AddComponent<Slider>(); // Dummy slider for testing
        splitDocumentMenu.EndSlider = new GameObject().AddComponent<Slider>(); // Dummy slider for testing
        splitDocumentMenu.SetupSliderListeners();
        splitDocumentMenu.StartSlider.onValueChanged.Invoke(0.20f); // Simulate slider value change

        // Check if the end page number was updated correctly
        Assert.AreEqual(2, splitDocumentMenu.StartPageNumber);
    }

    /// <summary>
    /// Test for end slider
    /// </summary>
    [Test]
    public void EndSliderListenersTest()
    {
        var splitDocumentMenu = new GameObject("SplitMenu").AddComponent<SplitDocumentMenu>();
        splitDocumentMenu.PdfPrefab = this.originalPdfCanvas;
        splitDocumentMenu.Menu = this.manipulationMenu;
        splitDocumentMenu.SplitMenu = this.splitMenu;
        splitDocumentMenu.StartPageNumberText = this.startPageNumber;
        splitDocumentMenu.EndPageNumberText = this.endPageNumber;
        splitDocumentMenu.StartPageNumber = 1;
        splitDocumentMenu.EndPageNumber = 5;
        splitDocumentMenu.MaxPage = 5;

        // Simulate changing the value of the end slider
        splitDocumentMenu.StartSlider = new GameObject().AddComponent<Slider>(); // Dummy slider for testing
        splitDocumentMenu.EndSlider = new GameObject().AddComponent<Slider>(); // Dummy slider for testing
        splitDocumentMenu.SetupSliderListeners();
        splitDocumentMenu.EndSlider.onValueChanged.Invoke(0.50f); // Simulate slider value change

        // Check if the end page number was updated correctly
        Assert.AreEqual(3, splitDocumentMenu.EndPageNumber);
    }

    /// <summary>
    /// Test for toggling the menu
    /// </summary>
    [Test]
    public void ToggleMenuSimplePass()
    {
        var splitDocumentMenu = new GameObject("SplitMenu").AddComponent<SplitDocumentMenu>();
        splitDocumentMenu.PdfPrefab = this.originalPdfCanvas;
        splitDocumentMenu.Menu = this.manipulationMenu;
        splitDocumentMenu.SplitMenu = this.splitMenu;
        splitDocumentMenu.StartPageNumberText = this.startPageNumber;
        splitDocumentMenu.EndPageNumberText = this.endPageNumber;
        splitDocumentMenu.EndPageNumber = 5;

        splitDocumentMenu.ToggleMenu();
        Assert.False(this.splitMenu.activeSelf);
        splitDocumentMenu.ToggleMenu();
        Assert.True(this.splitMenu.activeSelf);
        Assert.AreEqual(5, splitDocumentMenu.MaxPage);
    }

    /// <summary>
    /// Test for pressing the split button
    /// </summary>
    [Test]
    public void OnClickSplitSimplePass()
    {
        var splitDocumentMenu = new GameObject("SplitMenu").AddComponent<SplitDocumentMenu>();
        splitDocumentMenu.PdfPrefab = this.originalPdfCanvas;
        splitDocumentMenu.Menu = this.manipulationMenu;
        splitDocumentMenu.SplitMenu = this.splitMenu;
        splitDocumentMenu.StartPageNumberText = this.startPageNumber;
        splitDocumentMenu.EndPageNumberText = this.endPageNumber;
        splitDocumentMenu.EndPageNumber = 3;
        splitDocumentMenu.StartPageNumber = 2;

        var initialDocumentCount = envInfo.GetFloatingDocuments().Count;

        splitDocumentMenu.OnClickSplit();

        // Check that the split and manipulation menus are deactivated
        Assert.IsFalse(this.splitMenu.activeSelf);
        Assert.IsFalse(this.manipulationMenu.activeSelf);

        var newDocument = envInfo.GetFloatingDocuments()[initialDocumentCount - 1];
        Assert.IsNotNull(newDocument);
        Assert.AreNotSame(this.originalPdfCanvas, newDocument);

        // Check that the new document has the correct pages
        Assert.AreEqual(3, newDocument.pages.Count);
        Assert.AreEqual(0, newDocument.pages[0]);

        // Check that the original document has the remaining pages
        var originalDocument = envInfo.GetFloatingDocuments()[0];
        Assert.AreEqual(3, originalDocument.pages.Count);
        Assert.AreEqual(1, originalDocument.pages[1]);
    }

    /// <summary>
    /// Test for splitting an individual page
    /// </summary>
    [Test]
    public void OnClickSplitIndividualPageTest()
    {
        var splitDocumentMenu = new GameObject("SplitMenu").AddComponent<SplitDocumentMenu>();
        splitDocumentMenu.PdfPrefab = this.originalPdfCanvas;
        splitDocumentMenu.Menu = this.manipulationMenu;
        splitDocumentMenu.SplitMenu = this.splitMenu;
        splitDocumentMenu.StartPageNumberText = this.startPageNumber;
        splitDocumentMenu.EndPageNumberText = this.endPageNumber;
        splitDocumentMenu.EndPageNumber = 4;
        splitDocumentMenu.StartPageNumber = 2;

        var initialDocumentCount = envInfo.GetFloatingDocuments().Count;

        splitDocumentMenu.OnClickSplitIndividualPage();

        // Check that the split and manipulation menus are deactivated
        Assert.IsFalse(this.splitMenu.activeSelf);
        Assert.IsFalse(this.manipulationMenu.activeSelf);

        var newDocument = envInfo.GetFloatingDocuments()[initialDocumentCount - 1];
        Assert.IsNotNull(newDocument);
        Assert.AreNotSame(this.originalPdfCanvas, newDocument);

        // Check that the new document has the correct pages
        Assert.AreEqual(0, newDocument.pages[0]);

        // Check that the original document has the remaining pages
        var originalDocument = envInfo.GetFloatingDocuments()[0];
        Assert.AreEqual(4, originalDocument.pages.Count);
        Assert.AreEqual(0, originalDocument.pages[0]);
        Assert.AreEqual(1, originalDocument.pages[1]);
    }

    /// <summary>
    /// Clean up the test environment.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        // Destroy all GameObjects
        UnityEngine.Object.Destroy(this.manipulationMenu);
        UnityEngine.Object.Destroy(this.splitMenu);
        UnityEngine.Object.Destroy(this.originalPdfCanvas);
        UnityEngine.Object.Destroy(this.startPageNumber);
        UnityEngine.Object.Destroy(this.endPageNumber);
        UnityEngine.Object.DestroyImmediate(this.gameManager);
    }
}
