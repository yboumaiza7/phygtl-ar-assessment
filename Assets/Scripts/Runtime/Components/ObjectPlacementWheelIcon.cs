using UnityEngine;
using UnityEngine.UI;

namespace Phygtl.ARAssessment.Components
{
	/// <summary>
	/// A component that displays an icon on the object placement wheel.
	/// </summary>
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
		/// The object placement wheel.
		/// </summary>
		private ObjectPlacementWheel wheel;

		#endregion

		#region Methods

		private void Awake()
		{
			iconImage = GetComponent<Image>();
			rectTransform = GetComponent<RectTransform>();
			wheel = GetComponentInParent<ObjectPlacementWheel>();

			if (!wheel)
			{
				AppDebugger.LogError("Couldn't initialize the object placement wheel icon because the object placement wheel was not found!", this, nameof(ObjectPlacementWheelIcon));

				return;
			}
		}

		/// <summary>
		/// Initializes the icon on the wheel.
		/// </summary>
		/// <param name="icon">The icon to display.</param>
		/// <param name="index">The index of the icon.</param>
		/// <param name="totalCount">The total number of icons.</param>
		public void Initialize(Sprite icon, int index, int totalCount)
		{
			if (totalCount < 1 || !iconImage || !rectTransform || !wheel)
				return;

			iconImage.sprite = icon;

			// Get the wheel's RectTransform to calculate dimensions
			var wheelRect = wheel.GetComponent<RectTransform>();

			if (!wheelRect)
				return;

			// Calculate the wheel's effective radius (half of the smaller dimension)
			float wheelWidth = wheelRect.rect.width;
			float wheelHeight = wheelRect.rect.height;
			float wheelRadius = Mathf.Min(wheelWidth, wheelHeight) / 2f;

			// Calculate the angle for this icon's position (center of the segment)
			float anglePerSegment = 360f / totalCount;
			float iconAngle = (index + 0.5f) * anglePerSegment;
			float angleInRadians = iconAngle * Mathf.Deg2Rad;

			// Calculate the radial distance from center based on iconRadiusOffset
			float radialDistance = wheelRadius * wheel.iconRadiusOffset;

			// Calculate the position using polar coordinates (angle starts from right, goes counter-clockwise)
			// Unity UI uses standard math convention: 0° is up, 90° is right
			float x = radialDistance * Mathf.Sin(angleInRadians);
			float y = radialDistance * Mathf.Cos(angleInRadians);

			// Set the icon's anchored position (centered on the wheel)
			rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchoredPosition = new Vector2(x, y);

			// Set the icon size based on iconScale * wheel width
			float iconSize = wheelWidth * wheel.iconScale;
			rectTransform.sizeDelta = new Vector2(iconSize, iconSize);

			// Keep the icon upright (no rotation)
			rectTransform.localEulerAngles = Vector3.zero;
		}

		#endregion
	}
}
