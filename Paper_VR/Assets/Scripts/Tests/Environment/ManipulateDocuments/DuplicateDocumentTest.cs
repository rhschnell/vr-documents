using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// This class contains tests for the DuplicateDocument class.
/// </summary>
public class DuplicateDocumentTest : MonoBehaviour
{
    private GameObject originalPdfCanvas;
    private GameObject menu;
    private GameObject subPanel;
    private GameObject gameManager;
    private EnvironmentInformation envInfo;

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
        this.menu.transform.SetParent(this.originalPdfCanvas.transform);
        this.subPanel.transform.SetParent(this.originalPdfCanvas.transform);

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
        Destroy(this.gameManager);
    }

    /// <summary>
    /// A test that confirms that the document is duplicated.
    /// </summary>
    [Test]
    public void TestDuplicatePDFCanvas()
    {
        // Create the DuplicateDocument component and set its fields
        var duplicateDocument = new GameObject("DuplicateDocument").AddComponent<DuplicateDocument>();
        duplicateDocument.pdfCanvasPrefab = this.originalPdfCanvas;
        duplicateDocument.menu = this.menu;
        duplicateDocument.subPanel = this.subPanel;

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
        var duplicateDocument = new GameObject("DuplicateDocument").AddComponent<DuplicateDocument>();
        duplicateDocument.pdfCanvasPrefab = null;
        duplicateDocument.menu = this.menu;
        duplicateDocument.subPanel = this.subPanel;

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
        var duplicateDocument = new GameObject("DuplicateDocument").AddComponent<DuplicateDocument>();
        duplicateDocument.pdfCanvasPrefab = this.originalPdfCanvas;
        duplicateDocument.menu = this.menu;
        duplicateDocument.subPanel = this.subPanel;

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
        var duplicateDocument = new GameObject("DuplicateDocument").AddComponent<DuplicateDocument>();
        duplicateDocument.pdfCanvasPrefab = null;
        duplicateDocument.menu = this.menu;
        duplicateDocument.subPanel = this.subPanel;

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
        var duplicateDocument = new GameObject("DuplicateDocument").AddComponent<DuplicateDocument>();
        duplicateDocument.menu = this.menu;
        duplicateDocument.subPanel = this.subPanel;

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
        var duplicateDocument = new GameObject("DuplicateDocument").AddComponent<DuplicateDocument>();
        duplicateDocument.menu = this.menu;
        duplicateDocument.subPanel = this.subPanel;

        duplicateDocument.OnDuplicationClick();
        duplicateDocument.OnReturnClick();

        Assert.IsFalse(this.subPanel.activeSelf, "The subpanel should not show when OnReturnClick is called");
        Assert.IsTrue(this.menu.activeSelf, "The menu should show when OnReturnClick is called");
    }
}