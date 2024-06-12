using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.WSA;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// The testing class for EnvironmentInformation.
/// </summary>
public class EnvironmentInformationTest
{
    /// <summary>
    /// Get and set test for the name field.
    /// </summary>
    [Test]
    public void GetAndSetNameTest()
    {
        EnvironmentInformation envInf = this.CreateObjectEnv();
        envInf.SetName("jan");
        Assert.AreEqual("jan", envInf.GetName());
    }

    /// <summary>
    /// Get and set test for the name field.
    /// </summary>
    [Test]
    public void GetAndSetImportListTest()
    {
        EnvironmentInformation envInf = this.CreateObjectEnv();
        List<string> importList = new () { "a", "b" };

        envInf.SetImportList(importList);

        List<string> newImportList = envInf.GetImportList();
        Assert.AreEqual(newImportList, importList);
        Assert.IsTrue(newImportList.Contains("a"));
        Assert.IsTrue(newImportList.Contains("b"));
        Assert.AreEqual(newImportList.Count, 2);
    }

    /// <summary>
    /// Get and set test for the floating documents field.
    /// </summary>
    [Test]
    public void GetAndSetFloatingDocumentsTest()
    {
        EnvironmentInformation envInf = this.CreateObjectEnv();
        GameObject examplePrefab1 = new GameObject();
        FloatingDocument floatingDocument1 = examplePrefab1.AddComponent<FloatingDocument>();
        floatingDocument1.pages = new List<int> { 1, 2, 3 };
        floatingDocument1.pdfId = "googledrive";
        floatingDocument1.pdfName = "examplename";

        GameObject examplePrefab2 = new GameObject();
        FloatingDocument floatingDocument2 = examplePrefab2.AddComponent<FloatingDocument>();
        floatingDocument2.pages = new List<int> { 4, 5, 6 };
        floatingDocument2.pdfId = "googledrive";
        floatingDocument2.pdfName = "examplename";

        List<FloatingDocument> floatingDocuments = new ()
        {
            floatingDocument1,
            floatingDocument2,
        };

        envInf.SetFloatingDocuments(floatingDocuments);

        List<FloatingDocument> newfloatingDocuments = envInf.GetFloatingDocuments();
        Assert.AreEqual(newfloatingDocuments, floatingDocuments);
        Assert.IsTrue(newfloatingDocuments.Contains(floatingDocument1));
        Assert.IsTrue(newfloatingDocuments.Contains(floatingDocument2));
        Assert.AreEqual(newfloatingDocuments.Count, 2);
    }

    /// <summary>
    /// Get and set test for the background color field.
    /// </summary>
    [Test]
    public void GetAndSetBackgroundColorTest()
    {
        EnvironmentInformation envInf = this.CreateObjectEnv();
        envInf.SetBackgroundColor(Color.red);
        Assert.AreEqual(Color.red, envInf.GetBackgroundColor());
    }

    /// <summary>
    /// Tests the method that loads new information onto the instance.
    /// </summary>
    [Test]
    public void LoadNewInformationTest()
    {
        // Create a GameObject and add the EnvironmentInformation component to it
        EnvironmentInformation oldEnvironmentInformation = this.CreateObjectEnv();

        oldEnvironmentInformation.SetBackgroundColor(Color.red);

        // Create a new prefab for the document
        GameObject docPrefab = this.CreatePrefab();

        // Set the prefab for the document
        oldEnvironmentInformation.docPrefab = docPrefab;

        // Create new floating documents
        FloatingDocument floatingDocument1 = docPrefab.AddComponent<FloatingDocument>();
        floatingDocument1.pages = new List<int> { 1, 2, 3 };
        floatingDocument1.pdfId = "googledrive";
        floatingDocument1.pdfName = "examplename";
        floatingDocument1.exportPages = new List<Tuple<string, string, int>> { new Tuple<string, string, int>("googledrive", "examplename", 1) };

        FloatingDocument floatingDocument2 = docPrefab.GetComponent<FloatingDocument>();
        floatingDocument2.pages = new List<int> { 4, 5, 6 };
        floatingDocument2.pdfId = "googledrive";
        floatingDocument2.pdfName = "examplename";
        floatingDocument2.exportPages = new List<Tuple<string, string, int>> { new Tuple<string, string, int>("googledrive", "examplename", 1) };

        List<FloatingDocument> floatingDocuments = new ()
        {
            floatingDocument1,
            floatingDocument2,
        };

        oldEnvironmentInformation.SetFloatingDocuments(floatingDocuments);

        List<string> importList = new () { "a", "b" };
        oldEnvironmentInformation.SetImportList(importList);
        oldEnvironmentInformation.SetName("jan");

        // Create a new GameObject and add the EnvironmentInformation component to it
        EnvironmentInformation newEnvironmentInformation = this.CreateObjectEnv();
        newEnvironmentInformation.docPrefab = docPrefab;

        newEnvironmentInformation.LoadNewInformation(new EnvironmentInfo(oldEnvironmentInformation));

        List<FloatingDocument> newfloatingDocuments = newEnvironmentInformation.GetFloatingDocuments();
        List<string> newImportList = newEnvironmentInformation.GetImportList();

        Assert.AreEqual("jan", newEnvironmentInformation.GetName());
        Assert.AreEqual(importList, newImportList);
        Assert.IsTrue(newImportList.Contains("a"));
        Assert.IsTrue(newImportList.Contains("b"));
        Assert.AreEqual(2, newImportList.Count);
        Assert.AreEqual(floatingDocuments, newfloatingDocuments);
        Assert.IsTrue(newfloatingDocuments.Contains(floatingDocument1));
        Assert.IsTrue(newfloatingDocuments.Contains(floatingDocument2));
        Assert.AreEqual(2, newfloatingDocuments.Count);
        Assert.AreEqual(Color.red, newEnvironmentInformation.GetBackgroundColor());
    }

    /// <summary>
    /// This Test checks if the EnvironmentInformation can be saved to a JSON file.
    /// And then loaded back from the JSON file.
    /// </summary>
    [Test]
    public void SaveAndLoadJson()
    {
        // Create a GameObject and add the EnvironmentInformation component to it
        EnvironmentInformation environmentInformation = this.CreateObjectEnv();
        environmentInformation.SetBackgroundColor(Color.red);

        // Create a new prefab for the document
        GameObject docPrefab = this.CreatePrefab();

        environmentInformation.docPrefab = docPrefab;

        // Create new floating documents
        FloatingDocument floatingDocument1 = docPrefab.AddComponent<FloatingDocument>();
        floatingDocument1.pages = new List<int> { 1, 2, 3 };
        floatingDocument1.pdfId = "googledrive";
        floatingDocument1.pdfName = "examplename";
        floatingDocument1.exportPages = new List<Tuple<string, string, int>> { new Tuple<string, string, int>("googledrive", "examplename", 1) };

        FloatingDocument floatingDocument2 = docPrefab.GetComponent<FloatingDocument>();
        floatingDocument2.pages = new List<int> { 4, 5, 6 };
        floatingDocument2.pdfId = "googledrive";
        floatingDocument2.pdfName = "examplename";
        floatingDocument2.exportPages = new List<Tuple<string, string, int>> { new Tuple<string, string, int>("googledrive", "examplename", 1) };

        List<FloatingDocument> floatingDocuments = new List<FloatingDocument>
        {
            floatingDocument1,
            floatingDocument2,
        };

        environmentInformation.SetFloatingDocuments(floatingDocuments);

        List<string> importList = new () { "a", "b" };
        environmentInformation.SetImportList(importList);
        environmentInformation.SetName("jan");

        // Save the EnvironmentInformation to a JSON file
        EnvironmentInfo environmentInfo = new EnvironmentInfo(environmentInformation);
        string json = environmentInfo.SaveToJson();

        // Load the EnvironmentInformation from the JSON file
        EnvironmentInfo loadedEnvironmentInfo = EnvironmentInfo.LoadFromJson(json);

        Debug.Log(json);

        // Create a GameObject and add the EnvironmentInformation component to it
        EnvironmentInformation environmentInformation2 = this.CreateObjectEnv();
        environmentInformation2.docPrefab = docPrefab;

        // Load the EnvironmentInformation to the EnvironmentInformation component
        environmentInformation2.LoadNewInformation(loadedEnvironmentInfo);

        // Assert that environmentInformation2 has the same information as environmentInformation
        Assert.IsNotNull(environmentInformation2);
        Assert.AreEqual("jan", environmentInformation2.GetName());
        Assert.AreEqual(importList, environmentInformation2.GetImportList());
        Assert.IsTrue(environmentInformation2.GetImportList().Contains("a"));
        Assert.IsTrue(environmentInformation2.GetImportList().Contains("b"));
        Assert.AreEqual(2, environmentInformation2.GetImportList().Count);
        Assert.AreEqual(floatingDocuments, environmentInformation2.GetFloatingDocuments());

        // Get the first floating document
        FloatingDocument doc3 = environmentInformation2.GetFloatingDocuments()[0];
        Assert.AreEqual(floatingDocument1.transform.position, doc3.position);
        Assert.AreEqual(floatingDocument1.transform.rotation, doc3.rotation);
        Assert.AreEqual(floatingDocument1.transform.localScale, doc3.scale);

        // Get the second floating document
        FloatingDocument doc4 = environmentInformation2.GetFloatingDocuments()[1];
        Assert.AreEqual(floatingDocument2.transform.position, doc4.position);
        Assert.AreEqual(floatingDocument2.transform.rotation, doc4.rotation);
        Assert.AreEqual(floatingDocument2.transform.localScale, doc4.scale);

        Assert.IsTrue(environmentInformation2.GetFloatingDocuments().Contains(floatingDocument1));
        Assert.IsTrue(environmentInformation2.GetFloatingDocuments().Contains(floatingDocument2));
        Assert.AreEqual(2, environmentInformation2.GetFloatingDocuments().Count);
        Assert.AreEqual(Color.red, environmentInformation2.GetBackgroundColor());
    }

    /// <summary>
    /// This test will check if the json file is saved to the google drive.
    /// </summary>
    /// <returns>IEnumerator</returns>
    [UnityTest]
    public IEnumerator SaveToDrive()
    {
        // Create a GameObject and add the EnvironmentInformation component to it
        EnvironmentInformation environmentInformation = this.CreateObjectEnv();
        environmentInformation.SetName("Environment");

        // Create a new prefab for the document
        GameObject docPrefab = this.CreatePrefab();
        environmentInformation.docPrefab = docPrefab;

        // Create a response mock
        var responseMock = new Mock<GoogleDriveRequestYieldInstruction<UnityGoogleDrive.Data.FileList>>();

        // Mock the google methods
        Mock<GoogleMethods> googleMethods = new Mock<GoogleMethods>();
        googleMethods.Object.currentEnvId = "id";

        // Set up the google methods
        googleMethods.Setup(a => a.FindEnviormentId("Environment", It.IsAny<GoogleDriveFiles.ListRequest>())).Returns(responseMock.Object);
        googleMethods.Setup(a => a.DeleteFile("Environment.json", "id", It.IsAny<GoogleDriveFiles.ListRequest>(), false)).Returns(responseMock.Object);
        googleMethods.Setup(a => a.CreateJsonFile("id", It.IsAny<byte[]>(), It.IsAny<GoogleDriveFiles.CreateRequest>(), false)).Returns(responseMock.Object);

        // Save the EnvironmentInformation to the google drive
        yield return environmentInformation.SaveEnvironment(googleMethods.Object);

        // Verify that the findEnvid method is called
        googleMethods.Verify(a => a.FindEnviormentId("Environment", It.IsAny<GoogleDriveFiles.ListRequest>()));

        // Verify that the delete file method is called
        googleMethods.Verify(a => a.DeleteFile("Environment.json", "id", It.IsAny<GoogleDriveFiles.ListRequest>(), false));

        // Verify that that the create json file method is called
        googleMethods.Verify(a => a.CreateJsonFile("id", It.IsAny<byte[]>(), It.IsAny<GoogleDriveFiles.CreateRequest>(), false));
    }

    /// <summary>
    /// This method creates an empty EnvironmentInformation object.
    /// </summary>
    /// <returns>
    /// An empty EnvironmentInformation object.
    /// </returns>
    private EnvironmentInformation CreateObjectEnv()
    {
        // Create a GameObject and add the EnvironmentInformation component to it
        GameObject gameObject = new GameObject("Environment");
        EnvironmentInformation environmentInformation = gameObject.AddComponent<EnvironmentInformation>();

        return environmentInformation;
    }

    /// <summary>
    /// This method creates a new prefab for the document.
    /// </summary>
    /// <returns>
    /// The new prefab for the document.
    /// </returns>
    private GameObject CreatePrefab()
    {
        // Create a new prefab for the document
        GameObject docPrefab = new GameObject();
        docPrefab.AddComponent<Canvas>();

        // Add an image to the canvas as a child
        GameObject image = new GameObject();
        image.AddComponent<Image>();
        image.transform.SetParent(docPrefab.transform);

        return docPrefab;
    }
}
