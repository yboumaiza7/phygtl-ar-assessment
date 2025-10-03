#region Namespaces

using System.Collections.Generic;
using Phygtl.ARAssessment.Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
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

		#endregion

		#region Fields

		/// <summary>
		/// The allowed plane alignment.
		/// </summary>
		[SerializeField]
		private PlaneAlignment preferedPlaneAlignment = PlaneAlignment.HorizontalUp;

		/// <summary>
		/// The raycast manager.
		/// </summary>
		private ARRaycastManager raycastManager;

		/// <summary>
		/// The UI controller.
		/// </summary>
		private ObjectPlacementUIController uiController;

		
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

		#endregion

		#region Methods

		/// <summary>
		/// Event handler for the awake event.
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			raycastManager = GetComponent<ARRaycastManager>();
			uiController = GetComponent<ObjectPlacementUIController>();
			eventSystem = FindFirstObjectByType<EventSystem>();

			TryRetreiveCamera();
		}

		/// <summary>
		/// Event handler for the enable event.
		/// </summary>
		private void OnEnable()
		{
			EnhancedTouch.TouchSimulation.Enable();
			EnhancedTouch.EnhancedTouchSupport.Enable();
			EnhancedTouch.Touch.onFingerDown += OnFingerDown;
		}
		
		/// <summary>
		/// Event handler for the disable event.
		/// </summary>
		private void OnDisable()
		{
			EnhancedTouch.TouchSimulation.Disable();
			EnhancedTouch.EnhancedTouchSupport.Disable();
			EnhancedTouch.Touch.onFingerDown -= OnFingerDown;
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
		private bool IsFingerOverUI(EnhancedTouch.Finger finger)
		{
			if (!eventSystem)
				return false;

			PointerEventData pointerEventData = new(eventSystem)
			{
				position = finger.currentTouch.screenPosition
			};

			raycastResults.Clear();
			eventSystem.RaycastAll(pointerEventData, raycastResults);

			return raycastResults.Count > 0;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Event handler for the finger down event.
		/// </summary>
		/// <param name="finger">The finger that was pressed.</param>
		private void OnFingerDown(EnhancedTouch.Finger finger)
		{
			if (finger.index != 0 || IsFingerOverUI(finger) || !raycastManager.Raycast(finger.currentTouch.screenPosition, raycastHits, TrackableType.PlaneWithinPolygon))
				return;

			if (!mainCamera && !TryRetreiveCamera())
			{
				AppDebugger.LogError("Cannot find camera. Finger action will be ignored.", this, nameof(ObjectPlacementController));

				return;
			}

			foreach (var hit in raycastHits)
			{
				if (hit.trackable is not ARPlane plane || plane.alignment != preferedPlaneAlignment)
					continue;

				_ = uiController.OpenAsync(hit, mainCamera);

				break; // Break the loop because we only want to open the placement UI once.
			}
		}

		#endregion
	}
}
