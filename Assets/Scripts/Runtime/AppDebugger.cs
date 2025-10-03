#region Namespaces

using System;
using UnityEngine;

using Object = UnityEngine.Object;

#endregion

namespace Phygtl.ARAssessment
{
	/// <summary>
	/// A class for logging detailed messages to the console.
	/// </summary>
	public static class AppDebugger
	{
		/// <summary>
		/// Log a message to the console.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="context">The context of the message.</param>
		/// <param name="sourceName">The name of the source of the message.</param>
		[HideInCallstack]
		public static void Log(string message, Object context = null, string sourceName = "App")
		{
			Debug.Log($"{sourceName}: {message}", context);
		}

		/// <summary>
		/// Log a warning message to the console.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="context">The context of the message.</param>
		/// <param name="sourceName">The name of the source of the message.</param>
		[HideInCallstack]
		public static void LogWarning(string message, Object context = null, string sourceName = "App")
		{
			Debug.LogWarning($"{sourceName}: {message}", context);
		}

		/// <summary>
		/// Log an error message to the console.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="context">The context of the message.</param>
		/// <param name="sourceName">The name of the source of the message.</param>
		[HideInCallstack]
		public static void LogError(string message, Object context = null, string sourceName = "App")
		{
			Debug.LogError($"{sourceName}: {message}", context);
		}

		/// <summary>
		/// Log an exception to the console.
		/// </summary>
		/// <param name="exception">The exception to log.</param>
		/// <param name="context">The context of the exception.</param>
		[HideInCallstack]
		public static void LogException(Exception exception, Object context = null)
		{
			Debug.LogException(exception, context);
		}
	}
}
