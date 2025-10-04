using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Phygtl.ARAssessment.Controllers
{
	public class ScreenshotController : MonoBehaviour
	{
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
		/// The duration of the flash animation.
		/// </summary>
		[SerializeField]
		private float flashDuration = 0.5f;

		/// <summary>
		/// The duration of the animation.
		/// </summary>
		[SerializeField]
		private float animationDuration = 0.5f;

		public void Hide()
		{
			StartCoroutine(HideCoroutine());
		}

		private IEnumerator HideCoroutine()
		{
			flashImage.color = Color.clear;

			flashImage.gameObject.SetActive(false);

			yield return image.DOFade(0f, animationDuration).WaitForCompletion();

			image.gameObject.SetActive(false);
		}

		public void TakeScreenshot()
		{
			StartCoroutine(TakeScreenshotCoroutine());
		}

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

			flashImage.gameObject.SetActive(false);
		}
	}
}
