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

        private Vector3 newHandlePositionPositive;
        private bool handlePositvWasModified;

        private Vector3 newHandlePositionNegative;
        private bool handleNegativWasModified;

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
            //if (handlePositvWasModified || handleNegativWasModified)
            //{
            //    AdjustLengthToHandels();
            //    handlePositvWasModified = false;
            //    handleNegativWasModified = false;
            //}
        }

        public void SetHandles(Vector3 PointerPos, bool pos)
        {

            if (pos)
            {
                newHandlePositionPositive = new Vector3(capPos.localPosition.x - CalculateHandlePosition(lastPointerPosPos, PointerPos), 0, 0);
                newHandlePositionPositive = transform.localToWorldMatrix * new Vector4(newHandlePositionPositive.x, 0,0,1);
                lastPointerPosPos = PointerPos;
                handlePositvWasModified = true;
            }
            else
            {
                newHandlePositionNegative = new Vector3(capNeg.localPosition.x - CalculateHandlePosition(lastPointerPosNeg, PointerPos), 0, 0);
                newHandlePositionNegative = transform.localToWorldMatrix * new Vector4(newHandlePositionNegative.x, 0, 0, 1);
                lastPointerPosNeg = PointerPos;
                handleNegativWasModified = true;
            }

            AdjustLengthToHandels();
        }

        private float CalculateHandlePosition(Vector3 lastPosition, Vector3 position)
        {
            Vector3 delta = lastPosition - position;
            return Vector3.Dot(transform.right, delta);
        }

        private void AdjustLengthToHandels()
        {
            
            if (!handlePositvWasModified)
            {
                newHandlePositionPositive = capPos.position;
            }
            if (!handleNegativWasModified)
            {
                newHandlePositionNegative = capNeg.position;
            }

            //Adjust the tube
            float newLength = Vector3.Distance(newHandlePositionPositive, newHandlePositionNegative);
            if (newLength >= minLength && newLength <= maxLength)
            {
                tubes.localScale = new Vector3(newLength, 1f, 1f);

                //Adjust the parent
                transform.position = newHandlePositionNegative + 0.5f * (newHandlePositionPositive - newHandlePositionNegative);

                //The position of the caps is set at the end, so they arent affected by the translation of there parent
                capPos.position = newHandlePositionPositive;
                capNeg.position = newHandlePositionNegative;

                UpdateTextLabelPositioning(newLength);


                // also update box colliders and bounding box
                tubeCollider.height = newLength + 0.1f; // add 0.1 so that the cylindrical part covers the full length (otherwise it is too short because of the rounded caps)
                boundingBoxCollider.size = new Vector3(
                    newLength + 0.05f, // add 0.05f to encapsulate the end caps
                    boundingBoxCollider.size.y,
                    boundingBoxCollider.size.z);
                boundingBox.Refresh();
            }
        }

        public void StopResizing(bool pos)
        {
            if (pos)
            {
                newHandlePositionPositive = Vector3.negativeInfinity;
                handlePositvWasModified = false;
            }
            else
            {
                newHandlePositionNegative = Vector3.negativeInfinity;
                handleNegativWasModified = false;
            }         
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