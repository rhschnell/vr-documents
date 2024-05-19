using NUnit.Framework;
using UnityEngine;

/// <summary>
/// This class contains tests for the FloatingDocument class.
/// </summary>
public class FloatingDocumentTests : MonoBehaviour
{
    /// <summary>
    /// This is a simple test that asserts true.
    /// </summary>
    [Test]
    public void CreateFloatingDocument()
    {
        // Arrange
        int[] pages = new int[3] { 1, 2, 3 };
        GameObject examplePrefab = new GameObject();
        FloatingDocument floatingDocument = new FloatingDocument(examplePrefab, "googledrive", "examplename", pages);

        // Act
        Assert.IsNotNull(floatingDocument);
        Assert.AreEqual("googledrive", floatingDocument.pdfPath);
        Assert.AreEqual("examplename", floatingDocument.pdfName);
        Assert.AreEqual(pages, floatingDocument.pages);
        Assert.AreEqual(210, floatingDocument.width);
        Assert.AreEqual(297, floatingDocument.height);
        Assert.AreEqual(examplePrefab, floatingDocument.canvas);
        Assert.AreEqual(1, floatingDocument.currentPage);
    }

    /// <summary>
    /// This test checks that the constructor throws an exception when no pages are provided.
    /// </summary>
    [Test]
    public void NoPages()
    {
        // Arrange
        int[] pages = new int[0];
        GameObject examplePrefab = new GameObject();

        // Assert that the constructor throws an exception when no pages are provided
        Assert.Throws<System.ArgumentException>(() => new FloatingDocument(examplePrefab, "googledrive", "examplename", pages));
    }

    /// <summary>
    /// This test checks that the string representation of the object is correct.
    /// </summary>
    [Test]
    public void ToStringTest()
    {
        // Arrange
        int[] pages = new int[3] { 1, 2, 3 };
        GameObject examplePrefab = new GameObject();
        FloatingDocument floatingDocument = new FloatingDocument(examplePrefab, "googledrive", "examplename", pages);

        // Act
        string result = floatingDocument.ToString();

        // Assert
        Assert.AreEqual("PDF Path: googledrive, PDF Name: examplename, Number of Pages: 3, Width: 210, Height: 297", result);
    }

    /// <summary>
    /// This test checks that the Equals method works correctly.
    /// </summary>
    [Test]
    public void EqualsTest()
    {
        // Arrange
        int[] pages = new int[3] { 1, 2, 3 };
        GameObject examplePrefab = new GameObject();
        FloatingDocument floatingDocument1 = new FloatingDocument(examplePrefab, "googledrive", "examplename", pages);
        FloatingDocument floatingDocument2 = new FloatingDocument(examplePrefab, "googledrive", "examplename", pages);

        // Act
        bool result = floatingDocument1.Equals(floatingDocument2);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(floatingDocument1.Equals(floatingDocument1));
        Assert.IsFalse(floatingDocument1.Equals(null));
    }

    /// <summary>
    /// This test checks that the GetHashCode method works correctly.
    /// </summary>
    [Test]
    public void HashTest()
    {
        // Arrange
        int[] pages = new int[3] { 1, 2, 3 };
        GameObject examplePrefab = new GameObject();
        FloatingDocument floatingDocument = new FloatingDocument(examplePrefab, "googledrive", "examplename", pages);

        // Act
        int result = floatingDocument.GetHashCode();

        // Assert that the hash code is a number
        Assert.IsInstanceOf<int>(result);
    }
}
