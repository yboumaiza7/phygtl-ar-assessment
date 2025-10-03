#region Namespaces

using UnityEngine;
using UnityEngine.XR.ARFoundation;

#endregion

namespace Phygtl.ARAssessment.Data
{
	/// <summary>
	/// A struct for storing the placement of an object.
	/// </summary>
	public readonly struct ObjectPlacementPoint
	{
		#region Fields

		/// <summary>
		/// The raycast hit that caused the placement.
		/// </summary>
		public readonly ARRaycastHit raycastHit;

		/// <summary>
		/// The pose of the camera.
		/// </summary>
		public readonly CameraPose cameraPose;

		/// <summary>
		/// Whether the placement is valid. Defaults to zero when not initialized.
		/// </summary>
		public readonly bool isValid;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructor for the ObjectPlacement struct from a raycast hit and a camera.
		/// </summary>
		/// <param name="raycastHit">The raycast hit that caused the placement.</param>
		/// <param name="camera">The camera.</param>
		public ObjectPlacementPoint(ARRaycastHit raycastHit, Camera camera)
		{
			this.raycastHit = raycastHit;
			cameraPose = new(camera);
			isValid = true;
		}


		/// <summary>
		/// Constructor for the ObjectPlacement struct from a raycast hit and a camera pose.
		/// </summary>
		/// <param name="raycastHit">The raycast hit that caused the placement.</param>
		/// <param name="cameraPose">The pose of the camera.</param>
		public ObjectPlacementPoint(ARRaycastHit raycastHit, CameraPose cameraPose)
		{
			this.raycastHit = raycastHit;
			this.cameraPose = cameraPose;
			isValid = true;
		}

		#endregion
	}
}
