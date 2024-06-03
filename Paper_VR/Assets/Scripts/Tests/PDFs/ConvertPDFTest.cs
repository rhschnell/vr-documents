using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// This class contains tests for the ConvertPDF class.
/// </summary>
public class ConvertPDFTest
{
    private GameObject gameManager;
    private EnvironmentInformation env;

    /// <summary>
    /// Set up the test environment.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // Create a GameManager object and set up the environment
        this.gameManager = new GameObject("GameManager");
        this.env = this.gameManager.AddComponent<EnvironmentInformation>();
        this.env.SetFloatingDocuments(new List<FloatingDocument>());
    }

    /// <summary>
    /// Clean up the test environment.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        // Destroy the GameManager object
        Object.DestroyImmediate(this.gameManager);
    }

    /// <summary>
    /// This test checks that if the user presses the button on the import list
    /// A new game object is created and the ConvertPdf method is called.
    /// </summary>
    [Test]
    public void TestImportList()
    {
        // Setup the import list object
        GameObject gameObject = new GameObject();
        ImportList importList = gameObject.AddComponent<ImportList>();
        importList.characterTransform = gameObject.transform;
        importList.pdfPrefab = new GameObject();

        // Create a File object
        File file = new File { Name = "Document1.pdf" };

        // Set the convertPDF object as a mock object
        Mock<ConvertPDF> convertPDF = new Mock<ConvertPDF>();
        // When the Convert method is called, do nothing
        convertPDF.Setup(a => a.Convert(file, It.IsAny<FloatingDocument>()));

        importList.pdfConverter = convertPDF.Object;

        // Call the ImportList method
        importList.ConvertPdf(file);

        // Verify that the ConvertPdf method was called
        convertPDF.Verify(a => a.Convert(file, It.IsAny<FloatingDocument>()));

        // Assert that the floating document was created
        Assert.IsNotEmpty(this.env.GetFloatingDocuments());

        // Cleanup
        Object.DestroyImmediate(gameObject);
    }

    /// <summary>
    /// This test checks that a debug error is thrown if the file is null.
    /// </summary>
    [Test]
    public void TestImportListNull()
    {
        // Setup the import list object
        GameObject gameObject = new GameObject();
        ImportList importList = gameObject.AddComponent<ImportList>();
        importList.characterTransform = gameObject.transform;
        importList.pdfPrefab = new GameObject();

        // Assert that a Debug error is printed
        importList.ConvertPdf(null);
        LogAssert.Expect(LogType.Error, "File is null");

        // Cleanup
        Object.DestroyImmediate(gameObject);
    }

    /// <summary>
    /// This test checks that the create images method works as expected.
    /// It should create a new sprite for each image in the list.
    /// And add them to the sprites list.
    /// </summary>
    [Test]
    public void TestConvertPDF()
    {
        // Create a FloatingDocument object
        GameObject floatingDocumentGameObject = new GameObject();
        FloatingDocument floatingDocumentComponent = floatingDocumentGameObject.AddComponent<FloatingDocument>();
        floatingDocumentComponent.image = floatingDocumentGameObject.AddComponent<Image>();
        floatingDocumentComponent.currentPageIndex = 0;

        // Create a ConvertPDF object
        GameObject convertPDFGameObject = new GameObject();
        ConvertPDF convertPDF = convertPDFGameObject.AddComponent<ConvertPDF>();

        // Load the json response from the response.txt file
        string jsonResponse = Resources.Load<TextAsset>("Test/response").text;

        // Call the method with the mocked web request
        convertPDF.CreateImages(jsonResponse, floatingDocumentComponent);

        // Assert that the sprites list is not empty
        Assert.IsNotEmpty(floatingDocumentComponent.sprites);

        // Optionally, assert other conditions, such as the correct number of sprites
        Assert.AreEqual(2, floatingDocumentComponent.sprites.Count);

        // Cleanup
        Object.DestroyImmediate(floatingDocumentGameObject);
        Object.DestroyImmediate(convertPDFGameObject);
    }
}
