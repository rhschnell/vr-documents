using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectEnvironment : MonoBehaviour
{
    public TMP_Dropdown Dropdown;

    private void Awake()
    {
        // List of environment names
        List<string> environmentNames = new List<string>();
        Dropdown.ClearOptions();
        Dropdown.AddOptions(environmentNames);
    }
    public void AddEnvironmentButton()
    {
        int selectedIndex = Dropdown.value;
        string selectedEnvironmentName = Dropdown.options[selectedIndex].text;
    }
}
