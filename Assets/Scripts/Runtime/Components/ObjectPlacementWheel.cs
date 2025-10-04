#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Phygtl.ARAssessment.Controllers;
using Phygtl.ARAssessment.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#endregion

namespace Phygtl.ARAssessment.Components
{
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	/// <summary>
	/// A component that displays the object placement wheel.
	/// </summary>
	public class ObjectPlacementWheel : MonoBehaviour, IPointerClickHandler
	{
		#region Properties

		/// <summary>
		/// The list of slots.
		/// </summary>
		public IReadOnlyList<ObjectPlacementWheelSlot> Slots => slots;

		/// <summary>
		/// Whether the wheel is open.
		/// </summary>
		/// <returns>Whether the wheel is open.</returns>
		public bool IsOpen => isOpen;

		#endregion

		#region Fields

		/// <summary>
		/// The prefab for the slot.
		/// </summary>
		[Header("Prefab parameters")]
		[SerializeField]
		private  ObjectPlacementWheelSlot slotPrefab;
		/// <summary>
		/// The prefab for the icon.
		/// </summary>
		[SerializeField]
		private ObjectPlacementWheelIcon slotIconPrefab;
		/// <summary>
		/// The outer margin of the wheel.
		/// </summary>
		[SerializeField]
		private Vector4 outerMargin = new(64f, 64f, 64f, 64f);
		/// <summary>
		/// The spacing between the slots.
		/// </summary>
		[SerializeField]
		internal float slotSpacing = 1f;
		/// <summary>
		/// The radius offset of the icons.
		/// </summary>
		[SerializeField]
		internal float iconRadiusOffset = 0.75f;
		/// <summary>
		/// The scale of the icons.
		/// </summary>
		[SerializeField]
		internal float iconScale = 0.125f;
		/// <summary>
		/// The duration of the fill animation.
		/// </summary>
		[Header("Animation parameters")]
		[SerializeField]
		internal float animationDuration = 0.5f;

		/// <summary>
		/// The object placement controller.
		/// </summary>
		private ObjectPlacementController controller;

		/// <summary>
		/// The object placement UI controller.
		/// </summary>
		private ObjectPlacementUIController uiController;

		/// <summary>
		/// The rect transform component that positions the wheel.
		/// </summary>
		private RectTransform rectTransform;

		/// <summary>
		/// The image component that displays the wheel.
		/// </summary>
		private Image image;

		/// <summary>
		/// The list of slots.
		/// </summary>
		private readonly List<ObjectPlacementWheelSlot> slots = new();

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
			controller.DisableInteraction = true;

			// Wait a frame to ensure the wheel is not already open
			await Task.Yield();

			foreach (var slot in slots)
				slot.Hide();

			gameObject.SetActive(true);

			await image.DOFillAmount(1f, animationDuration).AsyncWaitForCompletion();

			foreach (var slot in slots)
				await slot.ShowAsync();

			controller.DisableInteraction = false;
			isOpen = true;
		}

		/// <summary>
		/// Hides the wheel asynchronously.
		/// </summary>
		public async Task HideAsync()
		{
			controller.DisableInteraction = true;

			// Wait a frame to not trigger a false positive
			await Task.Yield();

			await Task.WhenAll(slots.Select(slot => slot.HideAsync()));

			await image.DOFillAmount(0f, animationDuration).AsyncWaitForCompletion();

			gameObject.SetActive(false);

			controller.DisableInteraction = false;
			isOpen = false;
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
			image.fillAmount = 0f;

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

				slots.Add(slot);
			}
		}

		#endregion

		#region Methods

		private void Awake()
		{
			rectTransform = GetComponent<RectTransform>();
			controller = ObjectPlacementController.Default;
			uiController = controller ? controller.UIController : null;
			image = GetComponent<Image>();

			if (!controller)
				AppDebugger.LogError("Couldn't initialize the object placement wheel because the ObjectPlacementController was not found!", this, nameof(ObjectPlacementWheel));

			if (!uiController)
				AppDebugger.LogError("Couldn't initialize the object placement wheel because the ObjectPlacementUIController was not found!", this, nameof(ObjectPlacementWheel));
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

		#region Event Handlers

		public void OnPointerClick(PointerEventData eventData)
		{
			// Check if the click was on this image and not a child
			if (controller.DisableInteraction || eventData.pointerEnter != image.gameObject)
				return;
			
			_ = uiController.CloseAsync();
		}

		#endregion
	}
}
