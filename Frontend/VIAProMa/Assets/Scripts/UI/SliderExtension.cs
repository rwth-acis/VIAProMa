using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

namespace i5.VIAProMa.UI
{
    /// <summary>
    /// Extends the slider of the mixed reality toolkit
    /// Adds the feature that values from a range can be selected from the slider (instead of float values between 0 and 1)
    /// </summary>
    [RequireComponent(typeof(PinchSlider))]
    public class SliderExtension : MonoBehaviour
    {
        [SerializeField] TextMeshPro thumbValueLabel;
        [SerializeField] TextMeshPro minValueLabel;
        [SerializeField] TextMeshPro maxValueLabel;

        /// <summary>
        /// If true, only integers can be selected
        /// </summary>
        public bool roundToInt = true;

        /// <summary>
        /// The minimum value of the selectable range
        /// </summary>
        public int minValue;
        /// <summary>
        /// The maximum value of the selectable range
        /// </summary>
        public int maxValue;

        /// <summary>
        /// The slider component of the Mixed Reality Toolkit
        /// </summary>
        private PinchSlider slider;

        /// <summary>
        /// The currently selected value
        /// </summary>
        public float Value
        {
            get
            {
                return (maxValue - minValue) * slider.SliderValue + minValue;
            }
            set
            {
                value = Mathf.Clamp(value, minValue, maxValue); // make sure that the value is in range
                slider.SliderValue = (value - minValue) / (maxValue - minValue);
            }
        }

        /// <summary>
        /// The currently selected value rounded to the nearest integer and cast to int
        /// </summary>
        public int ValueInt
        {
            get
            {
                return Mathf.RoundToInt(Value);
            }
            set
            {
                Value = value;
            }
        }

        /// <summary>
        /// Checks the component's setup
        /// </summary>
        private void Awake()
        {
            slider = GetComponent<PinchSlider>();

            if (thumbValueLabel == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(thumbValueLabel));
            }
            if (minValueLabel == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(minValueLabel));
            }
            if (maxValueLabel == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(maxValueLabel));
            }
        }

        /// <summary>
        /// Called if the slider value is updated
        /// Updates the slider display to show the correct values
        /// </summary>
        /// <param name="data">Event data of the slider changed event</param>
        private void UpdateDisplay(SliderEventData data)
        {
            if (roundToInt)
            {
                thumbValueLabel.text = ValueInt.ToString();
            }
            else
            {
                thumbValueLabel.text = Value.ToString("0.0");
            }

            minValueLabel.text = minValue.ToString();
            maxValueLabel.text = maxValue.ToString();
        }

        /// <summary>
        /// Adds the UpdateDisplay method as a listener for slider change updates
        /// </summary>
        private void OnEnable()
        {
            slider.OnValueUpdated.AddListener(UpdateDisplay);
        }

        /// <summary>
        /// Removes the UpdateDisplay method as a listener for slider change updates
        /// </summary>
        private void OnDisable()
        {
            slider.OnValueUpdated.RemoveListener(UpdateDisplay);
        }
    }
}