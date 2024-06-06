using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

/// <summary>
/// This class contains tests for the DuplicateDocument class.
/// </summary>
public class DuplicateDocumentTest : MonoBehaviour
{
    private GameObject originalPdfCanvas;
    private GameObject menu;
    private GameObject subPanel;
    private GameObject duplicateMenu;
    private GameObject gameManager;
    private EnvironmentInformation envInfo;
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
        floatingDocument.pdfPath = "some/path/to/original.pdf";
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
        floatingDocument.currentPageIndex = 2;

        this.floatingDocumentImage = this.originalPdfCanvas.AddComponent<Image>();
        floatingDocument.image = this.floatingDocumentImage;

        this.gameManager = new GameObject("GameManager");
        this.envInfo = this.gameManager.AddComponent<EnvironmentInformation>();
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
        // Create the DuplicateDocument component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();

        var initalFloatingDocument = this.originalPdfCanvas.GetComponent<FloatingDocument>();
        this.envInfo.GetFloatingDocuments().Add(initalFloatingDocument);

        duplicateDocument.OnDuplicationDocumentClick();

        Assert.AreEqual(2, this.envInfo.GetFloatingDocuments().Count, "The duplicated PDF should be added to the environment.");
        Assert.IsFalse(this.menu.activeSelf, "The menu should be hidden after duplication");
        Assert.IsFalse(this.subPanel.activeSelf, "The subpanel should be hidden after duplication");
    }

    /// <summary>
    /// Tests if the log error is written to the log when trying to duplicate a floating document, but the prefab is null
    /// </summary>
    [Test]
    public void TestDuplicatePDFCanvasPrefabIsNull()
    {
        // Create the DuplicateDocument component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.pdfCanvasPrefab = null;

        LogAssert.Expect(LogType.Error, "The PDFCanvas prefab is null");

        duplicateDocument.OnDuplicationDocumentClick();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }

    /// <summary>
    /// A test that confirms that the shown page of the document is duplicated.
    /// </summary>
    [Test]
    public void TestDuplicatePagePDFCanvas()
    {
        // Create the DuplicateDocument component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();

        duplicateDocument.OnDuplicationPageClick();

        Assert.AreEqual(2, this.envInfo.GetFloatingDocuments().Count, "The duplicated PDF should be added to the environment.");

        var newFloatingDocument = this.envInfo.GetFloatingDocuments()[1];
        Assert.AreEqual(1, newFloatingDocument.pages.Count, "The new document should have only one page.");
        Assert.AreEqual(2, newFloatingDocument.pages[0], "The new document should contain the duplicated page.");
        Assert.AreEqual(this.envInfo.GetFloatingDocuments()[0].sprites[2], newFloatingDocument.sprites[0], "The new document should have the correct sprite.");

        Assert.IsFalse(this.menu.activeSelf, "The menu should be hidden after duplication");
        Assert.IsFalse(this.subPanel.activeSelf, "The subpanel should be hidden after duplication");
    }

    /// <summary>
    /// Tests if the log error is written to the log when trying to duplicate a page of a floating document, but the prefab is null
    /// </summary>
    [Test]
    public void TestDuplicatePagePDFCanvasPrefabIsNull()
    {
        // Create the DuplicateDocument component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.pdfCanvasPrefab = null;

        LogAssert.Expect(LogType.Error, "The PDFCanvas prefab is null");

        duplicateDocument.OnDuplicationPageClick();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }

    /// <summary>
    /// Tests if the panels correctly show when OnDuplicationClick is called
    /// </summary>
    [Test]
    public void TestOnDuplicationClick()
    {
        var duplicateDocument = this.SetupDuplicateDocument();

        duplicateDocument.OnDuplicationClick();

        Assert.IsFalse(this.menu.activeSelf, "The menu should not show when OnDuplicationClick is called");
        Assert.IsTrue(this.subPanel.activeSelf, "The subpanel should show when OnDuplicationClick is called");
    }

    /// <summary>
    /// Tests if the panels correctly show when OnReturnClick is called
    /// </summary>
    [Test]
    public void TestReturnClick()
    {
        var duplicateDocument = this.SetupDuplicateDocument();

        duplicateDocument.OnDuplicationClick();
        duplicateDocument.OnReturnClick();

        Assert.IsFalse(this.subPanel.activeSelf, "The subpanel should not show when OnReturnClick is called");
        Assert.IsTrue(this.menu.activeSelf, "The menu should show when OnReturnClick is called");
    }

    /// <summary>
    /// Tests if the duplication of a subsection is implemented correctly
    /// </summary>
    // [Test]
    // public void TestDuplicateSubsectionPDFCanvas()
    // {
    //    // Create the DuplicateDocument component and set its fields
    //    var duplicateDocument = new GameObject("DuplicateDocument").AddComponent<DuplicateDocument>();
    //    duplicateDocument.pdfCanvasPrefab = this.originalPdfCanvas;
    //    duplicateDocument.menu = this.menu;
    //    duplicateDocument.subPanel = this.subPanel;
    //    duplicateDocument.duplicateMenu = this.duplicateMenu;
    //    duplicateDocument.StartPageNumber = 2;
    //    duplicateDocument.EndPageNumber = 4;
    //    duplicateDocument.OnDuplicationSubsectionClick();
    //    //Assert.AreEqual(2, this.envInfo.GetFloatingDocuments().Count, "The duplicated PDF should be added to the environment.");
    //    var newFloatingDocument = this.envInfo.GetFloatingDocuments()[1];
    //    Assert.AreEqual(3, newFloatingDocument.pages.Count, "The new document should have three pages.");
    //    Assert.AreEqual(this.envInfo.GetFloatingDocuments()[0].sprites[1], newFloatingDocument.sprites[0], "The new document should have the correct sprite for the first page.");
    //    Assert.AreEqual(this.envInfo.GetFloatingDocuments()[0].sprites[2], newFloatingDocument.sprites[1], "The new document should have the correct sprite for the second page.");
    //    Assert.AreEqual(this.envInfo.GetFloatingDocuments()[0].sprites[3], newFloatingDocument.sprites[2], "The new document should have the correct sprite for the third page.");
    //    Assert.IsFalse(this.menu.activeSelf, "The menu should be hidden after duplication");
    //    Assert.IsFalse(this.subPanel.activeSelf, "The subpanel should be hidden after duplication");
    // }

    /// <summary>
    /// Tests if the log error is written to the log when trying to duplicate a subsection of a floating document, but the prefab is null
    /// </summary>
    [Test]
    public void TestDuplicateSubsectionPDFCanvasPrefabIsNull()
    {
        // Create the DuplicateDocument component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.pdfCanvasPrefab = null;

        LogAssert.Expect(LogType.Error, "The PDFCanvas prefab is null");

        duplicateDocument.OnDuplicationSubsectionClick();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }

    /// <summary>
    /// Tests if the log error is written to the log when trying to duplicate a subsection with an invalid range
    /// </summary>
    [Test]
    public void TestDuplicateSubsectionPDFCanvasInvalidRange()
    {
        // Create the DuplicateDocument component and set its fields
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.StartPageNumber = 6;
        duplicateDocument.EndPageNumber = 4;

        LogAssert.Expect(LogType.Error, "Invalid subsection range. Startindex must be above 0 and endindex should be higher then startindex");

        duplicateDocument.OnDuplicationSubsectionClick();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }

    /// <summary>
    /// Tests if the panels correctly show when OnDuplicationSubsectionMenuClick is called
    /// </summary>
    [Test]
    public void TestOnDuplicationSubsectionMenuClick()
    {
        var duplicateDocument = this.SetupDuplicateDocument();

        duplicateDocument.OnDuplicationSubsectionMenuClick();

        Assert.AreEqual(5, duplicateDocument.MaxPage, "The MaxPage should be updated correctly");
        Assert.AreEqual(5, duplicateDocument.EndPageNumber, "The EndPageNumber should be updated correctly");
    }

    /// <summary>
    /// Tests if the start plus button correctly increases the start page number
    /// </summary>
    [Test]
    public void TestStartPlusButton()
    {
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.StartPageNumber = 1;
        duplicateDocument.EndPageNumber = 5;

        duplicateDocument.ClickOnStartPlusButton();

        Assert.AreEqual(2, duplicateDocument.StartPageNumber, "Start page number should increase by 1");
    }

    /// <summary>
    /// Tests if the start minus button correctly decreases the start page number
    /// </summary>
    [Test]
    public void TestStartMinusButton()
    {
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.StartPageNumber = 3;
        duplicateDocument.EndPageNumber = 5;

        duplicateDocument.ClickOnStartMinButton();

        Assert.AreEqual(2, duplicateDocument.StartPageNumber, "Start page number should decrease by 1");
    }

    /// <summary>
    /// Tests if the end plus button correctly increases the end page number
    /// </summary>
    [Test]
    public void TestEndPlusButton()
    {
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.StartPageNumber = 1;
        duplicateDocument.EndPageNumber = 3;
        duplicateDocument.MaxPage = 5;

        duplicateDocument.ClickOnEndPlusButton();

        Assert.AreEqual(4, duplicateDocument.EndPageNumber, "End page number should increase by 1");
    }

    /// <summary>
    /// Tests if the end minus button correctly decreases the end page number
    /// </summary>
    [Test]
    public void TestEndMinusButton()
    {
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.StartPageNumber = 1;
        duplicateDocument.EndPageNumber = 3;

        duplicateDocument.ClickOnEndMinButton();

        Assert.AreEqual(2, duplicateDocument.EndPageNumber, "End page number should decrease by 1");
    }

    /// <summary>
    /// Tests if the update label method correctly updates the label text
    /// </summary>
    [Test]
    public void TestUpdateLabel()
    {
        var duplicateDocument = this.SetupDuplicateDocument();
        duplicateDocument.StartPageNumberText = new GameObject().AddComponent<TextMeshPro>();
        duplicateDocument.EndPageNumberText = new GameObject().AddComponent<TextMeshPro>();

        duplicateDocument.StartPageNumber = 2;
        duplicateDocument.EndPageNumber = 5;

        duplicateDocument.UpdateLabel();

        Assert.AreEqual("2", duplicateDocument.StartPageNumberText.text, "Start page number label text should be updated correctly");
        Assert.AreEqual("5", duplicateDocument.EndPageNumberText.text, "End page number label text should be updated correctly");
    }

    /// <summary>
    /// Sets up the DuplicateDocument component.
    /// </summary>
    private DuplicateDocument SetupDuplicateDocument()
    {
        var duplicateDocument = new GameObject("DuplicateDocument").AddComponent<DuplicateDocument>();
        duplicateDocument.pdfCanvasPrefab = this.originalPdfCanvas;
        duplicateDocument.menu = this.menu;
        duplicateDocument.subPanel = this.subPanel;
        duplicateDocument.duplicateMenu = this.duplicateMenu;
        return duplicateDocument;
    }
}