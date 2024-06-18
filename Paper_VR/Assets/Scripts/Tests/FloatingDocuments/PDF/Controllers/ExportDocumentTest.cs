using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// This class contains tests for the ExportController class.
/// </summary>
public class ExportDocumentTest : MonoBehaviour
{
    private GameObject originalPdfCanvas;
    private GameObject menu;
    private GameObject gameManager;
    private EnvironmentController envInfo;

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
        this.envInfo = this.gameManager.AddComponent<EnvironmentController>();
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
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator TestExportFloatingDocument()
    {
        // Create the ExportController component and set its fields
        var exportDocument = new GameObject("ExportController").AddComponent<ExportController>();
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
        List<Tuple<string, string, int>> exportPages = new List<Tuple<string, string, int>>()
        {
            new Tuple<string, string, int>("Test", "id", 1),
            new Tuple<string, string, int>("Test", "id", 2),
            new Tuple<string, string, int>("Test", "id", 3),
        };
        exportDocument.floatingDocument.GetComponent<FloatingDocument>().pages = pages;
        exportDocument.floatingDocument.GetComponent<FloatingDocument>().pdfName = "Test";
        exportDocument.floatingDocument.GetComponent<FloatingDocument>().pdfId = "id";
        exportDocument.floatingDocument.GetComponent<FloatingDocument>().exportPages = exportPages;
        convertPDF.Setup(c => c.ExtractPDF(It.IsAny<string>(), It.IsAny<string>(), pages));

        var initalFloatingDocument = this.originalPdfCanvas.GetComponent<FloatingDocument>();
        this.envInfo.GetFloatingDocuments().Add(initalFloatingDocument);

        yield return exportDocument.ExportPDF(convertPDF.Object);

        Assert.IsFalse(this.menu.activeSelf, "The menu should be hidden after export");
        // Verify that the ExtractPDF method was called
        convertPDF.Verify(c => c.ExtractPDF(It.IsAny<string>(), It.IsAny<string>(), pages));
    }

    /// <summary>
    /// Tests if the log error is written to the log.
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator TestFloatingDocumentIsNull()
    {
        // Create the ExportController component and set its fields
        var exportDocument = new GameObject("DuplicateController").AddComponent<ExportController>();
        exportDocument.floatingDocument = null;
        exportDocument.menu = this.menu;

        // Create a ExportPDF game object
        GameObject pdfConvert = new GameObject("PDFConversionManager");
        pdfConvert.AddComponent<BackendPDF>();

        yield return new WaitForSeconds(0.5f);

        LogAssert.Expect(LogType.Error, "The floating document is null");

        exportDocument.OnExportClick();

        // Check if the error log was created
        LogAssert.NoUnexpectedReceived();
    }

    /// <summary>
    /// This method tests the get PDF method.
    /// of the pdf operations class.
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator GetPDFTest()
    {
        // Create a list of pages
        List<Tuple<string, string, int>> pages = new List<Tuple<string, string, int>>
        {
               new Tuple<string, string, int>("Test", "id", 1),
               new Tuple<string, string, int>("Test", "id", 2),
               new Tuple<string, string, int>("Test", "id", 3),

               new Tuple<string, string, int>("Test2", "id2", 1),
               new Tuple<string, string, int>("Test2", "id2", 2),
               new Tuple<string, string, int>("Test2", "id2", 3),

               new Tuple<string, string, int>("Test3", "id3", 1),
               new Tuple<string, string, int>("Test3", "id3", 2),
               new Tuple<string, string, int>("Test3", "id3", 3),
        };

        // Create a new Mock object for the BackendPDF component
        Mock<BackendPDF> convertPDF = new Mock<BackendPDF>();
        convertPDF.Object.extractedPDFcontent = new byte[4] { 1, 2, 3, 4 };
        convertPDF.Object.mergedPDFcontent = new byte[8] { 1, 2, 3, 4, 1, 2, 3, 4 };
        convertPDF.Setup(c => c.ExtractPDF(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<int>>()));
        convertPDF.Setup(c => c.MergePDF(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<byte[]>()));

        yield return PDFOperations.GetPDF(pages, convertPDF.Object);

        convertPDF.Verify(c => c.ExtractPDF(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<int>>()));

        // Check if the content is merged
        Assert.AreEqual(8, PDFOperations.pdfContent.Length);
    }
}