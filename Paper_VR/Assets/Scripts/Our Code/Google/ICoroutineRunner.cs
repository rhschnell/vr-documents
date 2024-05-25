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
