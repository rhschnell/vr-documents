using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class handeling the merging of two PDFs, creating a new PDF.
/// </summary>
public class MergePDF
{
    FloatingDocument pdf1;
    FloatingDocument pdf2;
    FloatingDocument newFloatingDocument;

    /// <summary>
    /// Initializes a new instance of the <see cref="MergePDF"/> class.
    /// Contructor for the MergePDF class.
    /// </summary>
    /// <param name="pdf1">The first pdf to merge</param>
    /// <param name="pdf2">The second pdf to merge</param>
    /// <param name="newFloatingDocument">The pdf to merge it to</param>
    public MergePDF(FloatingDocument pdf1, FloatingDocument pdf2, FloatingDocument newFloatingDocument)
    {
        // Sets the pdfs and the new pdf to the class variables.
        this.pdf1 = pdf1;
        this.pdf2 = pdf2;
        this.newFloatingDocument = newFloatingDocument;
    }

    /// <summary>
    /// Excutes the merge of the two PDFs. puts the sprites and pages of the two PDFs into the new PDF. Also combines the page numbers and export pages.
    /// </summary>
    public void Merge()
    {
        int length1 = this.pdf1.pages.Count;

        // combines the sprites and pages of the two PDFs into the new PDF.
        List<Sprite> sprites = new List<Sprite>();
        sprites.AddRange(this.pdf1.sprites);
        sprites.AddRange(this.pdf2.sprites);
        this.newFloatingDocument.sprites = sprites;

        // combines the pages and export pages of the two PDFs into the new PDF, makes sure no overlapping pages are added.
        List<int> pages = new List<int>();
        pages.AddRange(this.pdf1.pages);
        foreach (int page in this.pdf2.pages)
        {
            pages.Add(page + length1);
        }

        // combines the export pages of the two PDFs into the new PDF.
        List<Tuple<string, string, int>> exportPages = new List<Tuple<string, string, int>>();
        exportPages.AddRange(this.pdf1.exportPages);
        exportPages.AddRange(this.pdf2.exportPages);

        // sets the new PDFs pages, export pages and sprite.
        this.newFloatingDocument.pages = pages;
        this.newFloatingDocument.exportPages = exportPages;

        this.newFloatingDocument.currentPageIndex = 0;
        this.newFloatingDocument.SetSprite();
    }
}
