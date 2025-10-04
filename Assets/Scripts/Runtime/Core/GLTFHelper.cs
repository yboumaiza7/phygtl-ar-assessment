#region Namespaces

using GLTFast;

using Object = UnityEngine.Object;

#endregion

namespace Phygtl.ARAssessment.Core
{
	/// <summary>
	/// A helper class for GLTF operations.
	/// </summary>
	public static class GLTFHelper
	{
		/// <summary>
		/// The defer agent.
		/// </summary>
		public static IDeferAgent DeferAgent { get; private set; }

		/// <summary>
		/// Disposes of the defer agent.
		/// </summary>
		public static void Initialize()
		{
			DeferAgent = Object.FindFirstObjectByType<TimeBudgetPerFrameDeferAgent>();
		}
	}
}
