#region Namespaces

using System;
#if UNITY_EDITOR
using System.Linq;
#endif
using UnityEngine;

using Object = UnityEngine.Object;

#endregion

namespace Phygtl.ARAssessment.Core
{
	/// <summary>
	/// A struct for storing a reference to a resource.
	/// </summary>
	/// <typeparam name="T">The type of the resource.</typeparam>
	[Serializable]
    public struct ResourceReference<T> where T : Object
    {
		#region Properties

		/// <summary>
		/// Whether the resource reference is empty.
		/// </summary>
		public readonly bool IsEmpty => string.IsNullOrEmpty(path);

		/// <summary>
		/// The value of the resource reference.
		/// </summary>
		public T Value
		{
			get
			{
				if (!cachedAsset && !IsEmpty)
					cachedAsset = Resources.Load<T>(path);

				return cachedAsset;
			}
			set
			{
				cachedAsset = value;

				#if UNITY_EDITOR
				if (!ValidateAsset(value, out var path))
					return;

				this.path = path;
				#endif
			}
		}

		#endregion

		#region Fields

		/// <summary>
		/// Path to the prefab relative to the Resources folder.
		/// </summary>
		[SerializeField]
		private string path;

		/// <summary>
		/// The cached asset.
		/// </summary>
		private T cachedAsset;

#if UNITY_EDITOR
		/// <summary>
		/// The separators for the path.
		/// </summary>
		private static readonly char[] pathSeparators = new char[] { '\\', '/' };
#endif

		#endregion
		
		#region Operators

		/// <summary>
		/// Implicitly convert a resource reference to its value.
		/// </summary>
		/// <param name="reference">The resource reference to convert.</param>
		/// <returns>The value of the resource reference.</returns>
		public static implicit operator T(ResourceReference<T> reference) => reference.Value;
#if UNITY_EDITOR
		/// <summary>
		/// Try to create a resource reference from a value. (Editor only)
		/// </summary>
		/// <param name="value">The value to create a resource reference from.</param>
		/// <returns>The created resource reference.</returns>
		public static implicit operator ResourceReference<T>(T value) => TryCreate(value, out var reference) ? reference : default;
#endif

		#endregion

		#region Methods

#if UNITY_EDITOR
		/// <summary>
		/// Validate the asset and get the path. (Editor only)
		/// </summary>
		/// <param name="asset">The asset to validate.</param>
		/// <param name="path">The path to the asset.</param>
		/// <returns>True if the asset is valid, false otherwise.</returns>
		public static bool ValidateAsset(T asset, out string path)
		{
			string fullPath = UnityEditor.AssetDatabase.GetAssetPath(asset);
			string[] pathParts = fullPath.Split(pathSeparators, StringSplitOptions.RemoveEmptyEntries);
			int index = Array.IndexOf(pathParts, "Resources");

			if (index < 0)
			{
				path = string.Empty;

				return false;
			}
			else
			{
				string newPath = string.Join(System.IO.Path.DirectorySeparatorChar, pathParts.Skip(index + 1));
				string directoryPath = System.IO.Path.GetDirectoryName(newPath);
				string name = System.IO.Path.GetFileNameWithoutExtension(newPath);

				path = System.IO.Path.Combine(directoryPath, name);

				return true;
			}
		}

		/// <summary>
		/// Try to create a resource reference from a value. (Editor only)
		/// </summary>
		/// <param name="value">The value to create a resource reference from.</param>
		/// <param name="reference">The created resource reference.</param>
		/// <returns>True if the resource reference was created successfully, false otherwise.</returns>
		public static bool TryCreate(T value, out ResourceReference<T> reference)
		{
			if (!ValidateAsset(value, out var path))
			{
				reference = default;

				return false;
			}

			reference = new ResourceReference<T>
			{
				path = path
			};

			return true;
		}
#endif

		#endregion
    }
}
