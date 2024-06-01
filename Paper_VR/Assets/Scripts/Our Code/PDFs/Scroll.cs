using System.Collections;
using System.Collections.Generic;
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
        Debug.Log("up");
        if (this.floatingDocument.currentPageIndex - 1 >= 0)
        {
            this.floatingDocument.currentPageIndex--;
            this.SetSprite();
        }

        Debug.Log("Current page: " + this.floatingDocument.currentPageIndex + " - 1 > = 0");
    }

    /// <summary>
    /// When called, we scroll down.
    /// </summary>
    public void ScrollDown()
    {
        Debug.Log("down'{ pageIndex: " + this.floatingDocument.currentPageIndex);
        if (this.floatingDocument.currentPageIndex + 1 < this.floatingDocument.pages.Count)
        {
            this.floatingDocument.currentPageIndex++;
            this.SetSprite();
        }

        Debug.Log("Current page: " + this.floatingDocument.currentPageIndex + " + 1 < = pages count: " + this.floatingDocument.pages[0].ToString());
    }

    /// <summary>
    /// Sets the sprite to the current pages sprite.
    /// </summary>
    public void SetSprite()
    {
        Debug.Log("setSprite");
        this.floatingDocument.image.sprite = this.floatingDocument.sprites[this.floatingDocument.currentPageIndex];
    }

    /// <summary>
    /// Set the hovering value to the hovering value passed through.
    /// </summary>
    /// <param name="isHovering">The is hovering value.</param>
    public void IsHovering(bool isHovering)
    {
        Debug.Log("set hovering to " + isHovering.ToString());
        this.isHovering = isHovering;
    }

    private void Update()
    {
        this.SetJoystickValue();
        this.ScrollCheck();
    }

    private void SetJoystickValue()
    {
        this.joystickValue = this.inputActionReference.action.ReadValue<Vector2>();
    }

    private void ScrollCheck()
    {
        if (this.available && this.isHovering) {
            if (this.joystickValue.y < -0.5f) {
                this.ScrollDown();
                this.StartCoroutine(this.LockAndUnlock());
            } else if (this.joystickValue.y > 0.5f) {
                this.ScrollUp();
                this.StartCoroutine(this.LockAndUnlock());
            }
        }
    }

    IEnumerator LockAndUnlock()
    {
        this.available = false;

        yield return new WaitForSeconds(this.scrollWaitingTime);

        this.available = true;
    }
}
