using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ARstatus class is used to keep track of the AR status.
/// </summary>
public class ARstatus : MonoBehaviour
{
    /// <summary>
    /// The boolean containing the current AR status.
    /// </summary>
    public bool isAR = false;

    [SerializeField]
    private static GameObject instance;

    /// <summary>
    /// The only instance of the ARstatus.
    /// </summary>
    void Awake()
    {
        // Check if there is already an instance of the GroundController
        // If there is not, set the instance to this object
        // If there is, destroy this object
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this.gameObject;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }
}
