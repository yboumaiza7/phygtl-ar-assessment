using UnityEngine;

namespace Phygtl.ARAssessment.Utilities
{
	/// <summary>
	/// Utility class for common operations.
	/// </summary>
	public static class Utility
	{
		/// <summary>
		/// Gets the combined bounds of all renderers in a GameObject and its children.
		/// </summary>
		/// <param name="gameObject">The GameObject to get bounds for.</param>
		/// <returns>The combined bounds of all renderers.</returns>
		public static Bounds GetObjectBounds(GameObject gameObject)
		{
			var renderers = gameObject.GetComponentsInChildren<Renderer>();

			if (renderers.Length == 0)
				// If no renderers, return a default bounds centered at the object's position
				return new Bounds(gameObject.transform.position, Vector3.zero);

			// Start with the first renderer's bounds
			Bounds bounds = renderers[0].bounds;

			// Encapsulate all other renderer bounds
			for (int i = 1; i < renderers.Length; i++)
				bounds.Encapsulate(renderers[i].bounds);

			return bounds;
		}
	}
}
