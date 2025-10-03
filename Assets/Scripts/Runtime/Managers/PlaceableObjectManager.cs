#region Namespaces

using System.Collections.Generic;
using Phygtl.ARAssessment.Components;
using Phygtl.ARAssessment.Core;
using Phygtl.ARAssessment.Data;
using UnityEngine;

#endregion

namespace Phygtl.ARAssessment.Managers
{
	/// <summary>
	/// A manager for the placeable objects.
	/// It is a singleton asset that contains the list of placeable objects.
	/// </summary>
	[CreateAssetMenu(fileName = "PlaceableObjectManager", menuName = "Phygtl/AR Assessment/Managers/Placeable Object Manager")]
	public class PlaceableObjectManager : ScriptableSingleton<PlaceableObjectManager>
	{
		/// <summary>
		/// The list of placeable objects.
		/// </summary>
		public List<PlaceableObject> placeableObjects;
	}
}
