#region Namespaces

using System;
using Phygtl.ARAssessment.Core;
using UnityEngine;

#endregion

namespace Phygtl.ARAssessment.Data
{
	/// <summary>
	/// A struct for storing the information of a placeable object.
	/// </summary>
	[Serializable]
	public class PlaceableObject
    {
		/// <summary>
		/// The name of the placeable object.
		/// </summary>
		public string name;

		/// <summary>
		/// The icon of the placeable object.
		/// </summary>
		public ResourceReference<Sprite> icon;

		/// <summary>
		/// The download URL of the placeable object.
		/// </summary>
		public string downloadUrl;

#if UNITY_EDITOR
		/// <summary>
		/// The local download URL of the placeable object. Used for testing purposes (editor only)!
		/// </summary>
		public string localDownloadUrl;
#endif
    }
}
