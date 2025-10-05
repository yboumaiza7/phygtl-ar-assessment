using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Phygtl.ARAssessment.Controllers
{
	/// <summary>
	/// A controller for the tutorial UI.
	/// It is used to play close animations and hide the tutorial UI.
	/// </summary>
	public class TutorialUIController : MonoBehaviour
	{
		#region Fields

		/// <summary>
		/// The background image.
		/// </summary>
		[SerializeField]
		private Image backgroundImage;

		/// <summary>
		/// The panel mask.
		/// </summary>
		[SerializeField]
		private Image panelMask;

		/// <summary>
		/// The close button.
		/// </summary>
		[SerializeField]
		private Button closeButton;

		/// <summary>
		/// The duration of the animation.
		/// </summary>
		[SerializeField]
		private float animationDuration = 0.5f;

		#endregion

		#region Methods

		private void OnEnable()
		{
			closeButton.onClick.AddListener(OnCloseButtonClicked);
		}

		private void OnDisable()
		{
			closeButton.onClick.RemoveListener(OnCloseButtonClicked);
		}

		#endregion

		#region Event Handlers

		/// <summary>
		/// The close button was clicked.
		/// </summary>
		private void OnCloseButtonClicked()
		{
			StartCoroutine(CloseCoroutine());
		}

		/// <summary>
		/// The close coroutine.
		/// </summary>
		private IEnumerator CloseCoroutine()
		{
			yield return panelMask.DOFillAmount(0f, animationDuration).WaitForCompletion();
			yield return backgroundImage.DOFade(0f, animationDuration).WaitForCompletion();

			gameObject.SetActive(false);
		}

		#endregion
	}
}
