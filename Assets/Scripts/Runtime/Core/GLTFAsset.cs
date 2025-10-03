#region Namespaces

using System.IO;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

#endregion

namespace Phygtl.ARAssessment.Core
{
	/// <summary>
	/// A class for loading and instantiating GLTF assets.
	/// </summary>
    public class GLTFAsset
    {
		/// <summary>
		/// The GLTF importer.
		/// </summary>
		private GltfImport importer;

		/// <summary>
		/// Loads a GLTF asset asynchronously.
		/// </summary>
		/// <param name="filePath">The path to the GLTF file.</param>
		/// <returns>The GLTF asset if loaded successfully, otherwise null.</returns>
		public static async Task<GLTFAsset> LoadAsync(string filePath)
		{
			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
				return null;

			GltfImport import = new();
			var bytes = File.ReadAllBytes(filePath);

			return await import.Load(bytes, new(filePath)) ? new() { importer = import } : null;
		}
		
		/// <summary>
		/// Instantiates the GLTF asset asynchronously.
		/// </summary>
		/// <param name="parent">The parent transform.</param>
		/// <returns>Whether the instantiation was successful.</returns>
		public async Task<bool> InstantiateAsync(Transform parent = null)
		{
			return await importer.InstantiateMainSceneAsync(parent);
		}

		/// <summary>
		/// Instantiates the GLTF asset asynchronously.
		/// </summary>
		/// <param name="instantiator">The instantiator.</param>
		/// <returns>Whether the instantiation was successful.</returns>
		public async Task<bool> InstantiateAsync(IInstantiator instantiator)
		{
			return await importer.InstantiateMainSceneAsync(instantiator);
		}
    }
}
