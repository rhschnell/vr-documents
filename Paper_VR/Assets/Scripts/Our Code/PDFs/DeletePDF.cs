namespace Assets.Scripts.Our_Code.PDFs
{
    using System.Collections.Generic;

    /// <summary>
    /// The class containing the actual deletion of pages of a document
    /// </summary>
    public class DeletePDF
    {
        private readonly FloatingDocument floatingDocument;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeletePDF"/> class.
        /// Creates a new class that handles the deletion of pages of a document
        /// </summary>
        /// <param name="floatingDocument">the document that will have pages removed</param>
        public DeletePDF(FloatingDocument floatingDocument)
        {
            this.floatingDocument = floatingDocument;
        }

        /// <summary>
        /// Splits a floating document into two new floating documents.
        /// /// </summary>
        /// <param name="startPage"> The start page from which to split </param>
        /// <param name="endPage"> The end page at which to split </param>
        /// <returns> the new floating document </returns>
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
            for (var idx = endPage - 1; idx >= startPage - 1; idx--)
            {
                var sprite = this.floatingDocument.sprites[idx];
                this.floatingDocument.sprites.Remove(sprite);
            }

            for (int i = 0; i < prevLength - removed; i++)
            {
                this.floatingDocument.pages.Add(i);
            }

            this.floatingDocument.currentPageIndex = 0;
            this.floatingDocument.SetSprite();

            return this.floatingDocument;
        }
    }
}
