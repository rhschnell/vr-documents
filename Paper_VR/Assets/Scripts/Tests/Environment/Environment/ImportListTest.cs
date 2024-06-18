using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// Testing the ImportList class.
/// </summary>
public class ImportListTest
{
    private GameObject go;
    private ImportList importList;
    private Mock<TMP_Dropdown> mockDropdown;
    private List<File> mockPDFs;
    private GameObject importButton;

    /// <summary>
    /// Sets up the test environment.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Initialize ImportList and its dependencies.
        GameObject go = new GameObject();
        this.importList = go.AddComponent<ImportList>();

        // Create a GameManeger object
        GameObject gameManager = new GameObject("GameManager");
        gameManager.AddComponent<EnvironmentController>();

        this.mockDropdown = new Mock<TMP_Dropdown>();
        this.importList.dropdown = this.mockDropdown.Object;

        this.go = new GameObject();
        this.importList = this.go.AddComponent<ImportList>();

        this.importButton = new GameObject("ImportButton");

        this.mockDropdown = new Mock<TMP_Dropdown>();
        this.importList.dropdown = this.mockDropdown.Object;

        this.mockPDFs = new List<File>
        {
            new File { Name = "Document1.pdf" },
            new File { Name = "Document2.pdf" },
        };

        this.importList.PDFs = this.mockPDFs;
        this.importList.importButton = this.importButton.AddComponent<Button>();
    }

    /// <summary>
    /// Tear down the test.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(this.go);
        GameObject.DestroyImmediate(this.importButton);
    }

    /// <summary>
    /// Makes sure that the update list function works as expected.
    /// </summary>
    [Test]
    public void UpdateList_ShouldUpdateDropdownOptions()
    {
        var expectedOptions = new List<TMP_Dropdown.OptionData> { new TMP_Dropdown.OptionData("Document1"), new TMP_Dropdown.OptionData("Document1") };

        this.importList.UpdateList();

        Assert.AreEqual(this.mockDropdown.Object.options[0].text, "Document1");
        Assert.AreEqual(this.mockDropdown.Object.options[1].text, "Document2");
    }

    /// <summary>
    /// Tests that the import button is not interactable when the import list is empty.
    /// </summary>
    [Test]
    public void UpdateList_NotInteractableOnEmptyList()
    {
        // Set the dropdown to have zero PDFs
        this.importList.PDFs.Clear();

        this.importList.UpdateList();

        Assert.IsFalse(this.importList.importButton.interactable);
    }

    /// <summary>
    /// Makes sure that the refresh button updates the import list.
    /// </summary>
    [Test]
    public void OnClickRefresh_ShouldUpdateImportList()
    {
        this.importList.PDFs.Clear();

        // Create mock PDF objects and add them to the import list
        var importListPDFs = new List<File>
        {
            new File { Name = "Document1.pdf" },
            new File { Name = "Document2.pdf" },
        };

        this.importList.PDFs = importListPDFs;

        // Call the update list method
        this.importList.UpdateList();

        // Assert

        Assert.AreEqual(2, this.mockDropdown.Object.options.Count);
        Assert.AreEqual("Document1", this.mockDropdown.Object.options[0].text);
        Assert.AreEqual("Document2", this.mockDropdown.Object.options[1].text);
    }

    /// <summary>
    /// Makes sure that the find PDF function works as expected
    /// </summary>
    /// <returns>Its a unity test</returns>
    [UnityTest]
    public IEnumerator FindPDF_ShouldUpdatePDFListAndCallUpdateList()
    {
        // Arrange
        var parentId = "test_parent_id";
        SelectEnvironment.parentId = parentId;

        var responseMock2 = new Mock<GoogleDriveRequest<UnityGoogleDrive.Data.FileList>>();
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();
        responseMock2.Setup(a => a.IsDone).Returns(true);
        responseMock.Setup(a => a.GoogleDriveRequest).Returns(responseMock2.Object);

        var mockRequest = new Mock<GoogleDriveFiles.ListRequest>();
        mockRequest.Setup(r => r.Send()).Returns(responseMock.Object);
        mockRequest.SetupGet(r => r.IsError).Returns(false);
        mockRequest.SetupGet(r => r.ResponseData).Returns(new FileList { Files = this.mockPDFs });

        GoogleDriveFiles.ListRequest a = mockRequest.Object;
        this.importList.envReq = a;

        // Act
        yield return this.importList.FindPDF();

        // Assert
        Assert.AreEqual(this.mockPDFs, this.importList.PDFs);
        Assert.AreEqual(this.mockDropdown.Object.options[0].text, "Document1");
        Assert.AreEqual(this.mockDropdown.Object.options[1].text, "Document2");

        SelectEnvironment.parentId = null;
    }
}