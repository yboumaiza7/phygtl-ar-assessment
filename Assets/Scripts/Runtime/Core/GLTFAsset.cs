#region Namespaces

using System;
using System.IO;
using System.Threading.Tasks;
using GLTFast;
using UnityEngine;

using Object = UnityEngine.Object;

#endregion

namespace Phygtl.ARAssessment.Core
{
	/// <summary>
	/// A class for loading and instantiating GLTF assets with performance optimizations.
	/// </summary>
    public class GLTFAsset
    {
		/// <summary>
		/// The GLTF importer.
		/// </summary>
		private GltfImport importer;

		/// <summary>
		/// Loads a GLTF asset asynchronously.
		/// glTFast handles threading internally and yields automatically to prevent frame drops.
		/// </summary>
		/// <param name="filePath">The path to the GLTF file.</param>
		/// <returns>The GLTF asset if loaded successfully, otherwise null.</returns>
		public static async Task<GLTFAsset> LoadAsync(string filePath)
		{
			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
				return null;

			try
			{
				// glTFast handles async operations natively without blocking
				// It automatically yields to prevent frame drops
				GltfImport import = new(deferAgent: GLTFHelper.DeferAgent);

				// Convert to URI format for efficient streaming
				Uri filePathUri = new(filePath);
				
				// Load from URI - glTFast streams and parses incrementally
				// Internal yields prevent frame drops
				bool success = await import.Load(filePathUri);

				if (success)
				{
					return new GLTFAsset { importer = import };
				}
				else
				{
					AppDebugger.LogError($"Failed to load GLTF asset from: {filePath}", null, nameof(GLTFAsset));
					return null;
				}
			}
			catch (Exception ex)
			{
				AppDebugger.LogError($"Error loading GLTF asset from {filePath}: {ex}", null, nameof(GLTFAsset));
				return null;
			}
		}

		/// <summary>
		/// Instantiates the GLTF asset asynchronously.
		/// </summary>
		/// <param name="parent">The parent transform.</param>
		/// <returns>Whether the instantiation was successful.</returns>
		public async Task<bool> InstantiateAsync(Transform parent = null)
		{
			if (importer == null)
				return false;

			try
			{
				return await importer.InstantiateMainSceneAsync(parent);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error instantiating GLTF asset: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Instantiates the GLTF asset asynchronously.
		/// </summary>
		/// <param name="instantiator">The instantiator.</param>
		/// <returns>Whether the instantiation was successful.</returns>
		public async Task<bool> InstantiateAsync(IInstantiator instantiator)
		{
			if (importer == null)
				return false;

			try
			{
				return await importer.InstantiateMainSceneAsync(instantiator);
			}
			catch (Exception ex)
			{
				Debug.LogError($"Error instantiating GLTF asset: {ex.Message}");
				return false;
			}
		}

		/// <summary>
		/// Disposes of the importer and frees resources.
		/// </summary>
		public void Dispose()
		{
			importer?.Dispose();
			importer = null;
		}
    }
}
