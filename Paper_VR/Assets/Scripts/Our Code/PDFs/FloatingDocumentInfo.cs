using System.Collections.Generic;
using UnityEngine;

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
    /// The ID of the PDF file.
    /// </summary>
    public string pdfId;

    /// <summary>
    /// The name of the PDF file.
    /// </summary>
    public string pdfName;

    /// <summary>
    /// an array of the pages of the PDF file.
    /// </summary>
    public List<int> pages;

    /// <summary>
    /// The current page of the PDF file.
    /// </summary>
    public int currentPage;

    /// <summary>
    /// The width of the PDF file.
    /// </summary>
    public float width;

    /// <summary>
    /// The height of the PDF file.
    /// </summary>
    public float height;

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
        this.position = doc.transform.position;
        this.rotation = doc.transform.rotation;
        this.scale = doc.transform.localScale;
        this.pdfId = doc.pdfId;
        this.pdfName = doc.pdfName;
        this.pages = doc.pages;
        this.currentPage = doc.currentPageIndex;
        this.width = doc.width;
        this.height = doc.height;
    }
}