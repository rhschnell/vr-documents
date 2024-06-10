using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
        GameObject go = new GameObject();
        GameObject menu = new GameObject();
        GameObject subsectionMenu = new GameObject();
        GameObject pdfCanvasPrefab = new GameObject();

        DeleteDocument deleteDocument = go.AddComponent<DeleteDocument>();
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

        Object.Destroy(go);
        Object.Destroy(menu);
        Object.Destroy(subsectionMenu);
        Object.Destroy(pdfCanvasPrefab);
    }

    /// <summary>
    /// Test to see if deleting an individual page works, the page should be removed from the document and the menu should be hidden.
    /// </summary>
    [Test]
    public void TestDeleteDocumentPage()
    {
        GameObject go = new GameObject();
        GameObject menu = new GameObject();
        GameObject subsectionMenu = new GameObject();
        GameObject pdfCanvasPrefab = new GameObject();

        DeleteDocument deleteDocument = go.AddComponent<DeleteDocument>();
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

        Object.Destroy(go);
        Object.Destroy(menu);
        Object.Destroy(subsectionMenu);
        Object.Destroy(pdfCanvasPrefab);
    }

    /// <summary>
    /// Test to see if deleting the full document works.
    /// </summary>
    [Test]
    public void TestDeleteDocument()
    {
        GameObject go = new GameObject();
        GameObject menu = new GameObject();
        GameObject subsectionMenu = new GameObject();
        GameObject pdfCanvasPrefab = new GameObject();

        DeleteDocument deleteDocument = go.AddComponent<DeleteDocument>();
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
        EnvironmentInformation environment = gameManager.AddComponent<EnvironmentInformation>();
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

        Object.Destroy(gameManager);
        Object.Destroy(environment);
        Object.Destroy(go);
        Object.Destroy(menu);
        Object.Destroy(subsectionMenu);
        Object.Destroy(pdfCanvasPrefab);
    }
}
