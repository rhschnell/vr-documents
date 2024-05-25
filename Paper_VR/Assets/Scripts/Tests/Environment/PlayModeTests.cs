using System.Collections;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Example PlayModeTests class that contains simple tests.
/// </summary>
public class PlayModeTests
{
    /// <summary>
    /// This is a simple test that asserts true.
    /// </summary>
    [Test]
    public void PlayModeTestsSimplePasses()
    {
        // Use the Assert class to test conditions.
        Assert.True(true);
    }

    /// <summary>
    /// A UnityTest behaves like a coroutine in PlayMode.
    /// </summary>
    /// <returns>
    /// nothing.
    /// </returns>
    [UnityTest]
    public IEnumerator PlayModeTestsWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // yield to skip a frame
        Assert.True(true);
        yield return null;
        Assert.True(true);
    }
}
