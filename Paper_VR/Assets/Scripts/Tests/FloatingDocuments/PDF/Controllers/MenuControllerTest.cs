using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class tests the menu controller.
/// </summary>
public class MenuControllerTest : MonoBehaviour
{
    /// <summary>
    /// The game manager
    /// </summary>
    public GameObject gameManager;

    /// <summary>
    /// the object for the document menu/menu controller
    /// </summary>
    public GameObject documentMenuObject;

    /// <summary>
    /// the document menu/menu controller
    /// </summary>
    public MenuController documentMenu;

    /// <summary>
    /// this is the menu
    /// </summary>
    public GameObject menu;

    /// <summary>
    /// this is the floating document
    /// </summary>
    public FloatingDocument floatingDocument;

    /// <summary>
    /// this is the environment controller
    /// </summary>
    private EnvironmentController envinfo;

    /// <summary>
    /// Setup for the tests.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        this.gameManager = new GameObject("GameManager");
        Assert.NotNull(this.gameManager, "Could not create gameManager : GameManager");

        this.documentMenuObject = new GameObject("DocumentMenu");
        this.documentMenu = this.documentMenuObject.AddComponent<MenuController>();
        Assert.NotNull(this.documentMenu, "Could not create documentMenu: DocumentMenu");

        this.menu = new GameObject("Menu");
        this.documentMenu.Menu = this.menu;
        this.documentMenu.subsectionMenu = new GameObject("SubsectionMenu");
        this.documentMenu.mergeMenu = new GameObject("MergeMenu");
        this.documentMenu.pdfPrefab = new GameObject("PdfCanvas");

        this.floatingDocument = this.documentMenu.pdfPrefab.AddComponent<FloatingDocument>();
        Assert.NotNull(this.floatingDocument, "Could not create floatingDocument: FloatingDocument");
        this.floatingDocument.mergeButton = new GameObject();
        this.floatingDocument.pdfId = "some/path/to/original.pdf";
        this.floatingDocument.pages = new List<int> { 0, 1, 2, 3, 4 };
        string name = "test";
        string id = "123";
        this.floatingDocument.exportPages = new List<Tuple<string, string, int>>
        {
            new Tuple<string, string, int>(name, id, 1),
            new Tuple<string, string, int>(name, id, 2),
            new Tuple<string, string, int>(name, id, 3),
            new Tuple<string, string, int>(name, id, 4),
            new Tuple<string, string, int>(name, id, 5),
        };

        this.envinfo = this.gameManager.AddComponent<EnvironmentController>();
        var floatingDocuments = new List<FloatingDocument> { this.floatingDocument };
        this.envinfo.SetFloatingDocuments(floatingDocuments);
    }

    /// <summary>
    /// This teardown destroys all created objects from the setup
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        Destroy(this.envinfo);
        Destroy(this.floatingDocument);
        Destroy(this.documentMenuObject);
        Destroy(this.gameManager);
    }

    /// <summary>
    /// This tests the toggle menu method.
    /// </summary>
    [Test]
    public void TestToggleMenu()
    {
        // Test when hover is true
        this.documentMenu.hover = true;
        this.documentMenu.Menu.SetActive(false);
        this.documentMenu.OpenAndCloseMenu(default(InputAction.CallbackContext));

        Assert.IsTrue(this.documentMenu.Menu.activeSelf);

        // Test when hover is false
        this.documentMenu.hover = true;
        this.documentMenu.Menu.SetActive(true);
        this.documentMenu.ToggleMainMenu();

        Assert.IsFalse(this.documentMenu.Menu.activeSelf);
        Assert.IsFalse(this.documentMenu.subsectionMenu.activeSelf);
        Assert.IsFalse(this.documentMenu.mergeMenu.activeSelf);
    }

    /// <summary>
    /// This tests the on hover method.
    /// </summary>
    [Test]
    public void TestOnHover()
    {
        // Test setting hover to true
        this.documentMenu.OnHover(true);
        Assert.IsTrue(this.documentMenu.hover);

        // Test setting hover to false
        this.documentMenu.OnHover(false);
        Assert.IsFalse(this.documentMenu.hover);
    }
}
