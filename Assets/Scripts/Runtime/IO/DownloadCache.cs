#region Namespaces

using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Phygtl.ARAssessment.Managers;
using UnityEngine;

#endregion

namespace Phygtl.ARAssessment.IO
{
	/// <summary>
	/// A static class for managing the download cache.
	/// </summary>
	public static class DownloadCache
	{
		/// <summary>
		/// The cache directory.
		/// </summary>
		private static readonly string cacheDirectory = Path.Combine(Application.persistentDataPath, PlaceableObjectManager.CacheDirectory);

		/// <summary>
		/// Downloads or retrieves a file from the cache.
		/// </summary>
		/// <param name="url">The URL of the file to download or load.</param>
		/// <param name="fetchCompletedCallback">The callback to report the completion of requesting file details.</param>
		/// <param name="progressCallback">The callback to report the progress of the download.</param>
		/// <returns>The download response.</returns>
		public static async Task<DownloadResponse> RetrieveOrDownloadAsync(string url, Action fetchCompletedCallback = null, Action<float> progressCallback = null)
		{
			DownloadResponse downloadResponse = new()
			{
				url = url
			};
			HttpResponseMessage response;

			using HttpClient client = new()
			{
				Timeout = TimeSpan.FromMinutes(10) // Increase timeout for large files
			};

			try
			{
				response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

				response.EnsureSuccessStatusCode();
			}
			catch (Exception e)
			{
				downloadResponse.error = e.ToString();
				
				return downloadResponse;
			}

			var totalBytes = response.Content.Headers.ContentLength ?? -1L;

			GetFileNameAndPath(url, out _, out string filePath);

			fetchCompletedCallback?.Invoke();

			if (File.Exists(filePath))
			{
				var currentFileSize = new FileInfo(filePath).Length;

				if (totalBytes < 0L || (totalBytes > 0L && currentFileSize == totalBytes))
				{
					downloadResponse.success = true;
					downloadResponse.localPath = filePath;

					return downloadResponse;
				}
			}

		// File doesn't exist in cache, download it
		try
		{
			// Ensure cache directory exists
			if (!Directory.Exists(cacheDirectory))
				Directory.CreateDirectory(cacheDirectory);

			var downloadedBytes = 0L;

			using var stream = await response.Content.ReadAsStreamAsync();
			
			// Write directly to file for maximum speed (no memory buffering)
			using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 
				bufferSize: 1048576, // 1 MB file buffer for maximum I/O performance
				useAsync: true);

			// Dynamic buffer size based on file size for optimal performance
			int bufferSize = CalculateOptimalBufferSize(totalBytes);
			var buffer = new byte[bufferSize];
			int bytesRead;
			bool unknownSizeCallbackInvoked = false;

			while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
			{
				await fileStream.WriteAsync(buffer, 0, bytesRead);

				downloadedBytes += bytesRead;

				if (totalBytes > 0L)
				{
					var progress = (float)downloadedBytes / totalBytes;

					progressCallback?.Invoke(progress);
				}
				else if (!unknownSizeCallbackInvoked)
				{
					progressCallback?.Invoke(0.5f); // Fallback once if total size is unknown

					unknownSizeCallbackInvoked = true;
				}
			}

			progressCallback?.Invoke(1.0f);

			downloadResponse.success = true;
			downloadResponse.localPath = filePath;
		}
		catch (Exception e)
		{
			downloadResponse.error = e.ToString();
			
			// Clean up partial download on error
			if (File.Exists(filePath))
				try { File.Delete(filePath); } catch { }
		}

			return downloadResponse;
		}

		/// <summary>
		/// Checks if a file is cached.
		/// </summary>
		/// <param name="url">The URL of the file to check.</param>
		/// <returns>Whether the file is cached.</returns>
		public static bool IsCached(string url, out string filePath)
		{
			GetFileNameAndPath(url, out _, out filePath);

			return File.Exists(filePath);
		}

		/// <summary>
		/// Gets the file name and path from the URL.
		/// </summary>
		/// <param name="url">The URL of the file.</param>
		/// <param name="fileName">The name of the file.</param>
		/// <param name="filePath">The path of the file.</param>
		private static void GetFileNameAndPath(string url, out string fileName, out string filePath)
		{
			fileName = url.GetHashCode().ToString();
			filePath = Path.Combine(cacheDirectory, fileName);
		}

	/// <summary>
	/// Calculates the optimal buffer size based on the total file size.
	/// Optimized for maximum download speed with larger buffers.
	/// </summary>
	/// <param name="totalBytes">The total size of the file in bytes, or -1 if unknown.</param>
	/// <returns>The optimal buffer size in bytes.</returns>
	private static int CalculateOptimalBufferSize(long totalBytes)
	{
		const int minBufferSize = 65536;     // 64 KB - minimum buffer
		const int maxBufferSize = 1048576;   // 1 MB - maximum buffer for optimal speed

		// If file size is unknown, use large buffer for best throughput
		if (totalBytes <= 0)
			return 524288; // 512 KB default

		// Aggressive buffer sizing for maximum download speed
		// Small files (< 1 MB): 64 KB - 128 KB
		// Medium files (1-10 MB): 128 KB - 256 KB
		// Large files (10-50 MB): 256 KB - 512 KB
		// Very large files (> 50 MB): 512 KB - 1 MB

		if (totalBytes < 1_048_576) // < 1 MB
			return Mathf.Clamp((int)(totalBytes / 8), minBufferSize, 131072);
		else if (totalBytes < 10_485_760) // < 10 MB
			return Mathf.Clamp((int)(totalBytes / 32), 131072, 262144);
		else if (totalBytes < 52_428_800) // < 50 MB
			return Mathf.Clamp((int)(totalBytes / 128), 262144, 524288);
		else // >= 50 MB
			return maxBufferSize; // Use maximum 1 MB buffer for large files
	}
	}
}
