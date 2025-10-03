#region Namespaces

using UnityEngine;

#endregion

namespace Phygtl.ARAssessment.Core
{
	/// <summary>
	/// A base class for creating singleton MonoBehaviour components.
	/// </summary>
	/// <typeparam name="T">The type of the MonoBehaviour.</typeparam>
	public abstract class PersistentSingletonBehaviour<T> : SingletonBehaviour<T> where T : MonoBehaviour
	{
		/// <summary>
		/// The static instance of the singleton.
		/// </summary>
		public new static T Default
		{
			get
			{
				if (!instance)
				{
					instance = FindAnyObjectByType<T>();

                    if (!instance)
                    {
                        GameObject singleton = new(typeof(T).Name);

                        instance = singleton.AddComponent<T>();

                        DontDestroyOnLoad(singleton);
                    }
				}

				return instance;
			}
		}

		/// <summary>
		/// On Awake, ensure that only one instance exists. If not, destroy the current instance.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			if (destroyed)
				return;

			DontDestroyOnLoad(gameObject);
		}
	}
}
