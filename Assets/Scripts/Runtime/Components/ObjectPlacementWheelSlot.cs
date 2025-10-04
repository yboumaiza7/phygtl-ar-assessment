using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Phygtl.ARAssessment.Controllers;
using Phygtl.ARAssessment.Data;
using Phygtl.ARAssessment.IO;
using Phygtl.ARAssessment.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Rendering;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
using Utilities;

namespace Phygtl.ARAssessment.Components
{
	/// <summary>
	/// A component that displays a slot on the object placement wheel.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Image))]
	public class ObjectPlacementWheelSlot : MonoBehaviour, IPointerClickHandler
	{
		#region Fields

		/// <summary>
		/// The image component that handles the click event.
		/// </summary>
		[Header("Slot parameters")]
		[SerializeField]
		private Image clickHandlerImage;

		/// <summary>
		/// The scale of the click handler image height.
		/// </summary>
		[SerializeField]
		[Range(0f, 1f)]
		private float clickHandlerImageHeightScale = .5f;
		
		/// <summary>
		/// The default color of the slot.
		/// </summary>
		[SerializeField]
		private Color defaultSlotColor = Color.white;

		/// <summary>
		/// The color of the slot when it is loading.
		/// </summary>
		[SerializeField]
		private Color loadingSlotColor = Color.clear;

		/// <summary>
		/// The default color of the icon.
		/// </summary>
		[Header("Icon parameters")]
		[SerializeField]
		private Color defaultIconColor = Color.white;

		/// <summary>
		/// The color of the icon when it is loading.
		/// </summary>
		[SerializeField]
		private Color loadingIconColor = Color.white;

		/// <summary>
		/// The image component that displays the load progress.
		/// </summary>
		[Header("Progress bar parameters")]
		[SerializeField]
		private Image loadProgressImage;

		/// <summary>
		/// The color of the load progress.
		/// </summary>
		[SerializeField]
		private Color defaultProgressColor = Color.cyan;

		/// <summary>
		/// The color of the load progress when an error occurs.
		/// </summary>
		[SerializeField]
		private Color errorProgressColor = Color.red;

		/// <summary>
		/// The color of the load progress when the file is cached.
		/// </summary>
		[SerializeField]
		private Color cachedProgressColor = Color.green;

		/// <summary>
		/// The period of the loading progress bar.
		/// </summary>
		[SerializeField]
		private float loadingProgressBarPeriod = 1f;

		/// <summary>
		/// The image component that displays the slot background.
		/// </summary>
		private Image image;

		/// <summary>
		/// The object placement wheel.
		/// </summary>
		private ObjectPlacementWheel wheel;

		/// <summary>
		/// The object placement wheel icon.
		/// </summary>
		private ObjectPlacementWheelIcon icon;

		/// <summary>
		/// The image component that displays the icon.
		/// </summary>
		private Image iconImage;

		/// <summary>
		/// The PlaceableObjectManager.
		/// </summary>
		private PlaceableObjectManager manager;

		/// <summary>
		/// The ObjectPlacementController.
		/// </summary>
		private ObjectPlacementController controller;

		/// <summary>
		/// The ObjectPlacementController.
		/// </summary>
		private ObjectPlacementUIController uiController;

		/// <summary>
		/// The index of the object in the PlaceableObjectManager.
		/// </summary>
		private int index;

		/// <summary>
		/// Whether the slot is initialized.
		/// </summary>
		private bool initialized;

		/// <summary>
		/// Whether the slot is busy.
		/// </summary>
		private bool busy;

		#endregion

		#region Methods

		private void Awake()
		{
			image = GetComponent<Image>();
			wheel = GetComponentInParent<ObjectPlacementWheel>();
			manager = PlaceableObjectManager.Default;
			controller = ObjectPlacementController.Default;
			uiController = controller ? controller.UIController : null;

			if (!wheel)
				AppDebugger.LogError("Couldn't initialize the object placement wheel slot because the object placement wheel was not found!", this, nameof(ObjectPlacementWheelSlot));

			if (!clickHandlerImage)
				AppDebugger.LogError("Couldn't initialize the object placement wheel slot because the click handler image was not found!", this, nameof(ObjectPlacementWheelSlot));

			if (!manager)
				AppDebugger.LogError("Couldn't initialize the object placement wheel slot because the PlaceableObjectManager was not found!", this, nameof(ObjectPlacementWheelSlot));

			if (!controller)
				AppDebugger.LogError("Couldn't initialize the object placement wheel slot because the ObjectPlacementController was not found!", this, nameof(ObjectPlacementWheelSlot));

			if (!uiController)
				AppDebugger.LogError("Couldn't initialize the object placement wheel slot because the ObjectPlacementUIController was not found!", this, nameof(ObjectPlacementWheelSlot));
		}

		/// <summary>
		/// Initializes the slot on the wheel.
		/// </summary>
		/// <param name="index">The index of the slot.</param>
		/// <param name="icon">The icon of the slot.</param>
		public void Initialize(int index, ObjectPlacementWheelIcon icon)
		{
			if (!image || !wheel || !manager || !clickHandlerImage || !loadProgressImage)
				return;

			int objectsCount = manager.placeableObjects.Count;

			if (index < 0 || index >= objectsCount)
				throw new ArgumentOutOfRangeException("The index of the slot must be greater than 0 and less than the total number of objects!", nameof(index));

			if (!icon)
				throw new ArgumentNullException(nameof(icon));

			this.icon = icon;
			iconImage = icon.GetComponent<Image>();

			if (image.gameObject == loadProgressImage.gameObject)
				AppDebugger.LogWarning("The image and the load progress image are the same object! This will cause issues with the load progress image!", this, nameof(ObjectPlacementWheelSlot));

			if (clickHandlerImage.transform.parent != transform)
				AppDebugger.LogWarning("The click handler image is not a child of the slot! This will cause issues with the click handler image!", this, nameof(ObjectPlacementWheelSlot));

			foreach (var childImage in GetComponentsInChildren<Image>(true))
				if (childImage != clickHandlerImage)
					childImage.raycastTarget = false;

			clickHandlerImage.raycastTarget = true;
			image.fillAmount = ((360f / objectsCount) - wheel.slotSpacing) / 360f;
			transform.localEulerAngles = new(0, 0, -index * (360f / objectsCount) - (wheel.slotSpacing / 2f));

			// Resize clickHandlerImage to fit the bounding box of the image fill
			var clickHandlerRect = clickHandlerImage.GetComponent<RectTransform>();
			var slotRect = GetComponent<RectTransform>();

			if (clickHandlerRect && slotRect)
			{
				// Get the wheel dimensions
				float wheelWidth = slotRect.rect.width;
				float wheelHeight = slotRect.rect.height;
				float wheelRadius = Mathf.Min(wheelWidth, wheelHeight) / 2f;

				// Calculate the angle of the segment in radians
				float segmentAngle = image.fillAmount * 2f * Mathf.PI;

				// Calculate bounding box dimensions for the circular segment
				// Width: chord length at the outer edge
				float boundingWidth = 2f * wheelRadius * Mathf.Sin(segmentAngle / 2f);

				// Height: radial distance from center to outer arc
				// For a filled radial segment from center, height is the full radius
				float boundingHeight = wheelRadius;

				// Set the clickHandlerImage size to match the bounding box
				clickHandlerRect.sizeDelta = new(boundingWidth, boundingHeight);

				// Position it at the center of the segment
				// The segment is centered, so we offset by half the height toward the outer edge
				clickHandlerRect.anchorMin = new(0.5f, 0.5f);
				clickHandlerRect.anchorMax = new(0.5f, 0.5f);

				// Calculate the scaled height (distance from center)
				float scaledHeight = boundingHeight * clickHandlerImageHeightScale;

				// Calculate the radius at this height (distance from wheel center)
				float radiusAtHeight = wheelRadius - scaledHeight;

				// Calculate the chord width at this specific radius
				// The chord width decreases as we move inward (higher scale = closer to center = narrower width)
				float scaledWidth = 2f * radiusAtHeight * Mathf.Cos(segmentAngle / 2f);

				// Set pivot to bottom center to rotate around the center of the wheel
				clickHandlerRect.pivot = new(0.5f, 0f);
				clickHandlerRect.sizeDelta = new(scaledWidth, scaledHeight);

				// Calculate the rotation angle in radians for positioning
				float rotationAngle = -segmentAngle * 0.5f;
				
				// Position the click area so its top edge (bottom + scaledHeight) is at the outer edge
				// Move in the direction of rotation (perpendicular to center)
				float positionRadius = wheelRadius - scaledHeight;
				float posX = -positionRadius * Mathf.Sin(rotationAngle);
				float posY = positionRadius * Mathf.Cos(rotationAngle);

				clickHandlerRect.anchoredPosition = new(posX, posY);
				clickHandlerRect.localEulerAngles = new(0f, 0f, rotationAngle * Mathf.Rad2Deg);
			}

			var obj = manager.placeableObjects[index];

			loadProgressImage.gameObject.SetActive(false);

			this.index = index;
			initialized = true;
		}

		public async Task ShowAsync()
		{
			Hide();

			image.DOFade(busy ? loadingSlotColor.a : defaultSlotColor.a, wheel.animationDuration);
			iconImage.DOFade(busy ? loadingIconColor.a : defaultIconColor.a, wheel.animationDuration);
			await loadProgressImage.DOFade(defaultProgressColor.a, wheel.animationDuration).AsyncWaitForCompletion();
		}

		public async Task HideAsync()
		{
			image.DOFade(0f, wheel.animationDuration);
			iconImage.DOFade(0f, wheel.animationDuration);
			await loadProgressImage.DOFade(0f, wheel.animationDuration).AsyncWaitForCompletion();
		}

		public void Hide()
		{
			Color clearImageColor = image.color;
			Color clearIconColor = iconImage.color;
			Color clearProgressColor = loadProgressImage.color;

			clearImageColor.a = 0f;
			clearIconColor.a = 0f;
			clearProgressColor.a = 0f;

			image.color = clearImageColor;
			iconImage.color = clearIconColor;
			loadProgressImage.color = clearProgressColor;
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Downloads the object asynchronously.
		/// </summary>
		private async Task DownloadObjectAsync()
		{
			if (!initialized)
				return;

			var obj = manager.placeableObjects[index];

			clickHandlerImage.raycastTarget = false;
			image.color = loadingSlotColor;
			iconImage.color = loadingIconColor;

			loadProgressImage.gameObject.SetActive(true);

			var loadingCoroutine = StartCoroutine(LoadingProgressBarCoroutine());
			var downloadResponse = await obj.DownloadAsync(() =>
			{
				if (loadingCoroutine != null)
				{
					StopCoroutine(loadingCoroutine);
					loadProgressImage.DOKill();

					loadingCoroutine = null;
				}

				loadProgressImage.fillAmount = 0f;
			},
			(progress) =>
			{
				if (loadingCoroutine != null)
				{
					StopCoroutine(loadingCoroutine);
					loadProgressImage.DOKill();

					loadingCoroutine = null;
				}

				loadProgressImage.fillAmount = progress * image.fillAmount;
			});

			image.color = defaultSlotColor;
			iconImage.color = defaultIconColor;

			if (downloadResponse.success)
			{
				loadProgressImage.fillAmount = image.fillAmount;
				loadProgressImage.color = cachedProgressColor;
			}
			else
				loadProgressImage.color = errorProgressColor;

			clickHandlerImage.raycastTarget = true;
		}

		/// <summary>
		/// The coroutine that loads the progress bar.
		/// </summary>
		private IEnumerator LoadingProgressBarCoroutine()
		{
			loadProgressImage.color = defaultProgressColor;
			loadProgressImage.fillAmount = 0f;

			float duration = loadingProgressBarPeriod * 0.5f;

			while (true)
			{
				yield return loadProgressImage.DOFillAmount(image.fillAmount, duration).WaitForCompletion();
				yield return loadProgressImage.DOFillAmount(0f, duration).WaitForCompletion();
			}
		}

		/// <summary>
		/// Loads and places the object asynchronously.
		/// </summary>
		/// <param name="placeableObject">The placeable object.</param>
		/// <param name="filePath">The file path.</param>
		private async Task LoadAndPlaceObject(PlaceableObject placeableObject, string filePath)
		{
			foreach (var slot in wheel.Slots)
			{
				slot.image.color = slot.loadingSlotColor;
				slot.iconImage.color = slot.loadingIconColor;
			}

			controller.DisableInteraction = true;

			var loadingCoroutine = StartCoroutine(LoadingProgressBarCoroutine());
			var asset = await placeableObject.GetOrLoadGLTFAssetAsync(filePath);
			GameObject gameObject = Instantiate(controller.placeableObjectPrefab, Vector3.zero, Quaternion.identity);

			gameObject.name = $"{placeableObject.name} (Clone)";

			if (asset != null)
				await asset.InstantiateAsync(gameObject.transform);
			else
			{
				AppDebugger.LogError("Couldn't load the object because the GLTF asset!", this, nameof(ObjectPlacementWheelSlot));

				goto CANCEL_LOADING;
			}

			gameObject.transform.localScale = placeableObject.defaultScale;

			var placement = uiController.CurrentPlacement;

			// Calculate rotation first
			var direction = placement.cameraPose.position - placement.raycastHit.pose.position;

			direction -= Vector3.Scale(direction, placement.raycastHit.pose.up);

			direction.Normalize();

			var rotation = Quaternion.LookRotation(direction, placement.raycastHit.pose.up);

			// Set temporary position and rotation to get accurate bounds in world space
			gameObject.transform.SetPositionAndRotation(placement.raycastHit.pose.position, rotation);

			// Get bounds in world space
			var bounds = Utility.GetObjectBounds(gameObject);

			// Calculate offset needed to place bottom of object at raycast position
			// bounds.min.y is the bottom of the object in world space
			float bottomOffset = gameObject.transform.position.y - bounds.min.y;

			// Final position: raycast position + offset to place bottom at surface
			var position = placement.raycastHit.pose.position + bottomOffset * placement.raycastHit.pose.up;

			gameObject.transform.position = position;

			// Destroy all cameras and add mesh colliders to the meshes
			foreach (var camera in gameObject.GetComponentsInChildren<Camera>(true))
				Destroy(camera.gameObject);

			foreach (var filter in gameObject.GetComponentsInChildren<MeshFilter>(true))
			{
				if (filter.TryGetComponent<Collider>(out _))
					continue;

				var meshCollider = filter.gameObject.AddComponent<MeshCollider>();

				meshCollider.sharedMesh = filter.mesh;
				meshCollider.convex = true;
			}

			// Add ARTransformer & Rigidbody to enable object manipulation
			if (!gameObject.TryGetComponent<ARTransformer>(out var transformer))
				transformer = gameObject.AddComponent<ARTransformer>();

			transformer.minScale = gameObject.transform.localScale.x * manager.minMaxScale.x;
			transformer.maxScale = gameObject.transform.localScale.x * manager.minMaxScale.y;

			if (!gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
				rigidbody = gameObject.AddComponent<Rigidbody>();

			rigidbody.isKinematic = !manager.useGravity;
			rigidbody.useGravity = manager.useGravity;
			rigidbody.mass = placeableObject.mass;

			gameObject.SetActive(true);

		CANCEL_LOADING:
			StopCoroutine(loadingCoroutine);
			loadProgressImage.DOKill();

			loadProgressImage.fillAmount = image.fillAmount;
			controller.DisableInteraction = false;

			foreach (var slot in wheel.Slots)
			{
				if (!slot.clickHandlerImage.raycastTarget)
					continue;

				slot.image.color = slot.defaultSlotColor;
				slot.iconImage.color = slot.defaultIconColor;
			}

			if (asset != null)
				await uiController.CloseAsync();
			else
				loadProgressImage.color = errorProgressColor;
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// Handles the pointer click event.
		/// </summary>
		/// <param name="eventData">The pointer event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!initialized || controller.DisableInteraction || !uiController.Wheel.IsOpen)
				return;

			var obj = manager.placeableObjects[index];

			_ = obj.IsDownloaded && DownloadCache.IsCached(obj.DownloadUrl, out string filePath) ? LoadAndPlaceObject(obj, filePath) : DownloadObjectAsync();
		}

		#endregion
	}
}
