using System.Collections;
using UnityEngine;

/// <summary>
/// The shell class that allows us to mock it.
/// </summary>
public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
{
    /// <summary>
    /// The base function.
    /// </summary>
    /// <param name="routine">The routine to run.</param>
    /// <returns>The base return.</returns>
    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        // Start the coroutine using the base class
        return base.StartCoroutine(routine);
    }
}