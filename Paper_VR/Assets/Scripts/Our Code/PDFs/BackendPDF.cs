using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityGoogleDrive;

/// <summary>
/// Class responsible for converting pdfs to sprites.
/// </summary>
public class BackendPDF : MonoBehaviour
{
    /// <summary>
    /// The URL for image conversion.
    /// </summary>
    public static string imageURL = "http://localhost:3000/convert-pdf-to-image";

    /// <summary>
    /// The URL for pdf extraction.
    /// </summary>
    public static string extractURL = "http://localhost:3000/extract-pages";

    /// <summary>
    /// This method converts pdfs to sprites.
    /// </summary>
    /// <param name="file">The file we want to get the sprites from.</param>
    /// <param name="floatingDocument">The floating doc we want to use.</param>
    /// <returns>The IENumarator.</returns>
    public virtual IEnumerator AddImagesToFloatingDocument(UnityGoogleDrive.Data.File file, FloatingDocument floatingDocument)
    {
        // Start the coroutine to send the pdf to the server
        yield return this.SendPdfToServer(file.Content, floatingDocument);
    }

    /// <summary>
    /// This methods sends the pdf to the server to be converted to images.
    /// It sends a request to the server to convert the pdf to images.
    /// The server returns a JSON response containing base64-encoded images.
    /// </summary>
    /// <param name="pdfBytes">The bytes to send to the server</param>
    /// <param name="floatingDocument">The floating document that is made with the images</param>
    /// <returns>IEnumerator</returns>
    public IEnumerator SendPdfToServer(byte[] pdfBytes, FloatingDocument floatingDocument)
    {
        // Create a UnityWebRequest to send the pdf to the server
        UnityWebRequest www = new UnityWebRequest(imageURL, "POST");
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(pdfBytes);
        uploadHandler.contentType = "application/pdf";
        www.uploadHandler = uploadHandler;
        www.downloadHandler = new DownloadHandlerBuffer();

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            // Create images from the JSON response containing base64-encoded images
            this.CreateImages(www.downloadHandler.text, floatingDocument);
        }
    }

    /// <summary>
    /// This method creates images from the JSON response containing base64-encoded images.
    /// It then creates a Texture2D and Sprite for each image.
    /// Then it adds the Sprite to the list of sprites in the floating document.
    /// </summary>
    /// <param name="jsonResponse">The JSON response from the server.</param>
    /// <param name="floatingDocument">The floating document to add the images to.</param>
    public void CreateImages(string jsonResponse, FloatingDocument floatingDocument)
    {
        // Initialize a list to store the sprites
        List<Sprite> sprites = new List<Sprite>();

        // Parse the JSON response containing base64-encoded images
        ImageDataList imageDataList = JsonUtility.FromJson<ImageDataList>("{\"images\":" + jsonResponse + "}");

        // Convert each base64 string to a byte array
        List<byte[]> imageBytesList = new List<byte[]>();
        foreach (string base64Image in imageDataList.images)
        {
            byte[] imageBytes = System.Convert.FromBase64String(base64Image);
            imageBytesList.Add(imageBytes);
        }

        // Create Texture2D and Sprite for each image (you can adjust this part based on your UI setup)
        foreach (byte[] imageBytes in imageBytesList)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.LoadImage(imageBytes);

            // Create a Sprite from the Texture2D
            Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            // Add the Sprite to the list
            sprites.Add(sprite);
        }

        // Set the list of sprites in the floating document
        floatingDocument.sprites = sprites;
        Sprite frontPage = sprites[0];

        // Remove the last sprite because it is a transparent image
        sprites.RemoveAt(sprites.Count - 1);

        // Set the list of pages in the floating document
        List<int> pages = new List<int>();
        int count = 0;
        foreach (Sprite sprite in sprites)
        {
            pages.Add(count);
            count++;
        }

        floatingDocument.pages = pages;

        Debug.Log(floatingDocument.scale.x);
        // Set the width and height of the floating document to match the size of the pdf
        if (floatingDocument.scale.x < 0.000001 || floatingDocument.scale.x > 0.5f)
        {
            Debug.Log("new document!");
            floatingDocument.scale.x = frontPage.rect.width;
            floatingDocument.scale.y = frontPage.rect.height;
        }

        floatingDocument.SetValues();
    }

    /// <summary>
    /// This method extacts all the pages from the pdf.
    /// And exports the new pdf to the drive.
    /// </summary>
    /// <param name="id">The id of the pdf file.</param>
    /// <param name="name">The name of the pdf file.</param>
    /// <param name="pages">The list of pages to extract.</param>
    /// <param name="folderId">The id of the folder to export to.</param>
    public virtual void ExtractPDF(string id, string name, List<int> pages, string folderId)
    {
        // Create a new request to download the pdf
        GoogleDriveFiles.DownloadRequest req = new GoogleDriveFiles.DownloadRequest(id);
        req.Send().OnDone += (UnityGoogleDrive.Data.File file) => this.StartCoroutine(this.SendPDFToServerExtract(file, name, pages, folderId));
    }

    /// <summary>
    /// This method sends the pdf to the server to be extracted.
    /// </summary>
    /// <param name="file">The pdf file to send to the server.</param>
    /// <param name="name">The name of the pdf file.</param>
    /// <param name="pages">The list of pages to extract.</param>
    /// <param name="folderId">The id of the folder to export to.</param>
    /// <returns>IEnumerator</returns>
    public IEnumerator SendPDFToServerExtract(UnityGoogleDrive.Data.File file, string name, List<int> pages, string folderId)
    {
        // Add 1 to each page number to match the server's page numbering
        List<int> pagesCopy = new List<int>();
        for (int i = 0; i < pages.Count; i++)
        {
            pagesCopy.Add(pages[i] + 1);
        }

        // Convert the list of pages to a comma-separated string
        string pagesString = string.Join(",", pagesCopy);

        // Create a UnityWebRequest to send the pdf to the server
        UnityWebRequest www = new UnityWebRequest(extractURL, "POST");
        UploadHandlerRaw uploadHandler = new UploadHandlerRaw(file.Content);
        uploadHandler.contentType = "application/pdf";
        www.uploadHandler = uploadHandler;
        www.downloadHandler = new DownloadHandlerBuffer();

        // Set the header with the pages as a comma-separated string
        www.SetRequestHeader("x-pdf-pages", pagesString);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            // Send the server response to the google drive
            this.StartCoroutine(this.ExportPDFToDrive(folderId, name, www.downloadHandler.data, null, false));
        }
    }

    /// <summary>
    /// This method exports the pdf to the drive.
    /// </summary>
    /// <param name="folderId">the id of the folder to export to</param>
    /// <param name="name">the name of the pdf</param>
    /// <param name="content">the content of the pdf</param>
    /// <param name="request">the request to use for mocking</param>
    /// <param name="testing">the boolean to check if the method is being tested</param>
    /// <returns>IEnumerator</returns>
    public virtual IEnumerator ExportPDFToDrive(string folderId, string name, byte[] content, GoogleDriveFiles.CreateRequest request, bool testing)
    {
        // Create a new file object.
        UnityGoogleDrive.Data.File newFile = new UnityGoogleDrive.Data.File { Name = name, Content = content, MimeType = "application/pdf" };
        newFile.Parents = new List<string> { folderId };

        // If the method is not being tested, create a new request
        if (!testing)
        {
            request = GoogleDriveFiles.Create(newFile);
        }

        request.Fields = new List<string> { "id" };

        yield return request.Send();
    }

    /// <summary>
    /// A class that contains the data of the images.
    /// </summary>
    [System.Serializable]
    internal class ImageDataList
    {
        /// <summary>
        /// A list of base64-encoded images.
        /// </summary>
        public List<string> images;
    }
}