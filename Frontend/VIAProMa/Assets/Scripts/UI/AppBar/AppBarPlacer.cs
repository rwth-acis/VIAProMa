using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the positioning of the app bar
/// Extracted from MRTK's AppBar script so that a custom menu structure can be used for the app bar
/// </summary>
public class AppBarPlacer : MonoBehaviour
{
    private const float backgroundBarMoveSpeed = 5;

    [Header("Target Bounding Box")]
    [SerializeField]
    private BoundingBox boundingBox;

    [Header("Scale & Position Options")]

    [SerializeField]
    [Tooltip("Pushes the app bar away from the object.")]
    private float hoverOffsetZ = 0.05f;

    [SerializeField] private Vector2 positionOffset2D;

    private BoundingBoxHelper helper = new BoundingBoxHelper();
    private List<Vector3> boundsPoints = new List<Vector3>();

    public BoundingBox TargetBoundingBox
    {
        get => boundingBox;
        set => boundingBox = value;
    }

    /// <summary>
    /// Pushes the app bar away from the object
    /// </summary>
    public float HoverOffsetZ
    {
        get { return hoverOffsetZ; }
        set { hoverOffsetZ = value; }
    }

    private void Update()
    {
        FollowBoundingBox(true);
    }

    private void FollowBoundingBox(bool smooth)
    {
        if (boundingBox == null)
            return;

        //calculate best follow position for AppBar
        Vector3 finalPosition = Vector3.zero;
        Vector3 headPosition = Camera.main.transform.position;
        boundsPoints.Clear();

        helper.UpdateNonAABoundingBoxCornerPositions(boundingBox, boundsPoints);
        int followingFaceIndex = helper.GetIndexOfForwardFace(headPosition);
        Vector3 faceNormal = helper.GetFaceNormal(followingFaceIndex);

        //finally we have new position
        finalPosition = helper.GetFaceBottomCentroid(followingFaceIndex) + (faceNormal * HoverOffsetZ) + new Vector3(positionOffset2D.x, positionOffset2D.y, 0);

        // Follow our bounding box
        transform.position = smooth ? Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * backgroundBarMoveSpeed) : finalPosition;

        // Rotate on the y axis
        Vector3 direction = (boundingBox.TargetBounds.bounds.center - finalPosition).normalized;
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
