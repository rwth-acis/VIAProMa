using ExitGames.Client.Photon.StructWrapping;
using MenuPlacement;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FinalPlacementOptimizer : Solver
{

    /// <summary>
    /// This solver should only be used for the Menu Placement System and should be added to object menus.
    /// If TrackedObject/TargetTransform is left to the head, the object this solver attaching to will be placed on the right side of the it, and vice versa.
    /// Note that it computes the direction and postion using the "transform.position" property.
    /// Be sure to order this at the lowest position in the inspector.
    /// </summary>

    [Tooltip("XYZ offset for this object oriented with the TrackedObject/TargetTransform's forward. X offset will be minus if this object should be placed on the left side of the TrackedObject/TargetTransform")]
    [SerializeField] private Vector3 orbitalOffset = Vector3.zero;

    public MenuHandler.MenuOrientationType OrientationType { get; set; }

    public bool TryToAvoidOcclusion { get; set; }

    public Vector3 LastPosition { get; set; }

    public Vector3 OrbitalOffset
    {
        get { return orbitalOffset; }
        set { orbitalOffset = value; }
    }

    public override void SolverUpdate() {
        Camera head = CameraCache.Main;
        if (gameObject.GetComponent<Orbital>().enabled) {
            Vector3 directionToHead = head.transform.position - SolverHandler.TransformTarget.position;
            bool rightSide = Vector3.Dot(directionToHead, head.transform.right) > 0 ? true : false;

            if (rightSide) {
                GoalPosition = SolverHandler.TransformTarget.position + head.transform.right * orbitalOffset.x + head.transform.up * orbitalOffset.y + head.transform.forward * orbitalOffset.z;
            }
            else {
                GoalPosition = SolverHandler.TransformTarget.position + (-head.transform.right) * orbitalOffset.x + head.transform.up * orbitalOffset.y + head.transform.forward * orbitalOffset.z;
            }
        }
        GoalRotation = head.transform.rotation;    
    }
}
