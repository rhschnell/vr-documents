using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// A class containing tests for the MergeController class.
/// </summary>
public class MergeTest
{
    // setup 2 documents, the menu, the merge menu and the dropdown menu
    GameObject go;
    GameObject document1;
    GameObject document2;
    GameObject menu;
    GameObject subsectionMenu;
    GameObject mergeMenu;
    GameObject gameManager;
    TMPro.TMP_Text error;
    TMPro.TMP_Text selected;
    MergeController mergeDocuments;
    MenuController menuController;

    /// <summary>
    /// Setup the testing environment, creating the necessary objects.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.go = new GameObject();
        this.document1 = new GameObject();
        this.document2 = new GameObject();
        this.menu = new GameObject();
        this.subsectionMenu = new GameObject();
        this.mergeMenu = new GameObject();
        this.gameManager = new GameObject();
        MergeManager mergeManager = this.gameManager.AddComponent<MergeManager>();
        this.error = this.mergeMenu.AddComponent<TMPro.TextMeshPro>();
        this.selected = this.gameManager.AddComponent<TMPro.TextMeshPro>();
        this.mergeDocuments = this.go.AddComponent<MergeController>();
        this.menuController = this.menu.AddComponent<MenuController>();

        this.menuController.Menu = this.menu;
        this.menuController.subsectionMenu = this.subsectionMenu;

        this.mergeDocuments.pdfPrefab = this.document1;
        this.mergeDocuments.menuController = this.menuController;
        this.mergeDocuments.GameManager = this.gameManager;
        this.mergeDocuments.selected = this.selected;
        this.mergeDocuments.error = this.error;
        this.mergeDocuments.mergeMenu = this.mergeMenu;
        this.mergeDocuments.selected = this.selected;
    }

    /// <summary>
    /// Destorys the objects created in the setup method.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        UnityEngine.Object.Destroy(this.go);
        UnityEngine.Object.Destroy(this.document1);
        UnityEngine.Object.Destroy(this.document2);
        UnityEngine.Object.Destroy(this.menu);
        UnityEngine.Object.Destroy(this.subsectionMenu);
        UnityEngine.Object.Destroy(this.mergeMenu);
        UnityEngine.Object.Destroy(this.gameManager);
    }

    /// <summary>
    /// checking if the merge menu is inactive after the update list method is called.
    /// </summary>
    [Test]
    public void UpdateListActive()
    {
        Assert.IsTrue(this.mergeMenu.activeSelf);
        var env = this.gameManager.AddComponent<EnvironmentController>();
        env.SetFloatingDocuments(new List<FloatingDocument>());
        this.mergeDocuments.UpdateList();
        Assert.IsFalse(this.mergeMenu.activeSelf);
    }

    /// <summary>
    /// Checks if the merge menu is active and the dropdown has the correct options.
    /// </summary>
    [Test]
    public void UpdateListInactive()
    {
        this.mergeMenu.SetActive(false);

        var environmentInformation = this.gameManager.AddComponent<EnvironmentController>();
        var floatingDocument = this.document1.AddComponent<FloatingDocument>();
        floatingDocument.mergeButton = new GameObject("xxx");
        floatingDocument.pdfName = "Test 1";
        var floatingDocument2 = this.document2.AddComponent<FloatingDocument>();
        floatingDocument2.mergeButton = new GameObject("xxx");
        floatingDocument2.pdfName = "Test 2";
        var list = new List<FloatingDocument> { floatingDocument, floatingDocument2 };
        environmentInformation.SetFloatingDocuments(list);

        this.mergeDocuments.UpdateList();

        Assert.IsTrue(this.mergeMenu.activeSelf);
    }

    /// <summary>
    /// checks what happens when you try to merge with itself.
    /// </summary>
    [Test]
    public void OnClickMergeError()
    {
        this.mergeDocuments.self = 0;
        this.mergeDocuments.OnClickMerge();
        Assert.IsTrue(this.error.gameObject.activeSelf);
    }

    /// <summary>
    /// checks if the a document is merged properly.
    /// </summary>
    [Test]
    public void OnClickMerge()
    {
        var environmentInformation = this.gameManager.AddComponent<EnvironmentController>();

        var floatingDocument = this.document1.AddComponent<FloatingDocument>();
        floatingDocument.mergeButton = new GameObject("xxx");
        floatingDocument.pdfName = "Test 1";
        floatingDocument.pages = new List<int> { 0, 1 };
        floatingDocument.exportPages = new List<Tuple<string, string, int>>
        {
            new Tuple<string, string, int>("Test 1", "id", 0),
            new Tuple<string, string, int>("Test 1", "id", 1),
        };
        floatingDocument.sprites = new List<Sprite>
        {
            Sprite.Create(null, new Rect(0, 0, 1, 1), new Vector2(0, 0)),
            Sprite.Create(null, new Rect(0, 0, 1, 1), new Vector2(0, 0)),
        };
        floatingDocument.image = this.document1.AddComponent<UnityEngine.UI.Image>();

        var floatingDocument2 = this.document2.AddComponent<FloatingDocument>();
        floatingDocument2.mergeButton = new GameObject("xxx");
        floatingDocument2.pdfName = "Test 2";
        floatingDocument2.pages = new List<int> { 0, 1 };
        floatingDocument2.exportPages = new List<Tuple<string, string, int>>
        {
            new Tuple<string, string, int>("Test 2", "id", 0),
            new Tuple<string, string, int>("Test 2", "id", 1),
        };
        floatingDocument2.sprites = new List<Sprite>
        {
            Sprite.Create(null, new Rect(0, 0, 1, 1), new Vector2(0, 0)),
            Sprite.Create(null, new Rect(0, 0, 1, 1), new Vector2(0, 0)),
        };
        floatingDocument2.image = this.document2.AddComponent<UnityEngine.UI.Image>();

        var mergeManager = this.gameManager.GetComponent<MergeManager>();
        mergeManager.floatingDocument = floatingDocument2;

        this.mergeDocuments.OnClickMerge();

        Assert.IsFalse(this.error.gameObject.activeSelf);
        EnvironmentController environment = this.gameManager.GetComponent<EnvironmentController>();
        Assert.AreEqual(1, environment.GetFloatingDocuments().Count);
        var newFloatingDocument = environment.GetFloatingDocuments()[0];
        Assert.AreEqual(4, newFloatingDocument.exportPages.Count);
        Assert.AreEqual(4, newFloatingDocument.sprites.Count);
    }

    /// <summary>
    /// Test for the SetFloatingDocument method.
    /// </summary>
    [Test]
    public void SetFloatingDocument()
    {
        var environmentInformation = this.gameManager.AddComponent<EnvironmentController>();
        var floatingDocument = this.document1.AddComponent<FloatingDocument>();
        floatingDocument.mergeButton = new GameObject("xxx");
        floatingDocument.pdfName = "Test 1";
        var floatingDocument2 = this.document2.AddComponent<FloatingDocument>();
        floatingDocument2.mergeButton = new GameObject("xxx");
        floatingDocument2.pdfName = "Test 2";
        var list = new List<FloatingDocument> { floatingDocument, floatingDocument2 };
        environmentInformation.SetFloatingDocuments(list);

        this.mergeDocuments.SetFloatingDocument();

        Assert.AreEqual(floatingDocument, environmentInformation.GetFloatingDocuments()[0]);
        Assert.IsFalse(floatingDocument.mergeButton.activeSelf);
        Assert.IsFalse(floatingDocument2.mergeButton.activeSelf);
    }

    /// <summary>
    /// Tests the update method.
    /// </summary>
    /// <returns>its a unity test</returns>
    [UnityTest]
    public IEnumerator UpdateTest()
    {
        yield return null;
        Assert.AreEqual("Selected: Nothing", this.selected.text);

        var mergeManager = this.gameManager.GetComponent<MergeManager>();
        mergeManager.floatingDocument = this.document1.AddComponent<FloatingDocument>();
        mergeManager.floatingDocument.pdfName = "Test 1";

        yield return null;
        Assert.AreEqual("Selected: Test 1", this.selected.text);
    }
}
