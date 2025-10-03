#region Namespaces

using System.Collections.Generic;
using System.IO;
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
		#region Constants

		/// <summary>
		/// The default cache directory.
		/// </summary>
		public const string DEFAULT_CACHE_DIRECTORY = "DownloadCache";

		/// <summary>
		/// The cache directory.
		/// </summary>
		public static string CacheDirectory { get; private set; }

		#endregion

		#region Fields

		/// <summary>
		/// The list of placeable objects.
		/// </summary>
		public List<PlaceableObject> placeableObjects;

		/// <summary>
		/// The directory to cache the placeable objects.
		/// </summary>
		public string cacheDirectory = DEFAULT_CACHE_DIRECTORY;

#if UNITY_EDITOR
		/// <summary>
		/// Whether to use local download. (Editor only)
		/// </summary>
		public bool useLocalDownload;
#endif

		#endregion

		#region Methods

		private void OnEnable()
		{
			CacheDirectory = Path.Combine(Application.persistentDataPath, Default ? instance.cacheDirectory : DEFAULT_CACHE_DIRECTORY);
		}

		#endregion
	}
}
