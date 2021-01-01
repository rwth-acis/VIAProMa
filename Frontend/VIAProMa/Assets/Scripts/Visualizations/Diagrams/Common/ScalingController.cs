using i5.VIAProMa.Utilities;
using UnityEngine;

namespace i5.VIAProMa.Visualizations.Common
{
    public class ScalingController : MonoBehaviour
    {
        [SerializeField] private i5.VIAProMa.Visualizations.Common.Diagram diagram;
        [SerializeField] private Vector3 minSize = 0.1f * Vector3.one;
        [SerializeField] private MovementSlider xPosSlider;
        [SerializeField] private MovementSlider xNegSlider;
        [SerializeField] private MovementSlider yPosSlider;
        [SerializeField] private MovementSlider yNegSlider;
        [SerializeField] private MovementSlider zPosSlider;
        [SerializeField] private MovementSlider zNegSlider;

        private void Awake()
        {
            if (diagram == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(diagram));
            }
            if (xPosSlider == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(xPosSlider));
            }
            if (xNegSlider == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(xNegSlider));
            }
            if (yPosSlider == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(yPosSlider));
            }
            if (yNegSlider == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(yNegSlider));
            }
            if (zPosSlider == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(zPosSlider));
            }
            if (zNegSlider == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(zNegSlider));
            }
        }
    }
}