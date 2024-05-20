using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface that is just a shell of the base version, allows us to mock request.
/// </summary>
public interface ICoroutineRunner
{
    /// <summary>
    /// the abract function
    /// </summary>
    /// <param name="routine">the thing to run</param>
    /// <returns>base return</returns>
    Coroutine StartCoroutine(IEnumerator routine);
}

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
