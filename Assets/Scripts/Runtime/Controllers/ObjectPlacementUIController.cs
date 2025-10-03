#region Namespaces

using Phygtl.ARAssessment.Components;
using Phygtl.ARAssessment.Core;
using Phygtl.ARAssessment.Data;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

#endregion

namespace Phygtl.ARAssessment.Controllers
{
	/// <summary>
	/// A controller for the object placement UI.
	/// It is used to open and close the placement UI.
	/// </summary>
	public class ObjectPlacementUIController : MonoBehaviour
	{
		#region Properties

		/// <summary>
		/// The UI wheel instance.
		/// </summary>
		public ObjectPlacementWheel Wheel => objectPlacementWheel;

		#endregion

		#region Events

		public event Action<ObjectPlacementWheel> OnWheelOpened;
		public event Action<ObjectPlacementWheel> OnWheelClosed;

		#endregion

		#region Fields

		/// <summary>
		/// The prefab for the object placement wheel.
		/// </summary>
		[SerializeField]
		private ObjectPlacementWheel objectPlacementWheelPrefab;
		
		/// <summary>
		/// The canvas to instantiate the UI wheel into.
		/// </summary>
		[SerializeField]
		private Canvas canvas;

		/// <summary>
		/// Whether the placement UI should be hidden by default.
		/// </summary>
		[SerializeField]
		private bool hideByDefault = true;

		/// <summary>
		/// The current placement data.
		/// </summary>
		private ObjectPlacementPoint currentPlacementData;

		/// <summary>
		/// The current object placement wheel.
		/// </summary>
		private ObjectPlacementWheel objectPlacementWheel;

		private ObjectPlacementController controller;

		#endregion

		#region Methods

		private void Awake()
		{
			controller = ObjectPlacementController.Default;
		}

		private void Start()
		{
			if (!canvas || !objectPlacementWheelPrefab)
			{
				AppDebugger.LogError("Couldn't initialize the object placement UI because the canvas or the object placement wheel prefab were not set!", this, nameof(ObjectPlacementUIController));

				return;
			}

			objectPlacementWheel = objectPlacementWheelPrefab.InstantiateClone(canvas, hideByDefault);
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Close the placement UI.
		/// </summary>
		public async Task CloseAsync()
		{
			if (!objectPlacementWheel)
			{
				AppDebugger.LogError("Couldn't close the placement UI because the object placement wheel was not set!", this, nameof(ObjectPlacementUIController));

				return;
			}

			currentPlacementData = default;

			await objectPlacementWheel.HideAsync();

			OnWheelClosed?.Invoke(objectPlacementWheel);
		}

		/// <summary>
		/// Open the placement UI.
		/// </summary>
		/// <param name="raycastHit">The raycast hit.</param>
		/// <param name="camera">The camera.</param>
		public async Task OpenAsync(ARRaycastHit raycastHit, Camera camera) => await OpenAsync(new(raycastHit, camera));

		/// <summary>
		/// Open the placement UI.
		/// </summary>
		/// <param name="placement">The placement data.</param>
		public async Task OpenAsync(ObjectPlacementPoint placement)
		{
			if (!objectPlacementWheel)
			{
				AppDebugger.LogError("Couldn't open the placement UI because the object placement wheel was not set!", this, nameof(ObjectPlacementUIController));

				return;
			}

			if (currentPlacementData.isValid)
			{
				AppDebugger.LogWarning("Placement UI already open. Close the current placement UI first.", this, nameof(ObjectPlacementUIController));

				return;
			}

			currentPlacementData = placement;
			OnWheelOpened?.Invoke(objectPlacementWheel);

			await objectPlacementWheel.ShowAsync();

			AppDebugger.Log("Placement UI opened", this, nameof(ObjectPlacementUIController));
		}

		#endregion
	}
}
