using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The shell class that allows us to mock it.
/// </summary>
public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
{
    /// <summary>
    /// The base function
    /// </summary>
    /// <param name="routine">the routine to run</param>
    /// <returns>the base return</returns>
    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return base.StartCoroutine(routine);
    }
}