using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Scaling Documents test.
/// </summary>
public class ScaleDocumentsTest : MonoBehaviour
{
    /// <summary>
    /// Tests the ScaleUp method.
    /// </summary>
    [Test]
    public void TestScaleUp()
    {
        GameObject gameObject = new GameObject();
        FloatingDocument fl = gameObject.AddComponent<FloatingDocument>();
        gameObject.transform.localScale = new Vector3(0.1f, 0.2f, 0.3f);
        ScaleDocuments scaleDocuments = gameObject.AddComponent<ScaleDocuments>();
        scaleDocuments.floatingDocument = fl;
        scaleDocuments.ScaleUp();
        Assert.AreEqual(new Vector3(0.1f, 0.2f, 0.3f) * 1.1f, scaleDocuments.scale);
    }

    /// <summary>
    /// Tests the ScaleDown method.
    /// </summary>
    [Test]
    public void TestScaleDown()
    {
        GameObject gameObject = new GameObject();
        FloatingDocument fl = gameObject.AddComponent<FloatingDocument>();
        gameObject.transform.localScale = new Vector3(0.1f, 0.2f, 0.3f);
        ScaleDocuments scaleDocuments = gameObject.AddComponent<ScaleDocuments>();
        scaleDocuments.floatingDocument = fl;
        scaleDocuments.ScaleDown();
        Assert.AreEqual(new Vector3(0.1f, 0.2f, 0.3f) / 1.1f, scaleDocuments.scale);
    }
}
