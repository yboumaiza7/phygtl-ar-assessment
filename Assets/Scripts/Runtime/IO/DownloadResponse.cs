namespace Phygtl.ARAssessment.IO
{
	/// <summary>
	/// A struct for storing the response of a download operation.
	/// </summary>
    public struct DownloadResponse
    {
		/// <summary>
		/// Whether the download operation was successful.
		/// </summary>
		public bool success;
		/// <summary>
		/// The error message if the download operation failed.
		/// </summary>
		public string error;
		/// <summary>
		/// The URL of the downloaded file.
		/// </summary>
		public string url;
		/// <summary>
		/// The local path of the downloaded file.
		/// </summary>
		public string localPath;
    }
}
