using System.Collections;
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
        this.originalPdfCanvas.AddComponent<FloatingDocument>();

        this.menu = new GameObject("Menu");

        this.gameManager = new GameObject("GameManager");
        this.envInfo = this.gameManager.AddComponent<EnvironmentInformation>();
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

        var initalFloatingDocument = this.originalPdfCanvas.GetComponent<FloatingDocument>();
        this.envInfo.GetFloatingDocuments().Add(initalFloatingDocument);

        duplicateDocument.OnDuplicationClick();

        Assert.AreEqual(2, this.envInfo.GetFloatingDocuments().Count, "The duplicated PDF should be added to the environment.");
        Assert.IsFalse(this.menu.activeSelf, "The menu should be hidden after duplication");
    }

    /// <summary>
    /// Tests if the log error is written to the log.
    /// </summary>
    [Test]
    public void TestDuplicatePDFCanvasPrefabIsNull()
    {
        // Create the DuplicateDocument component and set its fields
        var duplicateDocument = new GameObject("DuplicateDocument").AddComponent<DuplicateDocument>();
        duplicateDocument.pdfCanvasPrefab = null;
        duplicateDocument.menu = this.menu;

        LogAssert.Expect(LogType.Error, "The PDFCanvas prefab is null");

        duplicateDocument.OnDuplicationClick();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }
}