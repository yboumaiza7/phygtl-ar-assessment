#region Namespaces

using System.Collections.Generic;
using Phygtl.ARAssessment.Core;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using Utilities;

#endregion

namespace Phygtl.ARAssessment.Controllers
{
	/// <summary>
	/// A controller for selecting objects in the scene.
	/// It uses the XR ray interactor to detect when an object is selected and displays a selection image.
	/// </summary>
	public class ObjectSelectionController : SingletonBehaviour<ObjectSelectionController>
	{
		#region Fields

		/// <summary>
		/// The image that displays the selection.
		/// </summary>
		[SerializeField]
		private Image selectionImage;

		/// <summary>
		/// The scale of the selection image.
		/// </summary>
		[SerializeField]
		private float selectionImageScale = 1f;

		/// <summary>
		/// The offset of the selection image from the plane.
		/// </summary>
		[SerializeField]
		private float selectionImagePlaneOffset = 0.05f;

		/// <summary>
		/// The rotation of the selection image.
		/// </summary>
		[SerializeField]
		private Vector3 selectionImageOffsetRotation = new(-90f, 0f, 0f);

		/// <summary>
		/// The bounds of the selection image.
		/// </summary>
		private readonly Dictionary<GameObject, Bounds> selectionBounds = new();

		/// <summary>
		/// The XR ray interactor.
		/// </summary>
		private XRRayInteractor interactor;

		#endregion

		#region Methods

		protected override void Awake()
		{
			base.Awake();

			if (destroyed)
				return;

			interactor = FindFirstObjectByType<XRRayInteractor>();
		}

		private void Start()
		{
			selectionImage.gameObject.SetActive(false);
			selectionBounds.Clear();
		}

		private void Update()
		{
			
		}

		private void OnEnable()
		{
			interactor.selectEntered.AddListener(OnSelectEntered);
		}

		private void OnDisable()
		{
			interactor.selectEntered.RemoveListener(OnSelectEntered);
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Called when an object is selected.
		/// </summary>
		/// <param name="args">The event arguments.</param>
		private void OnSelectEntered(SelectEnterEventArgs args)
		{
			var transform = args.interactableObject.transform;
			var gameObject = transform.gameObject;
			var rotationOffset = Quaternion.Euler(selectionImageOffsetRotation);

			selectionImage.transform.rotation = transform.rotation * rotationOffset;

			if (!selectionBounds.TryGetValue(gameObject, out var bounds))
				selectionBounds[gameObject] = bounds = Utility.GetObjectBounds(gameObject);

			selectionImage.transform.position = bounds.center + (selectionImagePlaneOffset - bounds.extents.y) * transform.up;
			selectionImage.transform.localScale = rotationOffset * bounds.size * selectionImageScale;

			selectionImage.gameObject.SetActive(true);
		}

		/// <summary>
		/// Clears the selection.
		/// </summary>
		public void ClearSelection()
		{
			selectionImage.gameObject.SetActive(false);
		}

		#endregion
	}
}
