#region Namespaces

using UnityEngine;

#endregion

namespace Phygtl.ARAssessment.Core
{
    /// <summary>
    /// A base class for creating singleton MonoBehaviour components.
    /// </summary>
    /// <typeparam name="T">The type of the MonoBehaviour.</typeparam>
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        /// <summary>
        /// The static instance of the singleton.
        /// </summary>
        public static T Default
        {
            get
            {
                if (!instance)
                    instance = FindFirstObjectByType<T>();

                return instance;
            }
        }

        /// <summary>
        /// The cached instance.
        /// </summary>
        private static T instance;

		/// <summary>
		/// Whether the instance is going to be destroyed.
		/// </summary>
		protected bool destroyed = false;

        /// <summary>
        /// On Awake, ensure that only one instance exists. If not, destroy the current instance.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance && instance != this)
            {
				Destroy(this);

                destroyed = true;

				return;
			}

			instance = this as T;
        }
    }
}
