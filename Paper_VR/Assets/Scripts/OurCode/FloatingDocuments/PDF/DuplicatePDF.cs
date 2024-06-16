using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles the duplication of entire documents or pages of a document.
/// </summary>
public class DuplicatePDF
{
    private FloatingDocument floatingDocument;
    private FloatingDocument newFloatingDocument;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuplicatePDF"/> class.
    /// Creates a new class that handles the duplication of entire documents or pages of a document.
    /// </summary>
    /// <param name="floatingDocument">The document that will have pages removed.</param>
    /// <param name="newFloatingDocument">The new document that will be created.</param>
    public DuplicatePDF(FloatingDocument floatingDocument, FloatingDocument newFloatingDocument)
    {
        // Set the floatingDocument and newFloatingDocument fields
        this.floatingDocument = floatingDocument;
        this.newFloatingDocument = newFloatingDocument;
    }

    /// <summary>
    /// Duplicates the floating document from given indexes.
    /// </summary>
    /// <param name="startPage">The page where to start.</param>
    /// <param name="endPage">The page where to end.</param>
    /// <returns>The new floating document setup.</returns>
    public FloatingDocument DuplicateDocument(int startPage, int endPage)
    {
        List<Sprite> subsectionSprites = new List<Sprite>();
        List<Tuple<string, string, int>> exportPages = new List<Tuple<string, string, int>>();

        // Add the sprites in the subsection to a new list
        for (int i = startPage; i <= endPage; i++)
        {
            subsectionSprites.Add(this.floatingDocument.sprites[i]);
            exportPages.Add(this.floatingDocument.exportPages[i]);
        }

        // Set the fields of the new floating document
        this.newFloatingDocument.pdfId = this.floatingDocument.pdfId;
        this.newFloatingDocument.pages = new List<int>();
        for (int i = 0; i <= endPage - startPage; i++)
        {
            this.newFloatingDocument.pages.Add(i);
        }

        this.newFloatingDocument.currentPageIndex = 0;
        this.newFloatingDocument.sprites = subsectionSprites;
        this.newFloatingDocument.exportPages = exportPages;
        this.newFloatingDocument.SetSprite();

        return this.newFloatingDocument;
    }
}
