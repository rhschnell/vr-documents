using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// The class responsible for scrolling up and down.
/// </summary>
public class Scroll : MonoBehaviour
{
    /// <summary>
    /// The input refernce of the up action.
    /// </summary>
    public InputActionReference inputActionReference;

    /// <summary>
    /// The time that needs to be waited between scrolls.
    /// </summary>
    public float scrollWaitingTime = 1f;

    /// <summary>
    /// Floating doc script.
    /// </summary>
    public FloatingDocument floatingDocument;

    private bool isHovering = false;
    private bool available = true;
    private Vector2 joystickValue;

    /// <summary>
    /// When called, we scroll up.
    /// </summary>
    public void ScrollUp()
    {
        // Check if we are not on the first page
        if (this.floatingDocument.currentPageIndex - 1 >= 0)
        {
            // Scroll up (previous page)
            this.floatingDocument.currentPageIndex--;
            this.SetSprite();
        }
    }

    /// <summary>
    /// When called, we scroll down.
    /// </summary>
    public void ScrollDown()
    {
        // Check if we are not on the last page
        if (this.floatingDocument.currentPageIndex + 1 < this.floatingDocument.pages.Count)
        {
            // Scroll down (next page)
            this.floatingDocument.currentPageIndex++;
            this.SetSprite();
        }
    }

    /// <summary>
    /// Sets the sprite to the current pages sprite.
    /// </summary>
    public void SetSprite()
    {
        // Set the sprite of the floating document, found in the sprites list of the floating document
        this.floatingDocument.image.sprite = this.floatingDocument.sprites[this.floatingDocument.currentPageIndex];
    }

    /// <summary>
    /// Set the hovering value to the hovering value passed through.
    /// </summary>
    /// <param name="isHovering">The is hovering value.</param>
    public void IsHovering(bool isHovering)
    {
        // Set isHovering boolean
        this.isHovering = isHovering;
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
    /// Sets the current joystick value from the input action reference if it is available.
    /// </summary>
    private void SetJoystickValue()
    {
        // Read the joystick value
        this.joystickValue = this.inputActionReference.action.ReadValue<Vector2>();
    }

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
