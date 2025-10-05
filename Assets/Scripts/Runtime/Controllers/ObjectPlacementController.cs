#region Namespaces

using System.Collections.Generic;
using Phygtl.ARAssessment.Core;
using Phygtl.ARAssessment.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

#endregion

namespace Phygtl.ARAssessment.Controllers
{
	/// <summary>
	/// A controller for placing objects in the scene.
	/// It uses the Enhanced Touch API to detect finger down events and raycast to the plane to find the placement point.
	/// It then opens the placement UI at the placement point.
	/// </summary>
	[RequireComponent(typeof(ARRaycastManager), typeof(ObjectPlacementUIController))]
	public class ObjectPlacementController : SingletonBehaviour<ObjectPlacementController>
	{
		#region Properties

		/// <summary>
		/// The UI controller.
		/// </summary>
		public ObjectPlacementUIController UIController => uiController;

		/// <summary>
		/// The main camera.
		/// </summary>
		public Camera MainCamera => mainCamera;

		/// <summary>
		/// The preferred plane alignment.
		/// </summary>
		public PlaneAlignment PreferredPlaneAlignment => preferedPlaneAlignment;

		/// <summary>
		/// Whether the interaction is disabled. This disables all user interactions with the placement controller and the placement UI.
		/// </summary>
		public bool DisableInteraction { get; set; }

		#endregion

		#region Fields

		/// <summary>
		/// The prefab for the placeable object.
		/// </summary>
		public GameObject placeableObjectPrefab;

		/// <summary>
		/// The allowed plane alignment.
		/// </summary>
		[SerializeField]
		private PlaneAlignment preferedPlaneAlignment = PlaneAlignment.HorizontalUp;
		
        [SerializeField]
        [Tooltip("The AR ray interactor that determines where to spawn the object.")]
		[FormerlySerializedAs("m_ARInteractor")]
        private XRRayInteractor interactor;

		[SerializeField]
        private XRInputButtonReader placeObjectInput = new("Spawn Object");

		/// <summary>
		/// The raycast manager.
		/// </summary>
		private ARRaycastManager raycastManager;

		/// <summary>
		/// The UI controller.
		/// </summary>
		private ObjectPlacementUIController uiController;

		/// <summary>
		/// The selection controller.
		/// </summary>
		private ObjectSelectionController selectionController;
		
		/// <summary>
		/// The event system.
		/// </summary>
		private EventSystem eventSystem;

		/// <summary>
		/// The raycast hits of the AR raycast manager.
		/// </summary>
		private readonly List<ARRaycastHit> raycastHits = new();

		/// <summary>
		/// The raycast results of the event system.
		/// </summary>
		private readonly List<RaycastResult> raycastResults = new();

		/// <summary>
		/// The main camera.
		/// </summary>
		private Camera mainCamera;

		private bool attemptSpawn;

		#endregion

		#region Methods

		protected override void Awake()
		{
			base.Awake();

			GLTFHelper.Initialize();

			raycastManager = GetComponent<ARRaycastManager>();
			uiController = GetComponent<ObjectPlacementUIController>();
			eventSystem = FindFirstObjectByType<EventSystem>();
			selectionController = ObjectSelectionController.Default;

			TryRetreiveCamera();

			// Clear the placeable objects cache
			foreach (var placeableObject in PlaceableObjectManager.Default.placeableObjects)
				placeableObject.Clear();
		}

		private void Update()
		{
			// Wait a frame after the Spawn Object input is triggered to actually cast against AR planes and spawn
            // in order to ensure the touchscreen gestures have finished processing to allow the ray pose driver
            // to update the pose based on the touch position of the gestures.
            if (attemptSpawn)
            {
                attemptSpawn = false;

                TryShowWheel();

                return;
            }

            if (placeObjectInput.ReadWasPerformedThisFrame())
                attemptSpawn = true;
		}

		private void OnEnable()
		{
			//EnhancedTouch.TouchSimulation.Enable();
			EnhancedTouch.EnhancedTouchSupport.Enable();
			placeObjectInput.EnableDirectActionIfModeUsed();
		}
		
		private void OnDisable()
		{
			//EnhancedTouch.TouchSimulation.Disable();
			EnhancedTouch.EnhancedTouchSupport.Disable();
			placeObjectInput.DisableDirectActionIfModeUsed();
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Tries to retrieve the main camera.
		/// </summary>
		/// <returns>True if the camera was retrieved, false otherwise.</returns>
		private bool TryRetreiveCamera()
		{
			if (!mainCamera)
			{
				var camera = Camera.main;

				mainCamera = camera ? camera : FindFirstObjectByType<Camera>();
			}

			return mainCamera;
		}

		/// <summary>
		/// Checks if the finger is over the UI.
		/// </summary>
		/// <param name="finger">The finger to check.</param>
		/// <returns>True if the finger is over the UI, false otherwise.</returns>
		private bool IsFingerOverUI()
		{
			if (!eventSystem)
				return false;

			PointerEventData pointerEventData = new(eventSystem);

			raycastResults.Clear();
			eventSystem.RaycastAll(pointerEventData, raycastResults);

			return raycastResults.Count > 0;
		}

		/// <summary>
		/// Event handler for the finger down event.
		/// </summary>
		/// <param name="finger">The finger that was pressed.</param>
		private void TryShowWheel()
		{
			if (DisableInteraction || IsFingerOverUI() || uiController.Wheel.IsOpen || !interactor.TryGetCurrentARRaycastHit(out var hit))
				return;

			if (!mainCamera && !TryRetreiveCamera())
			{
				AppDebugger.LogError("Cannot find camera. Finger action will be ignored.", this, nameof(ObjectPlacementController));

				return;
			}

			if (hit.trackable is not ARPlane plane || plane.alignment != preferedPlaneAlignment)
				return;

			// Check we're hitting a placeable object
			PointerEventData pointerEventData = new(eventSystem);
			Vector3 direction = (hit.pose.position - mainCamera.transform.position).normalized;
			Ray ray = new(mainCamera.transform.position, direction);

			if (Physics.Raycast(ray, out var raycastHit, 10000f, Physics.AllLayers, QueryTriggerInteraction.Ignore) && raycastHit.collider.GetComponentInParent<ARTransformer>())
				return;

			if (selectionController)
				selectionController.ClearSelection();

			_ = uiController.OpenAsync(hit, mainCamera);
		}

		#endregion
	}
}
