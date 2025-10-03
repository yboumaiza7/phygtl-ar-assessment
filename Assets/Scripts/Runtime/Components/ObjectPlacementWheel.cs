#region Namespaces

using System;
using System.Threading.Tasks;
using Phygtl.ARAssessment.Managers;
using UnityEngine;

#endregion

namespace Phygtl.ARAssessment.Components
{
	/// <summary>
	/// A component that displays the object placement wheel.
	/// </summary>
	public class ObjectPlacementWheel : MonoBehaviour
	{
		#region Properties

		public bool IsOpen => isOpen;

		#endregion

		#region Fields

		/// <summary>
		/// The prefab for the slot.
		/// </summary>
		public ObjectPlacementWheelSlot slotPrefab;
		/// <summary>
		/// The prefab for the icon.
		/// </summary>
		public ObjectPlacementWheelIcon slotIconPrefab;
		/// <summary>
		/// The outer margin of the wheel.
		/// </summary>
		public Vector4 outerMargin = new(64f, 64f, 64f, 64f);
		/// <summary>
		/// The spacing between the slots.
		/// </summary>
		[Range(0f, 360f)]
		public float slotSpacing = 1f;
		/// <summary>
		/// The radius offset of the icons.
		/// </summary>
		[Range(0f, 1f)]
		public float iconRadiusOffset = .75f;
		/// <summary>
		/// The scale of the icons.
		/// </summary>
		[Range(0f, 1f)]
		public float iconScale = 0.125f;

		/// <summary>
		/// The rect transform component that positions the wheel.
		/// </summary>
		private RectTransform rectTransform;

		/// <summary>
		/// Whether the wheel is open.
		/// </summary>
		private bool isOpen;

		#endregion

		#region Utility Methods

		/// <summary>
		/// Shows the wheel.
		/// </summary>
		/// <param name="immediate">If true, the wheel will be shown immediately.</param>
		public void Show(bool immediate = false)
		{
			if (immediate)
			{
				gameObject.SetActive(true);

				isOpen = true;
			}
			else
				_ = ShowAsync();
		}

		/// <summary>
		/// Hides the wheel.
		/// </summary>
		/// <param name="immediate">If true, the wheel will be hidden immediately.</param>
		public void Hide(bool immediate = false)
		{
			if (immediate)
			{
				gameObject.SetActive(false);

				isOpen = false;
			}
			else
				_ = HideAsync();
		}

		/// <summary>
		/// Shows the wheel asynchronously.
		/// </summary>
		public async Task ShowAsync()
		{
			await Task.Yield();

			gameObject.SetActive(true);

			await Task.CompletedTask;

			isOpen = true;
		}

		/// <summary>
		/// Hides the wheel asynchronously.
		/// </summary>
		public async Task HideAsync()
		{
			await Task.Yield();

			gameObject.SetActive(false);

			isOpen = false;

			await Task.CompletedTask;
		}

		/// <summary>
		/// Initializes the object placement wheel.
		/// </summary>
		private void Initialize()
		{
			var manager = PlaceableObjectManager.Default;

			if (!manager)
			{
				AppDebugger.LogError("Couldn't initialize the object placement wheel because the PlaceableObjectManager is not found!", this, nameof(ObjectPlacementWheel));

				return;
			}

			// Stretch the wheel both horizontally and vertically, centering it
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.pivot = new(0.5f, 0.5f);
			rectTransform.offsetMin = new(outerMargin.w, outerMargin.z);
			rectTransform.offsetMax = new(-outerMargin.y, -outerMargin.x);

			int objectsCount = manager.placeableObjects.Count;

			for (var i = 0; i < objectsCount; i++)
			{
				var slot = slotPrefab ? Instantiate(slotPrefab, transform) : null;
				var icon = slotIconPrefab ? Instantiate(slotIconPrefab, transform) : null;

				if (!slot || !icon)
				{
					AppDebugger.LogError("Couldn't initialize the object placement wheel because the slot or the icon was not found!", this, nameof(ObjectPlacementWheel));

					continue;
				}

				var obj = manager.placeableObjects[i];

				slot.Initialize(i, icon);
				icon.Initialize(i, slot);
			}
		}

		#endregion

		#region Methods

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
		}

		/// <summary>
		/// Instantiates a clone of the wheel.
		/// </summary>
		/// <param name="canvas">The canvas to instantiate the wheel on.</param>
		/// <param name="hide">If true, the wheel will be hidden.</param>
		/// <returns>The instantiated wheel.</returns>
		public ObjectPlacementWheel InstantiateClone(Canvas canvas, bool hide = true)
		{
			if (!canvas)
				throw new ArgumentNullException(nameof(canvas));

			var wheel = Instantiate(this, canvas.transform);

			wheel.Initialize();

			if (hide)
				wheel.Hide(true);

			return wheel;
		}

		#endregion
	}
}
