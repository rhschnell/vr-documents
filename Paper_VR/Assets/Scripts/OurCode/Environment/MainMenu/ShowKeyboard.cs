using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// The keyboard doing to basic logic for the keyboard to show in VR.
/// </summary>
public class ShowKeyboard : MonoBehaviour
{
    private TMP_InputField inputField;

    /// <summary>
    /// Opens the keyboard.
    /// </summary>
    public void OpenKeyboard()
    {
        // Set the input field for the NonNativeKeyboard instance and present the keyboard
        NonNativeKeyboard.Instance.InputField = this.inputField;
        NonNativeKeyboard.Instance.PresentKeyboard(this.inputField.text);
    }

    /// <summary>
    /// Initializes the input field and subscribes the OpenKeyboard method to its onSelect event.
    /// </summary>
    void Start()
    {
        this.inputField = this.GetComponent<TMP_InputField>();

        // Subscribe the OpenKeyboard method to the input fieldss onSelect event
        this.inputField.onSelect.AddListener(x => this.OpenKeyboard());
    }
}
