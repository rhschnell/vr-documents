using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// the keyboard doing to basic logic for the keyboard to show in VR
/// </summary>
public class ShowKeyboard : MonoBehaviour
{
    private TMP_InputField inputField;

    /// <summary>
    /// Opens the keyboard
    /// </summary>
    public void OpenKeyboard()
    {
        NonNativeKeyboard.Instance.InputField = this.inputField;
        NonNativeKeyboard.Instance.PresentKeyboard(this.inputField.text);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.inputField = this.GetComponent<TMP_InputField>();
        this.inputField.onSelect.AddListener(x => this.OpenKeyboard());
    }
}
