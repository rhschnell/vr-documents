using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// This class is used to represent a floating document in the scene.
/// It has a reference to the PDF file that it represents.
/// </summary>
public class FloatingDocument
{
    /// <summary>
    /// The canvas of the PDF file.
    /// </summary>
    public GameObject canvas;

    /// <summary>
    /// The path to the PDF file.
    /// </summary>
    public string pdfPath;

    /// <summary>
    /// The name of the PDF file.
    /// </summary>
    public string pdfName;

    /// <summary>
    /// an array of the pages of the PDF file.
    /// </summary>
    public int[] pages;

    /// <summary>
    /// The current page of the PDF file.
    /// </summary>
    public int currentPage;

    /// <summary>
    /// The width of the PDF file.
    /// </summary>
    public int width;

    /// <summary>
    /// The height of the PDF file.
    /// </summary>
    public int height;

    private string imagePath = "Sprites";

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatingDocument"/> class.
    /// Constructor for the FloatingDocument class.
    /// </summary>
    /// <param name="canvas">The object in the scene</param>
    /// <param name="pdfPath">The path to the pdf</param>
    /// <param name="pdfName">The name of the pdf</param>
    /// <param name="pages">The pages of the pdf</param>
    public FloatingDocument(GameObject canvas, string pdfPath, string pdfName, int[] pages)
    {
        // set the attributes of the pdf
        this.canvas = canvas;
        this.pdfPath = pdfPath;
        this.pdfName = pdfName;
        this.pages = pages;
        this.width = 210;
        this.height = 297;
        if (pages.Length <= 0)
        {
            throw new System.ArgumentException("The number of pages must be greater than 0");
        }

        this.currentPage = pages[0];
    }

    /// <summary>
    /// Returns a string representation of the FloatingDocument.
    /// </summary>
    /// <returns>the attributes of the pdf</returns>
    public override string ToString()
    {
        // return the attributes of the pdf
        return "PDF Path: " + this.pdfPath +
            ", PDF Name: " + this.pdfName +
            ", Number of Pages: " + this.pages.Length +
            ", Width: " + this.width +
            ", Height: " + this.height;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>
    /// Returns true if the specified object is equal to the current object; otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj == null || this.GetType() != obj.GetType())
        {
            return false;
        }

        // check if the attributes of the pdf are the same
        FloatingDocument other = (FloatingDocument)obj;
        return this.pdfPath == other.pdfPath &&
            this.pdfName == other.pdfName &&
            Enumerable.SequenceEqual(this.pages, other.pages) &&
            this.currentPage == other.currentPage &&
            this.width == other.width &&
            this.height == other.height;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>
    /// Returns a hash code for the current object.
    /// </returns>
    public override int GetHashCode()
    {
        // return the hash code of the pdf
        return base.GetHashCode();
    }
}
