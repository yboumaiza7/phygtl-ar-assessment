using UnityEngine;
using UnityEngine.UI;

namespace Phygtl.ARAssessment.Components
{
	/// <summary>
	/// A component that displays a slot on the object placement wheel.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class ObjectPlacementWheelSlot : MonoBehaviour
	{
		#region Fields

		/// <summary>
		/// The image component that displays the slot background.
		/// </summary>
		private Image image;

		/// <summary>
		/// The object placement wheel.
		/// </summary>
		private ObjectPlacementWheel wheel;

		#endregion

		#region Methods

		private void Awake()
		{
			image = GetComponent<Image>();
			wheel = GetComponentInParent<ObjectPlacementWheel>();

			if (!wheel)
			{
				AppDebugger.LogError("Couldn't initialize the object placement wheel slot because the object placement wheel was not found!", this, nameof(ObjectPlacementWheelSlot));

				return;
			}
		}

		/// <summary>
		/// Initializes the slot on the wheel.
		/// </summary>
		/// <param name="index">The index of the slot.</param>
		/// <param name="totalCount">The total number of slots.</param>
		public void Initialize(int index, int totalCount)
		{
			if (totalCount < 1 || !image || !wheel)
				return;

			image.fillAmount = ((360f / totalCount) - wheel.slotSpacing) / 360f;
			transform.localEulerAngles = new(0, 0, -index * (360f / totalCount) - (wheel.slotSpacing / 2f));
		}

		#endregion
	}
}
