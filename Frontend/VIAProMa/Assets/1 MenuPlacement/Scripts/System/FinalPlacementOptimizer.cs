using MenuPlacement;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

/// <summary>
/// This solver should only be used for the Menu Placement System and it fine-tunes the transform of the menu based on several offsets.
/// Be sure to order this at the lowest position of all solvers except ConstantViewSize.
/// Its modification of scales will be overwritten by ConstantViewSize, if enabled.
/// </summary>
public class FinalPlacementOptimizer : Solver
{
    [Tooltip("XYZ offset for this object oriented with the TrackedObject/TargetTransform's forward. X offset will be minus if this object should be placed on the left side of the TrackedObject/TargetTransform")]
    [SerializeField] private Vector3 orbitalOffset = Vector3.zero;

    private Vector3 positionOffset = Vector3.zero;
    private Vector3 rotationOffset = Vector3.zero;
    private Vector3 scaleOffset = Vector3.one;

    public MenuHandler.MenuOrientationType OrientationType { get; set; }

    public Vector3 PositionWithoutOffset { get; private set; }
    public Vector3 OriginalScale { get; set; }

    public Vector3 OrbitalOffset
    {
        get { return orbitalOffset; }
        set { orbitalOffset = value; }
    }

    public Vector3 FinalPosition { get; set; }
    public Quaternion FinalRotation { get; set; }

    public Vector3 PositionOffset 
    {
        get => positionOffset;
        set { positionOffset = value; }
    }
    public Vector3 RotationOffset
    {
        get => rotationOffset;
        set { rotationOffset = value; }
    }
    public Vector3 ScaleOffset
    {
        get => scaleOffset;
        set { scaleOffset = value; }
    }

    public override void SolverUpdate() { 
        Camera head = CameraCache.Main;
        if (gameObject.GetComponent<MenuHandler>().menuVariantType == MenuHandler.MenuVariantType.MainMenu) {
            if (gameObject.GetComponent<MenuHandler>().compact) {
                GoalPosition += head.transform.right * positionOffset.x + head.transform.up * positionOffset.y + head.transform.forward * positionOffset.z;
            }
        }
        else {
            if (gameObject.GetComponent<Orbital>().enabled) {
               
                Vector3 directionToHead = head.transform.position - SolverHandler.TransformTarget.position;
                bool rightSide = Vector3.Dot(directionToHead, head.transform.right) > 0 ? true : false;

                if (rightSide) {
                    Vector3 finalOffset = head.transform.right * (orbitalOffset.x + positionOffset.x) + head.transform.up * (orbitalOffset.y + positionOffset.y) + head.transform.forward * (orbitalOffset.z + positionOffset.z);
                    GoalPosition += finalOffset;
                }
                else {
                    Vector3 finalOffset = - head.transform.right * (orbitalOffset.x + positionOffset.x) + head.transform.up * (orbitalOffset.y + positionOffset.y) + head.transform.forward * (orbitalOffset.z + positionOffset.z);
                    GoalPosition += finalOffset;
                }
            }
            else {
                GoalPosition += head.transform.right * positionOffset.x + head.transform.up * positionOffset.y + head.transform.forward * positionOffset.z;
            }
        }
        
        GoalRotation = Quaternion.Euler(head.transform.rotation.eulerAngles + RotationOffset);
        FinalPosition = GoalPosition;
        FinalRotation = GoalRotation;
        GoalScale = new Vector3(OriginalScale.x * ScaleOffset.x, OriginalScale.y * ScaleOffset.y, OriginalScale.z * ScaleOffset.z);
    }
}
