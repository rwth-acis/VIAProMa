using ExitGames.Client.Photon.StructWrapping;
using MenuPlacement;
using Microsoft.MixedReality.Toolkit.Experimental.StateVisualizer;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// This solver should only be used for the Menu Placement System and should be added to object menus.
/// If TrackedObject/TargetTransform is left to the head, the object this solver attaching to will be placed on the right side of the it, and vice versa.
/// Note that it computes the direction and postion using the "transform.position" property.
/// Be sure to order this at the lowest position in the inspector.
/// </summary>
public class FinalPlacementOptimizer : Solver
{
    [Tooltip("XYZ offset for this object oriented with the TrackedObject/TargetTransform's forward. X offset will be minus if this object should be placed on the left side of the TrackedObject/TargetTransform")]
    [SerializeField] private Vector3 orbitalOffset = Vector3.zero;

    private Vector3 positionOffset = Vector3.zero;
    private Vector3 rotationOffset = Vector3.zero;
    private Vector3 scaleOffset = Vector3.one;

    public MenuHandler.MenuOrientationType OrientationType { get; set; }

    public Vector3 LastPosition { get; set; }

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
        if (gameObject.GetComponent<MenuHandler>().isMainMenu) {
            if (gameObject.GetComponent<MenuHandler>().isCompact) {
                GoalPosition += positionOffset;
            }
        }
        else {
            if (gameObject.GetComponent<Orbital>().enabled) {
                Vector3 directionToHead = head.transform.position - SolverHandler.TransformTarget.position;
                bool rightSide = Vector3.Dot(directionToHead, head.transform.right) > 0 ? true : false;

                if (rightSide) {
                    PositionWithoutOffset = SolverHandler.TransformTarget.position + head.transform.right * orbitalOffset.x + head.transform.up * orbitalOffset.y + head.transform.forward * orbitalOffset.z;
                    GoalPosition = PositionWithoutOffset + PositionOffset;
                }
                else {
                    PositionWithoutOffset = SolverHandler.TransformTarget.position + (-head.transform.right) * orbitalOffset.x + head.transform.up * orbitalOffset.y + head.transform.forward * orbitalOffset.z;
                    GoalPosition = PositionWithoutOffset + new Vector3(-PositionOffset.x, PositionOffset.y, PositionOffset.z);
                }
            }
            else {
                GoalPosition += positionOffset;
            }
        }
        
        GoalRotation = Quaternion.Euler(head.transform.rotation.eulerAngles + RotationOffset);
        FinalPosition = GoalPosition;
        FinalRotation = GoalRotation;
        GoalScale = new Vector3(OriginalScale.x * ScaleOffset.x, OriginalScale.y * ScaleOffset.y, OriginalScale.z * ScaleOffset.z);
    }
}
