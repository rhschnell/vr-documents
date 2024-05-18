using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
        EnvironmentInformation envInf = new EnvironmentInformation();
        envInf.SetName("jan");
        Assert.AreEqual("jan", envInf.GetName());
    }

    /// <summary>
    /// Get and set test for the name field.
    /// </summary>
    [Test]
    public void GetAndSetImportListTest()
    {
        EnvironmentInformation envInf = new EnvironmentInformation();
        List<string> importList = new List<string>();
        importList.Add("a");
        importList.Add("b");

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
        EnvironmentInformation envInf = new EnvironmentInformation();
        List<int> floatingDocuments = new List<int>();
        floatingDocuments.Add(1);
        floatingDocuments.Add(2);

        envInf.SetFloatingDocuments(floatingDocuments);

        List<int> newfloatingDocuments = envInf.GetFloatingDocuments();
        Assert.AreEqual(newfloatingDocuments, floatingDocuments);
        Assert.IsTrue(newfloatingDocuments.Contains(1));
        Assert.IsTrue(newfloatingDocuments.Contains(2));
        Assert.AreEqual(newfloatingDocuments.Count, 2);
    }

    /// <summary>
    /// Get and set test for the background color field.
    /// </summary>
    [Test]
    public void GetAndSetBackgroundColorTest()
    {
        EnvironmentInformation envInf = new EnvironmentInformation();
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
        GameObject oldEnvironmentGO = new GameObject("OldEnvironment");
        EnvironmentInformation oldEnvironmentInformation = oldEnvironmentGO.AddComponent<EnvironmentInformation>();

        oldEnvironmentInformation.SetBackgroundColor(Color.red);

        List<int> floatingDocuments = new List<int> { 1, 2 };
        oldEnvironmentInformation.SetFloatingDocuments(floatingDocuments);

        List<string> importList = new List<string> { "a", "b" };
        oldEnvironmentInformation.SetImportList(importList);

        oldEnvironmentInformation.SetName("jan");

        // Create a new GameObject and add the EnvironmentInformation component to it
        GameObject newEnvironmentGO = new GameObject("NewEnvironment");
        EnvironmentInformation newEnvironmentInformation = newEnvironmentGO.AddComponent<EnvironmentInformation>();
        newEnvironmentInformation.LoadNewInformation(oldEnvironmentInformation);

        List<int> newfloatingDocuments = newEnvironmentInformation.GetFloatingDocuments();
        List<string> newImportList = newEnvironmentInformation.GetImportList();

        Assert.AreEqual("jan", newEnvironmentInformation.GetName());
        Assert.AreEqual(importList, newImportList);
        Assert.IsTrue(newImportList.Contains("a"));
        Assert.IsTrue(newImportList.Contains("b"));
        Assert.AreEqual(2, newImportList.Count);
        Assert.AreEqual(floatingDocuments, newfloatingDocuments);
        Assert.IsTrue(newfloatingDocuments.Contains(1));
        Assert.IsTrue(newfloatingDocuments.Contains(2));
        Assert.AreEqual(2, newfloatingDocuments.Count);
        Assert.AreEqual(Color.red, newEnvironmentInformation.GetBackgroundColor());

        // Clean up the GameObjects after the test
        Object.DestroyImmediate(oldEnvironmentGO);
        Object.DestroyImmediate(newEnvironmentGO);
    }
}
