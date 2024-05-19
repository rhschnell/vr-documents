using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// This class is responsible for converting a PDF file to an image.
/// </summary>
public class PDFImage : MonoBehaviour
{
    /// <summary>
    /// The path to the directory containing the PDFs.
    /// </summary>
    public string path = "Resources\\PDFs";

    /// <summary>
    /// This method converts a PDF file to an image.
    /// </summary>
    /// <param name="completePath">The completePath of the pdf</param>
    /// <param name="firstPage">The first page of the pdf</param>
    /// <param name="lastPage">The last page of the pdf</param>
    public void ConvertPDFToImage(string completePath, int firstPage, int lastPage)
    {
        // Create a converter object
        ConvertPDF converter = new ConvertPDF();

        // Get the pdf name
        string pdfName = Path.GetFileNameWithoutExtension(completePath);

        string imageName = pdfName + "-%d.jpg";

        string jpegOutput = Path.Combine("Assets\\Resources", imageName);

        converter.Convert(completePath, jpegOutput, firstPage, lastPage, "jpeg", 210, 297);

        Debug.Log(jpegOutput);
    }

    /// <summary>
    /// This method converts all the PDFs in the directory to images.
    /// </summary>
    public void ConvertAllPDFsToImages()
    {
        // Get all the files in the directory
        string completePath = Path.Combine(Application.dataPath, this.path);
        var info = new DirectoryInfo(completePath);
        var fileInfo = info.GetFiles();

        // Loop through all the files in the directory
        foreach (var file in fileInfo)
        {
            if (file.Extension == ".pdf")
            {
                this.ConvertPDFToImage(file.FullName, 1, 3);
            }
        }
    }

    void Awake()
    {
        // Convert all the PDFs to images
        this.ConvertAllPDFsToImages();
    }
}
