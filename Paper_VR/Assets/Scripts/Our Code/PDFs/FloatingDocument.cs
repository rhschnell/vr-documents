using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// This class is used to represent a floating document in the scene.
/// It has a reference to the PDF file that it represents.
/// </summary>
public class FloatingDocument : MonoBehaviour
{
    /// <summary>
    /// The input reference of the up action.
    /// </summary>
    public InputActionReference inputActionReference;

    /// <summary>
    /// The merge button that is used to merge the floating document with another floating document.
    /// </summary>
    public GameObject mergeButton;

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
    /// The time that needs to be waited between scrolls.
    /// </summary>
    public float scrollWaitingTime = 0.2f;

    /// <summary>
    /// The list with all pages as sprites.
    /// </summary>
    public List<Sprite> sprites;

    /// <summary>
    /// The canvas of the PDF file.
    /// </summary>
    public Image image;

    /// <summary>
    /// The id of the PDF file.
    /// </summary>
    public string pdfId;

    /// <summary>
    /// The name of the PDF file.
    /// </summary>
    public string pdfName;

    /// <summary>
    /// an array of the page numbers of the PDF file in the order of the floating doc.
    /// </summary>
    public List<int> pages;

    /// <summary>
    /// A list containing the pages that need to be exported in the format of (pdfName, pdfId, pageNumber).
    /// </summary>
    public List<Tuple<string, string, int>> exportPages;

    /// <summary>
    /// The current page of the PDF file.
    /// </summary>
    public int currentPageIndex;

    /// <summary>
    /// Whether there is being hovered.
    /// </summary>
    public bool isHovering = false;
    private bool available;
    private Vector2 joystickValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="FloatingDocument"/> class.
    /// Constructor for the FloatingDocument class.
    /// </summary>
    /// <param name="position">The initial position of the floating document</param>
    /// <param name="rotation">The initial rotation of the floating document</param>
    /// <param name="scale">The intial scale of the floating document</param>
    /// <param name="pdfId">The ID of the pdf</param>
    /// <param name="pdfName">The name of the pdf</param>
    /// <param name="pages">The pages of the pdf</param>
    /// <param name="exportPages">The pages that need to be exported</param>
    public void SetAttributes(
        Vector3 position,
        Quaternion rotation,
        Vector3 scale,
        string pdfId,
        string pdfName,
        List<int> pages,
        List<Tuple<string, string, int>> exportPages)
    {
        // Set the attributes of the pdf
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
        this.pdfId = pdfId;
        this.pdfName = pdfName;
        this.pages = pages;
        this.exportPages = exportPages;
        if (pages.Count <= 0)
        {
            throw new System.ArgumentException("The number of pages must be greater than 0");
        }

        this.currentPageIndex = 0;
    }

    /// <summary>
    /// Returns a string representation of the FloatingDocument.
    /// </summary>
    /// <returns>The attributes of the pdf.</returns>
    public override string ToString()
    {
        // Return the attributes of the pdf
        return "PDF Path: " + this.pdfId +
            ", PDF Name: " + this.pdfName +
            ", Number of Pages: " + this.pages.Count;
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

        // Check if the attributes of the pdf are the same
        FloatingDocument other = (FloatingDocument)obj;
        if (other == this)
        {
            return true;
        }

        // Return if the PDFs are equal
        return this.pdfId == other.pdfId &&
            this.pdfName == other.pdfName &&
            Enumerable.SequenceEqual(this.pages, other.pages) &&
            Enumerable.SequenceEqual(this.exportPages, other.exportPages) &&
            this.currentPageIndex == other.currentPageIndex &&
            this.gameObject.transform.position == other.gameObject.transform.position &&
            this.gameObject.transform.rotation == other.gameObject.transform.rotation &&
            this.gameObject.transform.localScale == other.gameObject.transform.localScale;
    }

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>
    /// Returns a hash code for the current object.
    /// </returns>
    public override int GetHashCode()
    {
        // Return the hash code of the pdf
        return base.GetHashCode();
    }

    /// <summary>
    /// Set the values of the canvas.
    /// </summary>
    public void SetValues()
    {
        // Calculate the width and height dimensions based on the current sprite width and height
        Sprite currentSprite = this.sprites[this.currentPageIndex];
        this.image.sprite = currentSprite;
        Vector2 dimensions = this.CalculateWidthAndHeight(currentSprite.rect.width, currentSprite.rect.height);

        this.transform.localScale = new Vector3(dimensions.x, dimensions.y, this.transform.localScale.z);
    }

    /// <summary>
    /// When called, we scroll up.
    /// </summary>
    public void ScrollUp()
    {
        // Check if the current page is not the first page
        if (this.currentPageIndex - 1 >= 0)
        {
            // Show the page before the shown page
            this.currentPageIndex--;
            this.SetSprite();
        }
    }

    /// <summary>
    /// When called, we scroll down.
    /// </summary>
    public void ScrollDown()
    {
        // Check if the current page is not the last page
        if (this.currentPageIndex + 1 < this.pages.Count)
        {
            // Show the page after the shownp page
            this.currentPageIndex++;
            this.SetSprite();
        }
    }

    /// <summary>
    /// Sets the sprite to the current pages sprite.
    /// </summary>
    public void SetSprite()
    {
        // Set the sprite with the currentPageIndex and the pages of a floating document
        this.image.sprite = this.sprites[this.pages[this.currentPageIndex]];
    }

    /// <summary>
    /// Set the hovering value to the hovering value passed through.
    /// </summary>
    /// <param name="isHovering">The is hovering value.</param>
    public void IsHovering(bool isHovering)
    {
        // Set the hovering boolean
        this.isHovering = isHovering;
    }

    /// <summary>
    /// Method that calculates the width and height based on relation between width and height.
    /// </summary>
    /// <param name="width">Old width.</param>
    /// <param name="height">Old height.</param>
    /// <returns>The new width and height.</returns>
    private Vector2 CalculateWidthAndHeight(float width, float height)
    {
        // Calculates the width and height
        Vector2 dimensions = default(Vector2);
        dimensions.x = Mathf.Sqrt(0.0001f * width / height);
        dimensions.y = 0.0001f / dimensions.x;

        return dimensions;
    }

    /// <summary>
    /// Called once per frame to update the joystick value and check for scrolling.
    /// </summary>
    private void Update()
    {
        // Update the current joystick value from the input action referenc
        this.SetJoystickValue();

        // Check the joystick input and scroll the content if necessary
        this.ScrollCheck();
    }

    /// <summary>
    /// Initializes the component when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        // Set available to true
        this.available = true;
    }

    /// <summary>
    /// Sets the current joystick value from the input action reference if it is available.
    /// </summary>
    private void SetJoystickValue()
    {
        if (this.inputActionReference != null)
        {
            // Read the joystick value
            this.joystickValue = this.inputActionReference.action.ReadValue<Vector2>();
        }
    }

    /// <summary>
    /// Checks the joystick input and scrolls the content up or down if certain conditions are met
    /// </summary>
    private void ScrollCheck()
    {
        // Check if scrolling is available and the user is hovering over the scrollable area
        if (this.available && this.isHovering)
        {
            // Check if the joystick is being pushed downward
            if (this.joystickValue.y < -0.5f)
            {
                this.ScrollDown();
                this.StartCoroutine(this.LockAndUnlock());
            }

            // Check if the joystick is being pushed upward
            else if (this.joystickValue.y > 0.5f)
            {
                this.ScrollUp();
                this.StartCoroutine(this.LockAndUnlock());
            }
        }
    }

    /// <summary>
    /// Handles that the scroll functionality scrolls only one page per scroll.
    /// </summary>
    /// <returns>An IEnumerator</returns>
    IEnumerator LockAndUnlock()
    {
        this.available = false;

        // Wait scrollWaitingTime seconds so that the user can scroll page for page
        yield return new WaitForSeconds(this.scrollWaitingTime);

        this.available = true;
    }
}