using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for scaling documents.
/// </summary>
public class ScaleDocuments : MonoBehaviour
{
    /// <summary>
    /// The floating document script we want to change the scale in.
    /// </summary>
    public FloatingDocument floatingDocument;

    /// <summary>
    /// The scale of the documents.
    /// </summary>
    public Vector3 scale;

    /// <summary>
    /// Scales the document up.
    /// </summary>
    public void ScaleUp()
    {
        this.scale = this.scale * 1.1f;
        this.SetScale();
    }

    /// <summary>
    /// Scales the document down.
    /// </summary>
    public void ScaleDown()
    {
        this.scale = this.scale / 1.1f;
        this.SetScale();
    }

    /// <summary>
    /// Sets the scale.
    /// </summary>
    private void SetScale()
    {
        this.floatingDocument.scale = this.scale;
        this.gameObject.transform.localScale = this.scale;
    }

    /// <summary>
    /// Called before method is started.
    /// </summary>
    private void Awake()
    {
        this.scale = this.gameObject.transform.localScale;
    }
}
