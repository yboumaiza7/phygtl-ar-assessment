#region Namespaces

using System;
using System.Threading.Tasks;
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

		#endregion

		#region Helper Methods

		internal void Clear()
		{
			downloadTask = null;
		}

		#endregion
    }
}
