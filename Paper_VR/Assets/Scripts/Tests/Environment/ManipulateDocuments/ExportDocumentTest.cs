using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// This class contains tests for the ExportDocument class.
/// </summary>
public class ExportDocumentTest : MonoBehaviour
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
    /// A test that confirms that the document is exported.
    /// </summary>
    [Test]
    public void TestExportFloatingDocument()
    {
        // Create the ExportDocument component and set its fields
        var exportDocument = new GameObject("ExportDocument").AddComponent<ExportDocument>();
        exportDocument.floatingDocument = this.originalPdfCanvas;
        exportDocument.menu = this.menu;

        // Create a new Mock object for the BackendPDF component
        Mock<BackendPDF> convertPDF = new Mock<BackendPDF>();
        List<int> pages = new ()
        {
            1,
            2,
            3,
        };
        exportDocument.floatingDocument.GetComponent<FloatingDocument>().pages = pages;
        convertPDF.Setup(c => c.ExtractPDF(It.IsAny<string>(), It.IsAny<string>(), pages, It.IsAny<string>()));

        var initalFloatingDocument = this.originalPdfCanvas.GetComponent<FloatingDocument>();
        this.envInfo.GetFloatingDocuments().Add(initalFloatingDocument);

        exportDocument.ExportPDF(convertPDF.Object);

        Assert.IsFalse(this.menu.activeSelf, "The menu should be hidden after export");
        // Verify that the ExtractPDF method was called
        convertPDF.Verify(c => c.ExtractPDF(It.IsAny<string>(), It.IsAny<string>(), pages, It.IsAny<string>()));
    }

    /// <summary>
    /// Tests if the log error is written to the log.
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator TestFloatingDocumentIsNull()
    {
        // Create the ExportDocument component and set its fields
        var exportDocument = new GameObject("DuplicateDocument").AddComponent<ExportDocument>();
        exportDocument.floatingDocument = null;
        exportDocument.menu = this.menu;

        // Create a ExportPDF game object
        GameObject pdfConvert = new GameObject("PDFConvert");
        BackendPDF convertPDF = pdfConvert.AddComponent<BackendPDF>();

        yield return new WaitForSeconds(0.5f);

        LogAssert.Expect(LogType.Error, "The floating document is null");

        exportDocument.OnExportClick();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }
}