using System;
using Phygtl.ARAssessment.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Phygtl.ARAssessment.Components
{
	/// <summary>
	/// A component that displays an icon on the object placement wheel.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	public class ObjectPlacementWheelIcon : MonoBehaviour
	{
		#region Fields

		/// <summary>
		/// The image component that displays the icon.
		/// </summary>
		private Image iconImage;

		/// <summary>
		/// The rect transform component that positions the icon.
		/// </summary>
		private RectTransform rectTransform;

		/// <summary>
		/// The placeable object manager.
		/// </summary>
		private PlaceableObjectManager manager;

		/// <summary>
		/// The object placement wheel.
		/// </summary>
		private ObjectPlacementWheel wheel;

		/// <summary>
		/// The object placement wheel slot.
		/// </summary>
		private ObjectPlacementWheelSlot slot;

		#endregion

		#region Methods

		private void Awake()
		{
			iconImage = GetComponent<Image>();
			rectTransform = GetComponent<RectTransform>();
			wheel = GetComponentInParent<ObjectPlacementWheel>();
			manager = PlaceableObjectManager.Default;

			if (!wheel)
				AppDebugger.LogError("Couldn't initialize the object placement wheel icon because the object placement wheel was not found!", this, nameof(ObjectPlacementWheelIcon));

			if (!manager)
				AppDebugger.LogError("Couldn't initialize the object placement wheel icon because the PlaceableObjectManager was not found!", this, nameof(ObjectPlacementWheelIcon));
		}

		/// <summary>
		/// Initializes the icon on the wheel.
		/// </summary>
		/// <param name="index">The index of the icon.</param>
		/// <param name="slot">The slot of the icon.</param>
		public void Initialize(int index, ObjectPlacementWheelSlot slot)
		{
			if (!iconImage || !rectTransform || !wheel)
				return;

			int objectsCount = manager.placeableObjects.Count;

			if (index < 0 || index >= objectsCount)
				throw new ArgumentOutOfRangeException("The index of the icon must be greater than 0 and less than the total number of objects!", nameof(index));

			if (!slot)
				throw new ArgumentNullException(nameof(slot));

			var obj = manager.placeableObjects[index];

			this.slot = slot;
			iconImage.sprite = obj.icon;

			// Get the wheel's RectTransform to calculate dimensions
			var wheelRect = wheel.GetComponent<RectTransform>();

			if (!wheelRect)
				return;

			// Disable raycast for all child images
			foreach (var childImage in GetComponentsInChildren<Image>(true))
				childImage.raycastTarget = false;

			// Calculate the wheel's effective radius (half of the smaller dimension)
			float wheelWidth = wheelRect.rect.width;
			float wheelHeight = wheelRect.rect.height;
			float wheelRadius = Mathf.Min(wheelWidth, wheelHeight) / 2f;

			// Calculate the angle for this icon's position (center of the segment)
			float anglePerSegment = 360f / objectsCount;
			float iconAngle = (index + 0.5f) * anglePerSegment;
			float angleInRadians = iconAngle * Mathf.Deg2Rad;

			// Calculate the radial distance from center based on iconRadiusOffset
			float radialDistance = wheelRadius * wheel.iconRadiusOffset;

			// Calculate the position using polar coordinates (angle starts from right, goes counter-clockwise)
			// Unity UI uses standard math convention: 0° is up, 90° is right
			float x = radialDistance * Mathf.Sin(angleInRadians);
			float y = radialDistance * Mathf.Cos(angleInRadians);

			// Set the icon's anchored position (centered on the wheel)
			rectTransform.anchorMin = new(0.5f, 0.5f);
			rectTransform.anchorMax = new(0.5f, 0.5f);
			rectTransform.pivot = new(0.5f, 0.5f);
			rectTransform.anchoredPosition = new(x, y);

			// Set the icon size based on iconScale * wheel width
			float iconSize = wheelWidth * wheel.iconScale;
			rectTransform.sizeDelta = new(iconSize, iconSize);

			// Keep the icon upright (no rotation)
			rectTransform.localEulerAngles = Vector3.zero;

			// Set the icon's parent to the slot
			rectTransform.SetParent(slot.transform, true);
		}

		#endregion
	}
}
