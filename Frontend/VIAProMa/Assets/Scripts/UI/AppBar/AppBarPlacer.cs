using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.UI.AppBar
{
    /// <summary>
    /// Handles the positioning of the app bar
    /// Extracted and modified from MRTK's AppBar script so that a custom menu structure can be used for the app bar
    /// </summary>
    public class AppBarPlacer : MonoBehaviour
    {
        private const float backgroundBarMoveSpeed = 5;

        [Header("Target Bounding Box")]
        [SerializeField]
        private BoundsControl boundsControl;

        [Header("Scale & Position Options")]

        [Tooltip("Pushes the app bar away from the object.")]
        [SerializeField]
        private float hoverOffsetZ = 0.05f;

        [Tooltip("The 2D offset of the app bar (x and y direction)")]
        [SerializeField] private Vector2 positionOffset2D;

        private BoundingBoxHelper helper = new BoundingBoxHelper();
        private List<Vector3> boundsPoints = new List<Vector3>();

        /// <summary>
        /// The target bounding box to which the app bar is attached
        /// </summary>
        public BoundsControl TargetBoundingBox
        {
            get => boundsControl;
            set => boundsControl = value;
        }

        /// <summary>
        /// Pushes the app bar away from the object
        /// </summary>
        public float HoverOffsetZ
        {
            get { return hoverOffsetZ; }
            set { hoverOffsetZ = value; }
        }

        /// <summary>
        /// Smoothly updates the position of the app bar
        /// </summary>
        private void Update()
        {
            FollowBoundingBox(true);
        }

        /// <summary>
        /// Sets the position of the app bar in relation to the target bounding box
        /// </summary>
        /// <param name="smooth">If true, the app bar is interpolated smoothly from its current position to the target position; otherwise it will immediately jump to the target position</param>
        private void FollowBoundingBox(bool smooth)
        {
            if (boundsControl == null)
                return;

            //calculate best follow position for AppBar
            Vector3 finalPosition = Vector3.zero;
            Vector3 headPosition = Camera.main.transform.position;
            boundsPoints.Clear();
            helper.GetRawBoundsCorners(boundsControl.TargetBounds);
            helper.UpdateNonAABoundsCornerPositions(boundsControl.TargetBounds, boundsPoints);
            int followingFaceIndex = helper.GetIndexOfForwardFace(headPosition);
            Vector3 faceNormal = helper.GetFaceNormal(followingFaceIndex);

            //finally we have new position
            finalPosition = helper.GetFaceBottomCentroid(followingFaceIndex) + (faceNormal * HoverOffsetZ) + new Vector3(positionOffset2D.x, positionOffset2D.y, 0);

            // Follow our bounding box
            transform.position = smooth ? Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * backgroundBarMoveSpeed) : finalPosition;

            // Rotate on the y axis
            Vector3 direction = (boundsControl.TargetBounds.bounds.center - finalPosition).normalized;
            if (direction != Vector3.zero)
            {
                Vector3 eulerAngles = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
                eulerAngles.x = 0f;
                eulerAngles.z = 0f;
                transform.eulerAngles = eulerAngles;
            }
            else
            {
                transform.eulerAngles = Vector3.zero;
            }
        }
    }
}