using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PDFtoImage;
using UnityEngine;
using UnityGoogleDrive;
using UnityGoogleDrive.Data;

/// <summary>
/// Class responsible for converting pdfs to sprites.
/// </summary>
public class ConvertPDF : MonoBehaviour
{
    /// <summary>
    /// This method converts pdfs to sprites.
    /// </summary>
    /// <param name="file">The pdf being turned into a list of sprites.</param>
    /// <returns>A list of sprites for every page.</returns>
    public static List<Sprite> Convert(UnityGoogleDrive.Data.File file)
    {
        IEnumerable<SkiaSharp.SKBitmap> bitmaps = FileToSKBitmaps(file);
        List<Texture2D> textures = SKBitmapsToTextures2D(bitmaps);
        List<Sprite> sprites = Textures2DToSprites(textures);
        return sprites;
    }

    /// <summary>
    /// This method turns a file into a IEnumerable of bitmaps.
    /// </summary>
    /// <param name="file">The pdf being turned into a list of sprites.</param>
    /// <returns>The IEnumerable of bitmaps.</returns>
    private static IEnumerable<SkiaSharp.SKBitmap> FileToSKBitmaps(UnityGoogleDrive.Data.File file)
    {
        IEnumerable<SkiaSharp.SKBitmap> images = Conversion.ToImages(file.Content);
        return images;
    }

    /// <summary>
    /// This method turns a IEnumerable of bitmaps into a list of textures.
    /// </summary>
    /// <param name="bitmaps">The IEnumerable of bitmaps.</param>
    /// <returns>A list of textures.</returns>
    private static List<Texture2D> SKBitmapsToTextures2D(IEnumerable<SkiaSharp.SKBitmap> bitmaps)
    {
        List<Texture2D> textures = new List<Texture2D>();

        foreach (SkiaSharp.SKBitmap bitmap in bitmaps)
        {
            // Create a new Texture2D with the same dimensions as the SKBitmap
            Texture2D texture = new Texture2D(bitmap.Width, bitmap.Height, TextureFormat.RGBA32, false);

            // Copy the pixel data from the SKBitmap to the Texture2D
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    SkiaSharp.SKColor skColor = bitmap.GetPixel(x, y);
                    Color32 color = new Color32(skColor.Red, skColor.Green, skColor.Blue, skColor.Alpha);
                    texture.SetPixel(x, y, color);
                }
            }

            // Apply the changes to the Texture2D
            texture.Apply();

            textures.Add(texture);
        }

        return textures;
    }

    /// <summary>
    /// Turns a List of textures into a list of sprites.
    /// </summary>
    /// <param name="textures">A List of textures.</param>
    /// <returns>A list of sprites.</returns>
    private static List<Sprite> Textures2DToSprites(List<Texture2D> textures)
    {
        List<Sprite> sprites = new List<Sprite>();

        foreach (Texture2D texture in textures)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            sprites.Add(sprite);
        }

        return sprites;
    }
}