using System.Collections;
using UnityEngine;

/// <summary>
/// An interface that is just a shell of the base version, allows us to mock request.
/// </summary>
public interface ICoroutineRunner
{
    /// <summary>
    /// The abract function.
    /// </summary>
    /// <param name="routine">The routine to run.</param>
    /// <returns>Base return.</returns>
    Coroutine StartCoroutine(IEnumerator routine);
}
