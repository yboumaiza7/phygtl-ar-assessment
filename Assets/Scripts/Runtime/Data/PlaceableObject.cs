#region Namespaces

using System;
using System.Threading.Tasks;
using GLTFast;
using Phygtl.ARAssessment.Core;
using Phygtl.ARAssessment.IO;
using Phygtl.ARAssessment.Managers;
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
		#region Properties

		/// <summary>
		/// The download URL of the placeable object.
		/// </summary>
		public string DownloadUrl
		{
			get
			{
				#if UNITY_EDITOR
				if (PlaceableObjectManager.Default.useLocalDownload)
					return localDownloadUrl;
				#endif

				return downloadUrl;
			}
			set
			{
				#if UNITY_EDITOR
				if (PlaceableObjectManager.Default.useLocalDownload)
					localDownloadUrl = value;
				else
					downloadUrl = value;
				#else
				downloadUrl = value;
				#endif
			}
		}

		/// <summary>
		/// Whether the placeable object is downloaded.
		/// </summary>
		public bool IsDownloaded => downloadTask?.IsCompleted ?? false;

		/// <summary>
		/// Whether the placeable object is loaded.
		/// </summary>
		public bool IsLoaded => gltfAsset != null;

		/// <summary>
		/// Downloads the placeable object asynchronously.
		/// </summary>
		/// <returns>The task for downloading the placeable object.</returns>
		public Task<DownloadResponse> DownloadAsync(Action fetchCompletedCallback = null, Action<float> progressCallback = null)
		{
			downloadTask ??= DownloadCache.RetrieveOrDownloadAsync(DownloadUrl, fetchCompletedCallback, progressCallback);

			return downloadTask;
		}

		#endregion

		#region Fields

		/// <summary>
		/// The name of the placeable object.
		/// </summary>
		public string name;

		/// <summary>
		/// The icon of the placeable object.
		/// </summary>
		public ResourceReference<Sprite> icon;

		/// <summary>
		/// The default scale of the placeable object.
		/// </summary>
		public Vector3 defaultScale = Vector3.one;

		/// <summary>
		/// The mass of the placeable object.
		/// </summary>
		public float mass = 1f;

		/// <summary>
		/// The download URL of the placeable object.
		/// </summary>
		[SerializeField]
		private string downloadUrl;

#if UNITY_EDITOR
		/// <summary>
		/// The local download URL of the placeable object. Used for testing purposes (editor only)!
		/// </summary>
		[SerializeField]
		private string localDownloadUrl;
#endif
	
		/// <summary>
		/// The task for downloading the placeable object.
		/// </summary>
		private Task<DownloadResponse> downloadTask;

		/// <summary>
		/// The cached GLTF importer.
		/// </summary>
		private GLTFAsset gltfAsset;

		#endregion

		#region Helper Methods

		/// <summary>
		/// Gets the GLTF asset cached or loads it asynchronously.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>The GLTF asset.</returns>
		public async Task<GLTFAsset> GetOrLoadGLTFAssetAsync(string filePath)
		{
			return IsLoaded ? gltfAsset : (gltfAsset = await GLTFAsset.LoadAsync(filePath));
		}

		/// <summary>
		/// Clears the cached GLTF asset and its download task.
		/// </summary>
		internal void Clear()
		{
			downloadTask = null;
			gltfAsset?.Dispose();
			gltfAsset = null;
		}

		#endregion
    }
}
