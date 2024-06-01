using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class responsible for converting pdfs to sprites.
/// </summary>
public class ConvertPDF : MonoBehaviour
{
    /// <summary>
    /// The URL of the server that converts pdfs to images.
    /// </summary>
    public static string serverURL = "http://localhost:3000/convert-pdf-to-image";

    /// <summary>
    /// This method converts pdfs to sprites.
    /// </summary>
    /// <param name="file">The pdf being turned into a list of sprites.</param>
    /// <param name="floatingDocument">The floating document where the sprites will be stored.</param>
    public virtual void Convert(UnityGoogleDrive.Data.File file, FloatingDocument floatingDocument)
    {
        // Start the coroutine to send the pdf to the server
        this.StartCoroutine(this.SendPdfToServer(file.Content, floatingDocument));
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
        UnityWebRequest www = new UnityWebRequest(serverURL, "POST");
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

        floatingDocument.sprites = sprites;
        Sprite frontPage = sprites[0];
        floatingDocument.width = frontPage.rect.width;
        floatingDocument.height = frontPage.rect.height;

        List<int> pages = new List<int>();
        int count = 0;
        foreach (Sprite sprite in sprites)
        {
            pages.Add(count);
            count++;
        }

        floatingDocument.pages = pages;
        floatingDocument.SetValues();
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