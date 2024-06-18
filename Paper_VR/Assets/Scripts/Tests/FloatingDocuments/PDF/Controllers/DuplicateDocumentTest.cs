using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

/// <summary>
/// This class contains tests for the DuplicateController class.
/// </summary>
public class DuplicateDocumentTest : MonoBehaviour
{
    private GameObject originalPdfCanvas;
    private GameObject menu;
    private GameObject subPanel;
    private GameObject duplicateMenu;
    private GameObject gameManager;
    private EnvironmentController envInfo;
    private Image floatingDocumentImage;
    private Slider startSlider;
    private Slider endSlider;

    /// <summary>
    /// Set up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // Create a new GameObejct to act as the PDFCanvas
        this.originalPdfCanvas = new GameObject("PDFCanvas");
        this.menu = new GameObject("Menu");
        this.subPanel = new GameObject("SubPanel");
        this.duplicateMenu = new GameObject("DuplicateMenu");
        this.menu.transform.SetParent(this.originalPdfCanvas.transform);
        this.subPanel.transform.SetParent(this.originalPdfCanvas.transform);
        this.duplicateMenu.transform.SetParent(this.originalPdfCanvas.transform);
        this.startSlider = new GameObject("StartSlider").AddComponent<Slider>();
        this.endSlider = new GameObject("EndSlider").AddComponent<Slider>();

        // Setup a floating document with 5 pages in the environment, this document is used for testing the duplication of a single page and the subection of a page
        var floatingDocument = this.originalPdfCanvas.AddComponent<FloatingDocument>();
        floatingDocument.pdfId = "some/path/to/original.pdf";
        floatingDocument.pages = new List<int> { 0, 1, 2, 3, 4 };
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

        Texture2D texture = new Texture2D(1, 1);
        floatingDocument.sprites = new List<Sprite>
        {
            Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero),
            Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero),
            Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero),
            Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero),
            Sprite.Create(texture, new Rect(0, 0, 1, 1), Vector2.zero),
        };
        floatingDocument.currentPageIndex = 2;

        this.floatingDocumentImage = this.originalPdfCanvas.AddComponent<Image>();
        floatingDocument.image = this.floatingDocumentImage;

        this.gameManager = new GameObject("GameManager");
        this.envInfo = this.gameManager.AddComponent<EnvironmentController>();
        this.envInfo.GetFloatingDocuments().Add(floatingDocument);
    }

    /// <summary>
    /// Clean up the test environment.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        // Destroy all GameObjects
        Destroy(this.originalPdfCanvas);
        Destroy(this.menu);
        Destroy(this.subPanel);
        Destroy(this.duplicateMenu);
        Destroy(this.gameManager);
    }

    /// <summary>
    /// A test that confirms that the document is duplicated.
    /// </summary>
    [Test]
    public void TestDuplicatePDFCanvas()
    {
        // Create the DuplicateController component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();

        var initalFloatingDocument = this.originalPdfCanvas.GetComponent<FloatingDocument>();
        this.envInfo.GetFloatingDocuments().Add(initalFloatingDocument);

        duplicateDocument.DuplicatePDFCanvas();

        Assert.AreEqual(2, this.envInfo.GetFloatingDocuments().Count, "The duplicated PDF should be added to the environment.");
    }

    /// <summary>
    /// Tests if the log error is written to the log when trying to duplicate a floating document, but the prefab is null
    /// </summary>
    [Test]
    public void TestDuplicatePDFCanvasPrefabIsNull()
    {
        // Create the DuplicateController component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.pdfPrefab = null;

        LogAssert.Expect(LogType.Error, "The PDFCanvas prefab is null");

        duplicateDocument.DuplicatePDFCanvas();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }

    /// <summary>
    /// A test that confirms that the shown page of the document is duplicated.
    /// </summary>
    [Test]
    public void TestDuplicatePagePDFCanvas()
    {
        // Create the DuplicateController component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();

        duplicateDocument.DuplicatePagePDF();

        Assert.AreEqual(2, this.envInfo.GetFloatingDocuments().Count, "The duplicated PDF should be added to the environment.");

        var newFloatingDocument = this.envInfo.GetFloatingDocuments()[1];
        Assert.AreEqual(1, newFloatingDocument.exportPages.Count, "The new document should have only one page.");
        Assert.AreEqual(3, newFloatingDocument.exportPages[0].Item3, "The new document should contain the duplicated page.");
        Assert.AreEqual(this.envInfo.GetFloatingDocuments()[0].sprites[2], newFloatingDocument.sprites[0], "The new document should have the correct sprite.");
    }

    /// <summary>
    /// Tests if the log error is written to the log when trying to duplicate a page of a floating document, but the prefab is null
    /// </summary>
    [Test]
    public void TestDuplicatePagePDFCanvasPrefabIsNull()
    {
        // Create the DuplicateController component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.pdfPrefab = null;

        LogAssert.Expect(LogType.Error, "The PDFCanvas prefab is null");

        duplicateDocument.DuplicatePagePDF();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }

    /// <summary>
    /// Tests if the log error is written to the log when trying to duplicate a subsection of a floating document, but the prefab is null
    /// </summary>
    [Test]
    public void TestDuplicateSubsectionPDFCanvasPrefabIsNull()
    {
        // Create the DuplicateController component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.pdfPrefab = null;

        LogAssert.Expect(LogType.Error, "The PDFCanvas prefab is null");

        duplicateDocument.DuplicateSubsectionPDF();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }

    /// <summary>
    /// Tests if the log error is written to the log when trying to duplicate a subsection with an invalid range
    /// </summary>
    [Test]
    public void TestDuplicateSubsectionPDFCanvasInvalidRange()
    {
        // Create the DuplicateController component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.MenuController.StartPageNumber = 6;
        duplicateDocument.MenuController.EndPageNumber = 4;

        LogAssert.Expect(LogType.Error, "Invalid subsection range. Startindex must be above 0 and endindex should be higher then startindex");

        duplicateDocument.DuplicateSubsectionPDF();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }

    /// <summary>
    /// Tests when trying to duplicate a subsection of a floating document.
    /// </summary>
    [Test]
    public void TestDuplicateSubsectionPDFCanvas()
    {
        // Create the DuplicateDocument component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.MenuController.StartPageNumber = 1;
        duplicateDocument.MenuController.EndPageNumber = 2;

        duplicateDocument.DuplicateSubsectionPDF();

        Assert.IsFalse(duplicateDocument.MenuController.Menu.activeSelf);
        Assert.IsFalse(duplicateDocument.MenuController.subsectionMenu.activeSelf);
    }

    /// <summary>
    /// Sets up the DuplicateController component.
    /// </summary>
    private DuplicateController SetupDuplicateDocument()
    {
        var duplicateDocument = new GameObject("DuplicateDocument2").AddComponent<DuplicateController>();
        var menuController = new GameObject("MenuController").AddComponent<MenuController>();
        duplicateDocument.pdfPrefab = this.originalPdfCanvas;
        duplicateDocument.MenuController = menuController;
        menuController.Menu = this.menu;
        menuController.subsectionMenu = this.subPanel;
        return duplicateDocument;
    }
}