using UnityEngine;
using System.Collections;

/// <summary>
/// Syntactic sugar to create stoppable coroutines.
/// </summary>
public static class StoppableCoroutineExtensions
{
		public static StoppableCoroutine MakeStoppable (this IEnumerator generator)
		{
				return new StoppableCoroutine (generator);
		}
}
