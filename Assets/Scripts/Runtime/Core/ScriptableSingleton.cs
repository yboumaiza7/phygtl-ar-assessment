#region Namespaces

using UnityEngine;

#endregion

namespace Phygtl.ARAssessment.Core
{
	/// <summary>
	/// A base class for creating singleton scriptable objects.
	/// </summary>
	/// <typeparam name="T">The type of the scriptable object.</typeparam>
	public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
	{
		/// <summary>
		/// The default instance of the scriptable object.
		/// </summary>
		public static T Default
		{
			get
			{
				if (!instance)
				{
					// Try to load from Resources using the type name
					instance = Resources.Load<T>(typeof(T).Name);

	#if UNITY_EDITOR
					// If not found, try to create and save an asset in the Resources folder (Editor only)
					if (!instance)
					{
						string assetPath = $"Assets/Resources/{typeof(T).Name}.asset";

						instance = CreateInstance<T>();

						UnityEditor.AssetDatabase.CreateAsset(instance, assetPath);
						UnityEditor.AssetDatabase.SaveAssets();
						UnityEditor.AssetDatabase.Refresh();
					}
	#endif

					if (!instance)
						AppDebugger.LogError($"ScriptableSingleton: Could not find or create instance of {typeof(T).Name} in Resources.", null, nameof(ScriptableSingleton<T>));
				}

				return instance;
			}
		}

		/// <summary>
		/// The cached instance of the scriptable object.
		/// </summary>
		private static T instance;
	}
}
