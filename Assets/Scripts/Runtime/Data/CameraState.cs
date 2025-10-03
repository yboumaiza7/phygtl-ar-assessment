#region Namespaces

using UnityEngine;

#endregion

namespace Phygtl.ARAssessment.Data
{
	/// <summary>
	/// A struct for storing the pose of the camera.
	/// </summary>
	public readonly struct CameraPose
	{
		#region Fields

		/// <summary>
		/// The position of the camera.
		/// </summary>
		public readonly Vector3 position;

		/// <summary>
		/// The rotation of the camera.
		/// </summary>
		public readonly Quaternion rotation;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor for the CameraPose struct.
		/// </summary>
		/// <param name="position">The position of the camera.</param>
		/// <param name="rotation">The rotation of the camera.</param>
		public CameraPose(Vector3 position, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
		}

		/// <summary>
		/// Constructor for the CameraPose struct from a camera.
		/// </summary>
		/// <param name="camera">The camera.</param>
		public CameraPose(Camera camera) : this(camera.transform.position, camera.transform.rotation) { }

		#endregion
	}
}
