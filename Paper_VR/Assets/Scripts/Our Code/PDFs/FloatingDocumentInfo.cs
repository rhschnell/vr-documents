using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// This class is used to represent a floating document in the scene.
/// It has a reference to the PDF file that it represents.
/// </summary>
public class FloatingDocumentInfo
{
    /// <summary>
    /// The position of the floating document in the scene.
    /// </summary>
    public Vector3 position;

    /// <summary>
    /// The rotation of the floating document in the scene.
    /// </summary>
    public Quaternion rotation;

    /// <summary>
    /// The scale of the floating document in the scene.
    /// </summary>
    public Vector3 scale;

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
    /// Initializes a new instance of the <see cref="FloatingDocumentInfo"/> class.
    /// The default constructor for the FloatingDocument class.
    /// </summary>
    public FloatingDocumentInfo()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatingDocumentInfo"/> class.
    /// Constructor for the FloatingDocument class.
    /// </summary>
    /// <param name="doc">The floating document</param>
    public FloatingDocumentInfo(FloatingDocument doc)
    {
        // set the attributes of the pdf
        this.position = doc.canvas.transform.position;
        this.rotation = doc.canvas.transform.rotation;
        this.scale = doc.canvas.transform.localScale;
        this.pdfPath = doc.pdfPath;
        this.pdfName = doc.pdfName;
        this.pages = doc.pages;
        this.currentPage = doc.currentPage;
        this.width = doc.width;
        this.height = doc.height;
    }
}
