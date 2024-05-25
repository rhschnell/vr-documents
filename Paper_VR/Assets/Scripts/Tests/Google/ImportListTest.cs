using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.TestTools;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// Testing the ImportList class
/// </summary>
public class ImportListTest
{
    private GameObject go;
    private ImportList importList;
    private Mock<TMP_Dropdown> mockDropdown;
    private List<File> mockPDFs;

    /// <summary>
    /// setUp the test
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Initialize ImportList and its dependencies
        this.go = new GameObject();
        this.importList = this.go.AddComponent<ImportList>();

        this.mockDropdown = new Mock<TMP_Dropdown>();
        this.importList.dropdown = this.mockDropdown.Object;

        this.mockPDFs = new List<File>
        {
            new File { Name = "Document1.pdf" },
            new File { Name = "Document2.pdf" },
        };

        this.importList.PDFs = this.mockPDFs;
    }

    /// <summary>
    /// Tear down the test
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(this.go);
    }

    /// <summary>
    /// Makes sure that the update list function works as expected
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
