using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// This class contains tests for the BackendPDF class.
/// </summary>
public class BackendPDFTest
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
    /// A new game object is created and the ImportPDF method is called.
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator TestImportList()
    {
        // Setup the import list object
        GameObject gameObject = new GameObject();
        ImportList importList = gameObject.AddComponent<ImportList>();
        importList.characterTransform = gameObject.transform;
        importList.pdfPrefab = new GameObject();
        importList.pdfPrefab.AddComponent<FloatingDocument>();

        // Create a File object
        File file = new File { Name = "Document1.pdf", Id = "appel" };

        // Set the convertPDF object as a mock object
        Mock<BackendPDF> convertPDF = new Mock<BackendPDF>();
        // When the AddImagesToFloatingDocument method is called, do nothing
        convertPDF.Setup(a => a.AddImagesToFloatingDocument(file, It.IsAny<FloatingDocument>()));

        importList.pdfConverter = convertPDF.Object;

        // Call the ImportList method
        yield return importList.ImportPDF(file, "Document1.pdf");

        // Verify that the ImportPDF method was called
        convertPDF.Verify(a => a.AddImagesToFloatingDocument(file, It.IsAny<FloatingDocument>()));


        // Assert that the floating document was created
        Assert.IsNotEmpty(this.env.GetFloatingDocuments());

        // Cleanup
        Object.DestroyImmediate(gameObject);
    }

    /// <summary>
    /// This test checks that a debug error is thrown if the file is null.
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator TestImportListNull()
    {
        // Setup the import list object
        GameObject gameObject = new GameObject();
        ImportList importList = gameObject.AddComponent<ImportList>();
        importList.characterTransform = gameObject.transform;
        importList.pdfPrefab = new GameObject();


        // Assert that a Debug error is printed
        yield return importList.ImportPDF(null, "aoo");

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

        // Create a BackendPDF object
        GameObject convertPDFGameObject = new GameObject();
        BackendPDF convertPDF = convertPDFGameObject.AddComponent<BackendPDF>();

        // Load the json response from the response.txt file
        string jsonResponse = Resources.Load<TextAsset>("Test/response").text;

        // Call the method with the mocked web request
        convertPDF.CreateImages(jsonResponse, floatingDocumentComponent);

        // Assert that the sprites list is not empty
        Assert.IsNotEmpty(floatingDocumentComponent.sprites);

        // Assert that the sprites list has the correct number of sprites
        Assert.AreEqual(1, floatingDocumentComponent.sprites.Count);

        // Cleanup
        Object.DestroyImmediate(floatingDocumentGameObject);
        Object.DestroyImmediate(convertPDFGameObject);
    }

    /// <summary>
    /// This test checks if the send pdf to drive method works as expected.
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator ExportPDFTest()
    {
        Mock<UnityGoogleDrive.GoogleDriveRequest<UnityGoogleDrive.Data.File>> mockFile = new Mock<GoogleDriveRequest<File>>();
        Mock<UnityGoogleDrive.GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>> mockFileYield = new Mock<GoogleDriveRequestYieldInstruction<FileList>>();
        // Create a mock response
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.File>>();
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(mockFile.Object);

        // Create a mock for the create request
        Mock<GoogleDriveFiles.CreateRequest> mockCreateRequest = new Mock<GoogleDriveFiles.CreateRequest>();

        // Set up the create request
        mockCreateRequest.Setup(req => req.IsDone).Returns(true);
        mockCreateRequest.Setup(req => req.Send()).Returns(responseMock.Object);

        // Create a BackendPDF object
        GameObject convertPDFGameObject = new GameObject();
        BackendPDF convertPDF = convertPDFGameObject.AddComponent<BackendPDF>();

        // Act
        yield return convertPDF.ExportPDFToDrive("id", "document.pdf", new byte[0], mockCreateRequest.Object, true);

        mockCreateRequest.Verify(a => a.Send());
    }
}
