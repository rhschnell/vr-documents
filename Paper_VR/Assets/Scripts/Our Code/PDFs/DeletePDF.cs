using System.Collections.Generic;

/// <summary>
/// The class containing the actual deletion of pages of a document.
/// </summary>
public class DeletePDF
{
    private readonly FloatingDocument floatingDocument;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeletePDF"/> class.
    /// Creates a new class that handles the deletion of pages of a document.
    /// </summary>
    /// <param name="floatingDocument">The document that will have pages removed.</param>
    public DeletePDF(FloatingDocument floatingDocument)
    {
        this.floatingDocument = floatingDocument;
    }

    /// <summary>
    /// Deletes a subsection of a floating document.
    /// </summary>
    /// <param name="startPage">The start page of the subsection to delete.</param>
    /// <param name="endPage">The end page of the subsection to delete.</param>
    /// <returns>The new floating document.</returns>
    public FloatingDocument DeleteDocument(int startPage, int endPage)
    {
        for (int i = endPage - 1; i >= startPage - 1; i--)
        {
            var page = this.floatingDocument.exportPages[i];
            this.floatingDocument.exportPages.Remove(page);
        }

        int removed = endPage - startPage + 1;
        int prevLength = this.floatingDocument.pages.Count;
        this.floatingDocument.pages = new List<int>();

        // Removes the sprites according to the subsection
        for (var idx = endPage - 1; idx >= startPage - 1; idx--)
        {
            var sprite = this.floatingDocument.sprites[idx];
            this.floatingDocument.sprites.Remove(sprite);
        }

        // Reassign the pages of the floating document
        for (int i = 0; i < prevLength - removed; i++)
        {
            this.floatingDocument.pages.Add(i);
        }

        // Set the new floating documents currentPageIndex and show the frist page
        this.floatingDocument.currentPageIndex = 0;
        this.floatingDocument.SetSprite();

        return this.floatingDocument;
    }
}
