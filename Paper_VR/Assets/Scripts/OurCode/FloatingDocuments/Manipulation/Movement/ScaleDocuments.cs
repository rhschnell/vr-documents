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
        // Scale the document up by x1.1
        this.scale = this.gameObject.transform.localScale * 1.1f;
        this.SetScale();
    }

    /// <summary>
    /// Scales the document down.
    /// </summary>
    public void ScaleDown()
    {
        // Scale the document down by 1.1f
        this.scale = this.gameObject.transform.localScale / 1.1f;
        this.SetScale();
    }

    /// <summary>
    /// Sets the scale.
    /// </summary>
    private void SetScale()
    {
        // Set the scale of the document to this.scale
        this.floatingDocument.scale = this.scale;
        this.gameObject.transform.localScale = this.scale;
    }

    /// <summary>
    /// Called before method is started.
    /// </summary>
    private void Awake()
    {
        // Set the scale of the document
        this.scale = this.gameObject.transform.localScale;
    }
}
