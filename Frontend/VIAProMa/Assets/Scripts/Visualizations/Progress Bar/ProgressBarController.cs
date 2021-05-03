using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine;
using System.Threading.Tasks;

namespace i5.VIAProMa.Visualizations.ProgressBars
{
    /// <summary>
    /// Visual controller for the progress bar
    /// Handles the data visualization and the non-uniform scaling
    /// </summary>
    public class ProgressBarController : MonoBehaviour, IProgressBarVisuals
    {
        [Header("Visualization Elements")]
        [Tooltip("End cap of the progress bar at the positive end")]
        [SerializeField] private Transform capPos;
        [Tooltip("End cap of the progress bar at the negative end")]
        [SerializeField] private Transform capNeg;
        [Tooltip("Parent transform containing all tube GameObjects")]
        [SerializeField] private Transform tubes;
        [Tooltip("Bar which visualizes how many issues are done")]
        [SerializeField] private Transform innerBarDone;
        [Tooltip("Bar which visualizes how many isses are in progress")]
        [SerializeField] private Transform innerBarInProgress;
        [Tooltip("The collider of the progress bar")]
        [SerializeField] private CapsuleCollider tubeCollider;

        [Header("References")]
        [Tooltip("Reference to the bounding box of the progress bar")]
        [SerializeField] private BoundingBox boundingBox;
        [Tooltip("Reference to the title label of the progress bar")]
        [SerializeField] private TextLabel textLabel;

        /// <summary>
        /// Minimum length of the progress bar
        /// </summary>
        public float minLength = 0.05f;
        /// <summary>
        /// Maximum length of the progress bar
        /// </summary>
        public float maxLength = 3f;

        private float percentageDone;
        private float percentageInProgress;

        private BoxCollider boundingBoxCollider;

        [System.NonSerialized]
        public Vector3 lastPointerPosPos = Vector3.negativeInfinity;
        [System.NonSerialized]
        public Vector3 lastPointerPosNeg = Vector3.negativeInfinity;

        private bool wasResized = false;

        /// <summary>
        /// Gets the length of the progress bar (at overall scale 1)
        /// </summary>
        public float Length
        {
            get => tubes.localScale.x;
            set
            {
                tubes.localScale = new Vector3(
                    value,
                    tubes.localScale.y,
                    tubes.localScale.z);

                capPos.localPosition = new Vector3(value / 2f, 0f, 0f);
                capNeg.localPosition = new Vector3(-value / 2f, 0f, 0f);

                // also update box colliders and bounding box
                tubeCollider.height = value + 0.1f; // add 0.1 so that the cylindrical part covers the full length (otherwise it is too short because of the rounded caps)
                boundingBoxCollider.size = new Vector3(
                    value + 0.05f, // add 0.05f to encapsulate the end caps
                    boundingBoxCollider.size.y,
                    boundingBoxCollider.size.z);
                boundingBox.Refresh();

                UpdateTextLabelPositioning(value);
            }
        }

        /// <summary>
        /// Gets or sets the title of the progress bar and applies it to the text label
        /// </summary>
        public string Title
        {
            get => textLabel.Text;
            set => textLabel.Text = value;
        }

        public float PercentageDone
        {
            get => percentageDone;
            set
            {
                percentageDone = Mathf.Clamp01(value);
                UpdateVisuals();
            }
        }

        public float PercentageInProgress
        {
            get => percentageInProgress;
            set
            {
                percentageInProgress = Mathf.Clamp01(value);
                UpdateVisuals();
            }
        }

        private void Awake()
        {
            if (capPos == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(capPos));
            }
            if (capNeg == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(capNeg));
            }
            if (tubes == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(tubes));
            }
            if (innerBarDone == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(innerBarDone));
            }
            if (innerBarInProgress == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(innerBarInProgress));
            }
            if (tubeCollider == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(tubeCollider));
            }
            if (boundingBox == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(boundingBox));
            }
            boundingBoxCollider = boundingBox?.gameObject.GetComponent<BoxCollider>();
            if (boundingBox == null)
            {
                SpecialDebugMessages.LogComponentNotFoundError(this, nameof(BoundingBox), boundingBoxCollider?.gameObject);
            }

            if (textLabel == null)
            {
                SpecialDebugMessages.LogMissingReferenceError(this, nameof(textLabel));
            }

            UpdateVisuals();
            UpdateTextLabelPositioning(Length);
        }

        private void UpdateVisuals()
        {
            if (innerBarDone == null || innerBarInProgress == null)
            {
                return;
            }
            float doneBarScale = percentageDone;
            float inProgressBarScale = percentageInProgress;

            if (percentageDone == 0)
            {
                doneBarScale = 0.001f;
            }
            if (percentageInProgress == 0)
            {
                inProgressBarScale = 0.001f;
            }
            else
            {
                inProgressBarScale = Mathf.Clamp(percentageInProgress, 0, 1 - percentageDone);
            }

            innerBarDone.localScale = new Vector3(doneBarScale, 1f, 1f);
            innerBarInProgress.localPosition = new Vector3(-0.5f + doneBarScale, 0f, 0f);
            innerBarInProgress.localScale = new Vector3(inProgressBarScale, 1f, 1f);
        }

        public void LateUpdate()
        {
            if (wasResized)
            {
                AdjustLengthToHandels();
                wasResized = false;
            }
        }

        public void SetHandles(Vector3 PointerPos, bool pos)
        {

            if (pos)
            {
                capPos.localPosition = new Vector3(capPos.localPosition.x - CalculateHandlePosition(lastPointerPosPos, PointerPos), 0, 0);
                lastPointerPosPos = PointerPos;
            }
            else
            {
                capNeg.localPosition = new Vector3(capNeg.localPosition.x - CalculateHandlePosition(lastPointerPosNeg, PointerPos), 0, 0);
                lastPointerPosNeg = PointerPos;
            }
            wasResized = true;
        }

        private float CalculateHandlePosition(Vector3 lastPosition, Vector3 position)
        {
            Vector3 delta = lastPosition - position;
            return Vector3.Dot(transform.right, delta);
        }

        private void AdjustLengthToHandels()
        {
            //Adjust the tube
            float newLength = Vector3.Distance(capPos.position,capNeg.position);
            tubes.localScale = new Vector3(newLength, 1f, 1f);

            //Adjust the parent
            Vector3 capPosPosition = capPos.position;
            Vector3 capNegPosition = capNeg.position;

            transform.position = 0.5f * (capPos.position - capNeg.position) + capNeg.position;

            capPos.position = capPosPosition;
            capNeg.position = capNegPosition;

            UpdateTextLabelPositioning(newLength);
        }

        private void UpdateTextLabelPositioning(float progressBarLength)
        {
            textLabel.MaxWidth = progressBarLength / 2f;
            textLabel.transform.localPosition = new Vector3(
                -progressBarLength / 2f + progressBarLength / 4f,
            textLabel.transform.localPosition.y,
                textLabel.transform.localPosition.z);
        }
    }
}