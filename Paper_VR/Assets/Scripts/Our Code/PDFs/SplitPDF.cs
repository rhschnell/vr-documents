using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class containing the actual split of two documents
/// </summary>
public class SplitPDF
{
    private FloatingDocument floatingDocument;
    private FloatingDocument newFloatingDocument;

    /// <summary>
    /// Initializes a new instance of the <see cref="SplitPDF"/> class.
    /// </summary>
    /// <param name="floatingDocument">the old floating document</param>
    /// <param name="newFloatingDocument">the newly created floated document</param>
    public SplitPDF(FloatingDocument floatingDocument, FloatingDocument newFloatingDocument)
    {
        this.floatingDocument = floatingDocument;
        this.newFloatingDocument = newFloatingDocument;
    }

    /// <summary>
    /// Splits a floating document into two new floating documents.
    /// </summary>
    /// <param name="startPage"> The start page from which to split </param>
    /// <param name="endPage"> The end page at which to split </param>
    /// <returns> the new floating document </returns>
    public FloatingDocument SplitDocument(int startPage, int endPage)
    {
        List<Tuple<string, string, int>> exportPages = new List<Tuple<string, string, int>>();
        for (int i = endPage - 1; i >= startPage - 1; i--)
        {
            var page = this.floatingDocument.exportPages[i];
            exportPages.Add(page);
            this.floatingDocument.exportPages.Remove(page);
        }

        exportPages.Reverse();
        this.newFloatingDocument.exportPages = exportPages;

        this.newFloatingDocument.pages = new List<int>();
        this.newFloatingDocument.sprites = new List<Sprite>();
        var pageIndex = 0;

        // Add pages to new floating document
        for (var idx = startPage - 1; idx < endPage; idx++)
        {
            var spriteIndex = this.floatingDocument.pages[idx];
            this.newFloatingDocument.pages.Add(pageIndex++);
            this.newFloatingDocument.sprites.Add(this.floatingDocument.sprites[spriteIndex]);
        }

        this.newFloatingDocument.currentPageIndex = 0;
        this.newFloatingDocument.SetSprite();

        var countBefore = this.floatingDocument.sprites.Count;
        // Remove pages from initial document
        foreach (var sprite in this.newFloatingDocument.sprites)
        {
            this.floatingDocument.sprites.Remove(sprite);
        }

        var countAfter = this.floatingDocument.sprites.Count;

        this.floatingDocument.pages = new List<int>();
        for (var pageIdx = 0; pageIdx < this.floatingDocument.sprites.Count; pageIdx++)
        {
            this.floatingDocument.pages.Add(pageIdx);
        }

        this.floatingDocument.currentPageIndex = 0;
        this.floatingDocument.SetSprite();

        return this.floatingDocument;
    }
}
