using System.Collections;
using DG.Tweening;
using Phygtl.ARAssessment.Components;
using Phygtl.ARAssessment.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Phygtl.ARAssessment.Controllers
{
	/// <summary>
	/// A controller for taking screenshots.
	/// It uses the Enhanced Touch API to detect finger down events and raycast to the plane to find the placement point.
	/// It then opens the placement UI at the placement point.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class ScreenshotController : MonoBehaviour
	{
		#region Properties

		/// <summary>
		/// The capture icon.
		/// </summary>
		public Image CaptureIcon => captureIcon;

		#endregion

		#region Fields

		/// <summary>
		/// The container image.
		/// </summary>
		[SerializeField]
		private Image image;

		/// <summary>
		/// The flash image.
		/// </summary>
		[SerializeField]
		private Image flashImage;

		/// <summary>
		/// The hide text.
		/// </summary>
		[SerializeField]
		private TextMeshProUGUI hideText;
		
		/// <summary>
		/// The duration of the flash animation.
		/// </summary>
		[SerializeField]
		private float flashDuration = 0.5f;

		/// <summary>
		/// The duration of the animation.
		/// </summary>
		[SerializeField]
		private float animationDuration = 0.5f;

		private ObjectPlacementController placementController;

		private ObjectPlacementUIController uiController;

		private Image captureIcon;

		#endregion

		#region Methods

		private void Awake()
		{
			captureIcon = GetComponent<Image>();
			placementController = ObjectPlacementController.Default;
			uiController = placementController.UIController;

			captureIcon.DOFade(1f, animationDuration).SetDelay(animationDuration);
		}

		private void OnEnable()
		{
			uiController.OnWheelOpened += OnWheelOpened;
			uiController.OnWheelClosed += OnWheelClosed;
		}

		private void OnDisable()
		{
			uiController.OnWheelOpened -= OnWheelOpened;
			uiController.OnWheelClosed -= OnWheelClosed;
		}

		#endregion

		#region Event Handlers

		private void OnWheelOpened(ObjectPlacementWheel _)
		{
			captureIcon.DOFade(0f, animationDuration);
		}

		private void OnWheelClosed(ObjectPlacementWheel _)
		{
			captureIcon.DOFade(1f, animationDuration);
		}

		#endregion

		#region Utility Methods

		/// <summary>
		/// Hides the screenshot.
		/// </summary>
		public void Hide()
		{
			StartCoroutine(HideCoroutine());
		}

		/// <summary>
		/// Hides the screenshot.
		/// </summary>
		private IEnumerator HideCoroutine()
		{
			flashImage.color = Color.clear;

			flashImage.gameObject.SetActive(false);
			hideText.DOKill();
			hideText.DOFade(0f, animationDuration);

			yield return image.DOFade(0f, animationDuration).WaitForCompletion();

			image.gameObject.SetActive(false);
		}

		/// <summary>
		/// Takes a screenshot.
		/// </summary>
		public void TakeScreenshot()
		{
			StartCoroutine(TakeScreenshotCoroutine());
		}

		/// <summary>
		/// Takes a screenshot.
		/// </summary>
		private IEnumerator TakeScreenshotCoroutine()
		{
			yield return new WaitForEndOfFrame();
			
			int width = Screen.width;
			int height = Screen.height;
			Texture2D texture = new(width, height, TextureFormat.ARGB32, false);

			texture.ReadPixels(new(0, 0, width, height), 0, 0);
			texture.Apply();

			flashImage.gameObject.SetActive(true);

			image.sprite = Sprite.Create(texture, new(0, 0, width, height), new(0.5f, 0.5f));
			image.color = Color.white;
			flashImage.color = Color.white;
			
			image.gameObject.SetActive(true);

			yield return flashImage.DOFade(0f, flashDuration).WaitForCompletion();

			hideText.DOFade(1f, animationDuration);
			flashImage.gameObject.SetActive(false);
		}

		#endregion
	}
}
