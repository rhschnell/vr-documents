using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Class containing tests for the delete document functionality.
/// </summary>
public class DeleteTest
{
    /// <summary>
    /// Test for deleting a subsection of a document, the pages should be removed from the document and the menu should be hidden.
    /// </summary>
    [Test]
    public void TestDeleteDocumentSubsection()
    {
        // Creating the objects
        GameObject go = new GameObject();
        GameObject menu = new GameObject();
        GameObject subsectionMenu = new GameObject();
        GameObject pdfCanvasPrefab = new GameObject();

        DeleteController deleteDocument = go.AddComponent<DeleteController>();
        MenuController menuController = go.AddComponent<MenuController>();
        deleteDocument.MenuController = menuController;
        deleteDocument.pdfPrefab = pdfCanvasPrefab;

        FloatingDocument floatingDocument = pdfCanvasPrefab.AddComponent<FloatingDocument>();
        floatingDocument.pages = new List<int> { 1, 2, 3, 4, 5 };
        string name = "test";
        string id = "123";
        floatingDocument.exportPages = new List<Tuple<string, string, int>>
        {
            new Tuple<string, string, int>(name, id, 1),
            new Tuple<string, string, int>(name, id, 2),
            new Tuple<string, string, int>(name, id, 3),
            new Tuple<string, string, int>(name, id, 4),
            new Tuple<string, string, int>(name, id, 5),
        };
        floatingDocument.sprites = new List<Sprite> {
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
        };
        floatingDocument.currentPageIndex = 0;
        floatingDocument.image = pdfCanvasPrefab.AddComponent<UnityEngine.UI.Image>();

        menuController.StartPageNumber = 1;
        menuController.EndPageNumber = 3;
        menuController.MaxPage = 5;
        menuController.pdfPrefab = pdfCanvasPrefab;
        menuController.Menu = menu;
        menu.SetActive(true);
        menuController.subsectionMenu = subsectionMenu;
        subsectionMenu.SetActive(true);

        deleteDocument.OnClickDeleteSubsection();

        Assert.AreEqual(2, floatingDocument.pages.Count);
        Assert.AreEqual(0, floatingDocument.pages[0]);
        Assert.AreEqual(false, menu.activeSelf);
        Assert.AreEqual(false, subsectionMenu.activeSelf);

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(menu);
        UnityEngine.Object.Destroy(subsectionMenu);
        UnityEngine.Object.Destroy(pdfCanvasPrefab);
    }

    /// <summary>
    /// Test to see if deleting an individual page works, the page should be removed from the document and the menu should be hidden.
    /// </summary>
    [Test]
    public void TestDeleteDocumentPage()
    {
        // Creating the objects
        GameObject go = new GameObject();
        GameObject menu = new GameObject();
        GameObject subsectionMenu = new GameObject();
        GameObject pdfCanvasPrefab = new GameObject();

        DeleteController deleteDocument = go.AddComponent<DeleteController>();
        MenuController menuController = go.AddComponent<MenuController>();
        deleteDocument.MenuController = menuController;
        deleteDocument.pdfPrefab = pdfCanvasPrefab;

        FloatingDocument floatingDocument = pdfCanvasPrefab.AddComponent<FloatingDocument>();
        floatingDocument.pages = new List<int> { 1, 2, 3, 4, 5 };
        string name = "test";
        string id = "123";
        floatingDocument.exportPages = new List<Tuple<string, string, int>>
        {
            new Tuple<string, string, int>(name, id, 1),
            new Tuple<string, string, int>(name, id, 2),
            new Tuple<string, string, int>(name, id, 3),
            new Tuple<string, string, int>(name, id, 4),
            new Tuple<string, string, int>(name, id, 5),
        };
        floatingDocument.sprites = new List<Sprite> {
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
        };
        floatingDocument.currentPageIndex = 0;
        floatingDocument.image = pdfCanvasPrefab.AddComponent<UnityEngine.UI.Image>();

        menuController.StartPageNumber = 1;
        menuController.EndPageNumber = 3;
        menuController.MaxPage = 5;
        menuController.pdfPrefab = pdfCanvasPrefab;
        menuController.Menu = menu;
        menu.SetActive(true);
        menuController.subsectionMenu = subsectionMenu;
        subsectionMenu.SetActive(true);

        deleteDocument.OnClickDeletePage();

        Assert.AreEqual(4, floatingDocument.pages.Count);
        Assert.AreEqual(0, floatingDocument.pages[0]);
        Assert.AreEqual(false, menu.activeSelf);
        Assert.AreEqual(false, subsectionMenu.activeSelf);

        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(menu);
        UnityEngine.Object.Destroy(subsectionMenu);
        UnityEngine.Object.Destroy(pdfCanvasPrefab);
    }

    /// <summary>
    /// Test to see if deleting the full document works.
    /// </summary>
    [Test]
    public void TestDeleteDocument()
    {
        // Creating the objects
        GameObject go = new GameObject();
        GameObject menu = new GameObject();
        GameObject subsectionMenu = new GameObject();
        GameObject pdfCanvasPrefab = new GameObject();

        DeleteController deleteDocument = go.AddComponent<DeleteController>();
        MenuController menuController = go.AddComponent<MenuController>();
        deleteDocument.MenuController = menuController;
        deleteDocument.pdfPrefab = pdfCanvasPrefab;

        FloatingDocument floatingDocument = pdfCanvasPrefab.AddComponent<FloatingDocument>();
        floatingDocument.pages = new List<int> { 1, 2, 3, 4, 5 };
        floatingDocument.sprites = new List<Sprite> {
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
            Sprite.Create(null, default(Rect), new Vector2(1, 2)),
        };
        floatingDocument.currentPageIndex = 0;
        floatingDocument.image = pdfCanvasPrefab.AddComponent<UnityEngine.UI.Image>();
        GameObject gameManager = new GameObject("GameManager");
        EnvironmentController environment = gameManager.AddComponent<EnvironmentController>();
        environment.GetFloatingDocuments().Add(floatingDocument);
        menuController.StartPageNumber = 1;
        menuController.EndPageNumber = 3;
        menuController.MaxPage = 5;
        menuController.pdfPrefab = pdfCanvasPrefab;
        menuController.Menu = menu;
        menu.SetActive(true);
        menuController.subsectionMenu = subsectionMenu;
        subsectionMenu.SetActive(true);

        deleteDocument.OnClickDeleteDocument();

        Assert.Null(deleteDocument.pdfPrefab);

        UnityEngine.Object.Destroy(gameManager);
        UnityEngine.Object.Destroy(environment);
        UnityEngine.Object.Destroy(go);
        UnityEngine.Object.Destroy(menu);
        UnityEngine.Object.Destroy(subsectionMenu);
        UnityEngine.Object.Destroy(pdfCanvasPrefab);
    }
}
