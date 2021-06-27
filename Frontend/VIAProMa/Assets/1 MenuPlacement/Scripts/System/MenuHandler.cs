using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using ExitGames.Client.Photon.StructWrapping;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.Utilities;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.WSA;

namespace MenuPlacement {

    public class MenuHandler : MonoBehaviour {

        public enum BoundsType {
            BasedOnColliders,
            BasedOnRenderers
        }

        public enum MenuOrientationType {
            CameraAligned
        }

        #region Serialize Fields
        [Header("General Properties")]
        [Tooltip("True, if the menu is an object menu. False, if the menu is a main menu")]
        public bool isMainMenu;
        [Tooltip("Is the menu a normal one or a compact one?")]
        public bool isCompact;
        [Tooltip("The ID of this menu. Make sure they are different from one to another")]
        public int menuID;
        [Tooltip("The bounding box will be used to decide whether the space is enough to place the menu. It is a 'Bounds' object containing all Bounds of the corresponding base")]
        [SerializeField] private BoundsType boudingBoxType = BoundsType.BasedOnColliders;
        [Tooltip("The orientation type used on the solver attached to the object by the placement system if applicable")]
        [SerializeField] private MenuOrientationType menuOrientationType = MenuOrientationType.CameraAligned;

        [Header("Offsets")]
        [SerializeField] private float maxFloatingDistance = 0.6f;
        [SerializeField] private float minFloatingDistance = 0.3f;
        [Tooltip("Position Offsets To The Target Object. For object menus, the additive inverse of the X (right) offset will be taken if the menu is on the left side of the targetObjects (e.g. 2 to -2).")]
        [SerializeField] private Vector3 orbitalOffset = Vector3.zero;
        [Tooltip("Offsets for the distance between the surface and the attached menu, i.e. the 'Surface Normal Offset' of the SurfaceMagnetism solver")]
        [SerializeField] private float surfaceMagnetismSafetyOffset = 0.05f;


        #endregion Serialize Fields

        private MenuPlacementService placementManager;
        private PlacementMessage message = new PlacementMessage();
        private PlacementMessage.SolverActivated currentSolvers;
        private PlacementMessage.SwitchType switchTo = PlacementMessage.SwitchType.NoSwitch;
        private bool collisionDetected = false;
        private Camera head;
        private float defaultFloatingDistance;
        private Vector3 orbitalOffsetOppositeType;
        public GameObject targetObject { get; private set; }
        public Vector3 OrbitalOffset
        {
            get => orbitalOffset;
            set
            {
                orbitalOffset = value;
            }
        }


        // Start is called before the first frame update
        void Start() {
            defaultFloatingDistance = (minFloatingDistance + maxFloatingDistance) / 2;
            placementManager = ServiceManager.GetService<MenuPlacementService>();
            head = CameraCache.Main;
            if (isMainMenu) {
                InitializeMainMenu();
            }
        }

        // Update is called once per frame
        void Update() {
            CheckSpatialMapping();
            CheckOcclusion();
            if (switchTo != PlacementMessage.SwitchType.NoSwitch) {
                Debug.Log(message.switchType);
                placementManager.UpdatePlacement(message, gameObject);
                switchTo = PlacementMessage.SwitchType.NoSwitch;
            }
        }
        private void OnDrawGizmos() {
            foreach (Collider c in gameObject.GetComponentsInChildren<Collider>()) {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawCube(c.bounds.center, c.bounds.size);
            }

            /*foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>()) {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                Gizmos.DrawCube(r.bounds.center, r.bounds.size);
            }*/
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawCube(GetBoundingBox().center, GetBoundingBox().size);
        }
        public void Open(GameObject targetObject) {
            Debug.Log("Menu Open");
            //Find all menus in the scene. If there is already one menu for the targetObject, no menu will be opened.
            MenuHandler [] menus = FindObjectsOfType<MenuHandler>();
            foreach(MenuHandler m in menus) {
                if(m.menuID == menuID && m.targetObject == targetObject) {
                    return;
                }
            }
            if (placementManager == null) {
                placementManager = ServiceManager.GetService<MenuPlacementService>();
            }
            GameObject menu = placementManager.OpenMenu(gameObject);
            menu.GetComponent<MenuHandler>().orbitalOffsetOppositeType = placementManager.GetOrbitalOffsetOppositeType(menu);
            menu.GetComponent<MenuHandler>().head = CameraCache.Main;
            //Initialize the solvers for object menus.
            if (!isMainMenu) {
                menu.transform.position = targetObject.transform.position;
                menu.GetComponent<MenuHandler>().targetObject = targetObject;
                (menu.GetComponent<SolverHandler>() ?? menu.AddComponent<SolverHandler>()).TrackedTargetType = TrackedObjectType.CustomOverride;
                menu.GetComponent<SolverHandler>().TransformOverride = targetObject.transform;
                float targetDistance = (menu.GetComponent<MenuHandler>().head.transform.position - targetObject.transform.position).magnitude;
                (menu.GetComponent<Orbital>() ?? menu.AddComponent<Orbital>()).UpdateLinkedTransform = true;
                menu.GetComponent<Orbital>().OrientationType = SolverOrientationType.Unmodified;
                menu.GetComponent<Orbital>().LocalOffset = Vector3.zero;
                menu.GetComponent<Orbital>().enabled = true;
                //For floating version, add the InBetween solver additionally for far manipulation
                if (!isCompact) {                 
                    (menu.GetComponent<InBetween>() ?? menu.AddComponent<InBetween>()).SecondTrackedObjectType = TrackedObjectType.CustomOverride;
                    menu.GetComponent<InBetween>().UpdateLinkedTransform = true;
                    menu.GetComponent<InBetween>().SecondTransformOverride = placementManager.GetInBetweenTarget().transform;
                    menu.GetComponent<InBetween>().PartwayOffset = (defaultFloatingDistance / targetDistance);
                    if (targetDistance > maxFloatingDistance) {
                        menu.GetComponent<Orbital>().enabled = false;
                    }
                    else {
                        menu.GetComponent<InBetween>().enabled = false;
                    }
                }
                //For compact version, add the HandConstraint solver additionally for interaction in very narror space.
                else {
                    (menu.GetComponent<HandConstraint>() ?? menu.AddComponent<HandConstraint>()).SafeZone = HandConstraint.SolverSafeZone.RadialSide;
                    menu.GetComponent<HandConstraint>().SafeZoneBuffer = 0.05f;
                    //HandConstraint is always deactivated after initializing. Refer to the collision handling process in CheckSpatialMapping() for more detail.                   
                    menu.GetComponent<HandConstraint>().enabled = false;
                }
                (menu.GetComponent<FinalPlacementOptimizer>() ?? menu.AddComponent<FinalPlacementOptimizer>()).OrbitalOffset = orbitalOffset;
                menu.GetComponent<FinalPlacementOptimizer>().OrientationType = menuOrientationType;
                menu.GetComponent<FinalPlacementOptimizer>().enabled = true;
            }
            menu.SetActive(true);
        }

        public void Close() {
            placementManager.StoreBoundingBoxOnClose(gameObject, GetBoundingBox());
            placementManager.CloseMenu(gameObject);
            gameObject.SetActive(false);    
        }

        public Vector3 GetOffset() {
            return orbitalOffset;
        }

        #region Private Methods

        //This function should be called just once for one main menu
        private void InitializeMainMenu() {  
            if (!isCompact) {
                gameObject.AddComponent<SolverHandler>().AdditionalOffset = orbitalOffset;
                gameObject.AddComponent<SurfaceMagnetism>().SurfaceNormalOffset = surfaceMagnetismSafetyOffset;
                gameObject.GetComponent<SurfaceMagnetism>().UpdateLinkedTransform = true;
                gameObject.GetComponent<SurfaceMagnetism>().CurrentOrientationMode = SurfaceMagnetism.OrientationMode.None;
                gameObject.GetComponent<SurfaceMagnetism>().MagneticSurfaces[0] = LayerMask.GetMask("Spatial Mapping");
                gameObject.GetComponent<SurfaceMagnetism>().MaxRaycastDistance = maxFloatingDistance;
                gameObject.GetComponent<SurfaceMagnetism>().ClosestDistance = minFloatingDistance;
                gameObject.AddComponent<Follow>().MinDistance = minFloatingDistance;
                gameObject.GetComponent<Follow>().MaxDistance = maxFloatingDistance;
                gameObject.GetComponent<Follow>().DefaultDistance = defaultFloatingDistance;
                gameObject.GetComponent<Follow>().MaxViewHorizontalDegrees = 60f;
                gameObject.GetComponent<Follow>().MaxViewVerticalDegrees = 50f;
                gameObject.GetComponent<Follow>().OrientToControllerDeadzoneDegrees = 20f;
            }
            else {
                gameObject.AddComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                gameObject.AddComponent<HandConstraint>().SafeZone = HandConstraint.SolverSafeZone.RadialSide;
                gameObject.GetComponent<HandConstraint>().SafeZoneBuffer = 0.05f;
            }
        }

        private void CheckSpatialMapping() {
            float distanceToMenu = Vector3.Dot(gameObject.transform.position - head.transform.position, head.transform.forward);
            //Use to check main menu
            bool closeToSpatialMapping = Physics.Raycast(head.transform.position, head.transform.forward, minFloatingDistance + surfaceMagnetismSafetyOffset, LayerMask.GetMask("Spatial Mapping"));
            //Debug.Log(distanceToMenu);
            //Debug.Log(closeToSpatialMapping);
            Debug.Log("Collide with Spatial Mapping Self = " + CollideWithSpatialMappingSelf());
            if (CollideWithSpatialMappingSelf() || (distanceToMenu < minFloatingDistance && closeToSpatialMapping)) {
                if (isMainMenu) {
                    if (!isCompact) {
                        // Cast a ray towards the position of the menu from the camera, if the menu is closer than the minFloatingDistance, switch to the compact version.
                        Ray ray = new Ray(head.transform.position, GetBoundingBox().center - head.transform.position);
                        if (Physics.Raycast(ray, minFloatingDistance, LayerMask.GetMask("Menu"))) {
                            message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                            switchTo = message.switchType;
                        }
                    }
                    //Compact menus don't need to be handled here.
                }
                else {
                    float targetDistance = (head.transform.position - targetObject.transform.position).magnitude;
                    //If the target object is far away, namely the InBetween solver is activated.
                    if (targetDistance > maxFloatingDistance) {
                        if (!isCompact) {
                            message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                            switchTo = message.switchType;
                            //Because the object is far, the HandConstraint solver should be activated on next call.
                        }
                        //Activate the HandConstraint solver
                        else {
                            gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                            gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                            gameObject.GetComponent<HandConstraint>().enabled = true;
                            gameObject.GetComponent<FinalPlacementOptimizer>().enabled = false;
                            gameObject.GetComponent<Orbital>().enabled = false;
                        }
                    }
                    //The object is close, namely the Beside and Orbital solvers are activated.
                    //Begin the collision handling process:
                    //First step: Try to switch to compact version if it is a floating menu
                    if (!isCompact) { 
                        message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                        switchTo = message.switchType;
                    }
                    //Second step: If it is already compact and there is still a collision, then activate the HandConstraint solver.
                    else {
                        gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                        gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                        gameObject.GetComponent<HandConstraint>().enabled = true;
                        gameObject.GetComponent<FinalPlacementOptimizer>().enabled = false;
                        gameObject.GetComponent<Orbital>().enabled = false;
                    }
                    
                }
            }
            else {
                if (isMainMenu) {
                    if (isCompact) {
                        //check the bounding box of the floating menu from GetBoundingBox() in front of the user's head.
                        Vector3 center = head.transform.position + head.transform.forward * minFloatingDistance;
                        if (!Physics.CheckBox(center, placementManager.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))) {
                            Debug.Log("Front Free for Floating main menu");
                            message.switchType = PlacementMessage.SwitchType.CompactToFloating;
                            switchTo = message.switchType;
                        }
                    }
                    //Floating menus don't need to be handled here.
                }
                else {
                    float targetDistance = (head.transform.position - targetObject.transform.position).magnitude;
                    if (!isCompact) {                
                        //The object is between maxFloatingDistance and minFloatingDistance, use the Orbital solver.
                        if (targetDistance <= maxFloatingDistance && targetDistance >= minFloatingDistance) {
                            gameObject.GetComponent<Orbital>().enabled = true;                           
                            gameObject.GetComponent<InBetween>().enabled = false;
                            gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
                        }
                        else if (targetDistance < minFloatingDistance) {
                            message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                            switchTo = message.switchType;
                        }
                        else if (targetDistance > maxFloatingDistance) {
                            //We should enable InBetween solver;
                            gameObject.GetComponent<Orbital>().enabled = false;
                            gameObject.GetComponent<InBetween>().enabled = true;
                            gameObject.GetComponent<InBetween>().PartwayOffset = (defaultFloatingDistance / targetDistance);
                            gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
                        }
                    }
                    //Compact 
                    else {
                        //Check space around and try to switch to floating version.
                        if (targetDistance > maxFloatingDistance) {
                            //Check the space in front of user because InBetween solver should be activated
                            Vector3 center = placementManager.GetInBetweenTarget().transform.position + Vector3.Normalize(targetObject.transform.position - placementManager.GetInBetweenTarget().transform.position) * defaultFloatingDistance;
                            if (!Physics.CheckBox(center, placementManager.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))) {
                                Debug.Log("Front Free for InBetween");
                                message.switchType = PlacementMessage.SwitchType.CompactToFloating;
                                switchTo = message.switchType;
                            }
                            else {
                                gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                                gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                                gameObject.GetComponent<HandConstraint>().enabled = true;
                                gameObject.GetComponent<FinalPlacementOptimizer>().enabled = false;
                                gameObject.GetComponent<Orbital>().enabled = false;
                            }                 
                        }                       
                        if (targetDistance >= minFloatingDistance && targetDistance <= maxFloatingDistance) {
                            //Orbital and Beside solver should be activated on the floating menu.
                            //Check the space "besides" the targetObject on the left or right side according to user's position, similar to Beside solver.
                            Vector3 directionToHead = head.transform.position - targetObject.transform.position;
                            bool rightSide = Vector3.Dot(directionToHead, head.transform.right) > 0 ? true : false;
                            Vector3 centerFloating = Vector3.zero;
                            Vector3 centerCompact = Vector3.zero;
                            if (rightSide) {
                                centerFloating = targetObject.transform.position + head.transform.right * orbitalOffsetOppositeType.x + head.transform.up * orbitalOffsetOppositeType.y + head.transform.forward * orbitalOffsetOppositeType.z;
                                centerCompact = targetObject.transform.position + head.transform.right * orbitalOffset.x + head.transform.up * orbitalOffset.y + head.transform.forward * orbitalOffset.z;                               
                            }
                            else {
                                centerFloating = targetObject.transform.position + ( - head.transform.right * orbitalOffsetOppositeType.x) + head.transform.up * orbitalOffsetOppositeType.y + head.transform.forward * orbitalOffsetOppositeType.z;
                                centerCompact = targetObject.transform.position + ( - head.transform.right * orbitalOffset.x) + head.transform.up * orbitalOffset.y + head.transform.forward * orbitalOffset.z;
                            }
                            if (!Physics.CheckBox(centerFloating, placementManager.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))
                                && !Physics.Raycast(targetObject.transform.position, centerFloating - targetObject.transform.position, (centerFloating - targetObject.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                                Debug.Log("Besides Free for Floating Orbital");
                                message.switchType = PlacementMessage.SwitchType.CompactToFloating;
                                switchTo = message.switchType;
                            }else if (!Physics.CheckBox(centerCompact, placementManager.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))
                                && !Physics.Raycast(targetObject.transform.position, centerCompact - targetObject.transform.position, (centerCompact - targetObject.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {                       
                                Debug.Log("Beside Free for Compact Orbital");
                                gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.CustomOverride;
                                gameObject.GetComponent<SolverHandler>().TransformOverride = targetObject.transform;
                                gameObject.GetComponent<Orbital>().enabled = true;
                                gameObject.GetComponent<HandConstraint>().enabled = false;
                                gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
                            }
                        }
                        //If targetDistance < minFloatingDistance, the menu should remain compact.
                    }
                    
                }
            }
        }
        //Get the bounding box which contains all renderers or colliders of the object depending on "boundingBoxType"


        /// <summary>
        /// Check Occlusion using raycast
        /// We don't need to check occlusion for main menu because of the SurfaceMagnetism solver and all menus with HandConstraint solver activated.
        /// </summary>
        private void CheckOcclusion() {
            if (!isMainMenu) {
                float targetDistance = (head.transform.position - targetObject.transform.position).magnitude;
                Ray ray = new Ray(head.transform.position, gameObject.transform.position - head.transform.position);
                float headMenuDistance = (gameObject.transform.position - head.transform.position).magnitude;
                if (!isCompact) {      
                    //InBetween or Orbital activated
                    if(Physics.Raycast(ray, headMenuDistance, LayerMask.GetMask("Spatial Mapping"))){
                        message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                        switchTo = message.switchType;
                        Debug.Log("Occlusion Detected for Floating menu");
                        /* if (!gameObject.GetComponent<FinalPlacementOptimizer>().TryToAvoidOcclusion) {
                        gameObject.GetComponent<FinalPlacementOptimizer>().TryToAvoidOcclusion = true;
                    }
                    else {
                        gameObject.GetComponent<FinalPlacementOptimizer>().TryToAvoidOcclusion = false;
                        message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                        switchTo = message.switchType;
                    }*/

                    }
                    /*else {
                        //Check if there is no occlusion anymore
                        if (targetDistance > maxFloatingDistance) {
                            //For InBetween
                            Vector3 inBetweenPostion = placementManager.GetInBetweenTarget().transform.position + Vector3.Normalize(targetObject.transform.position - placementManager.GetInBetweenTarget().transform.position) * defaultFloatingDistance;
                            if (!Physics.Raycast(head.transform.position, inBetweenPostion - head.transform.position, (inBetweenPostion - head.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                                gameObject.GetComponent<FinalPlacementOptimizer>().TryToAvoidOcclusion = false;
                            }
                        }
                        else {
                            if(!Physics.Raycast(head.transform.position, gameObject.GetComponent<FinalPlacementOptimizer>().LastPosition-head.transform.position,
                                (gameObject.GetComponent<FinalPlacementOptimizer>().LastPosition-head.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                                gameObject.GetComponent<FinalPlacementOptimizer>().TryToAvoidOcclusion = false;
                            }
                        }
                    }*/
                }
                else {
                    if(Physics.Raycast(ray, headMenuDistance, LayerMask.GetMask("Spatial Mapping"))) {
                        Debug.Log("Occlusion Detected for Compact Menu");
                        //If orbital activated，turn on the HandConstraint solver, else we do not need to do anything.
                        if (!gameObject.GetComponent<HandConstraint>().enabled) {
                            gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                            gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                            gameObject.GetComponent<HandConstraint>().enabled = true;
                            gameObject.GetComponent<FinalPlacementOptimizer>().enabled = false;
                            gameObject.GetComponent<Orbital>().enabled = false;
                        }
                    }
                    //Check if there is no occlusion anymore (and have space)
                    if (targetDistance > maxFloatingDistance) {
                        //For InBetween
                        Vector3 inBetweenPostion = placementManager.GetInBetweenTarget().transform.position + Vector3.Normalize(targetObject.transform.position - placementManager.GetInBetweenTarget().transform.position) * defaultFloatingDistance;
                        if (Physics.Raycast(head.transform.position, inBetweenPostion - head.transform.position, (inBetweenPostion - head.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                            message.switchType = PlacementMessage.SwitchType.NoSwitch;
                            switchTo = message.switchType;
                        }
                    }
                    if(targetDistance <= maxFloatingDistance && targetDistance >= minFloatingDistance) {
                        Vector3 directionToHead = head.transform.position - targetObject.transform.position;
                        bool rightSide = Vector3.Dot(directionToHead, head.transform.right) > 0 ? true : false;
                        Vector3 centerFloating = Vector3.zero;
                        if (rightSide) {
                            centerFloating = targetObject.transform.position + head.transform.right * orbitalOffsetOppositeType.x + head.transform.up * orbitalOffsetOppositeType.y + head.transform.forward * orbitalOffsetOppositeType.z;
                        }
                        else {
                            centerFloating = targetObject.transform.position + (-head.transform.right * orbitalOffsetOppositeType.x) + head.transform.up * orbitalOffsetOppositeType.y + head.transform.forward * orbitalOffsetOppositeType.z;
                        }
                        if (Physics.Raycast(head.transform.position, centerFloating - head.transform.position, (centerFloating - head.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                            message.switchType = PlacementMessage.SwitchType.NoSwitch;
                            switchTo = message.switchType;
                        }
                        
                    }
                }
                
            }

        }

        private Bounds GetBoundingBox() {
            List<Bounds> allBounds = new List<Bounds>();
            switch (boudingBoxType) {
                case BoundsType.BasedOnColliders:
                    foreach (Collider c in gameObject.GetComponentsInChildren<Collider>()) {
                        allBounds.Add(c.bounds);
                    }
                    break;
                case BoundsType.BasedOnRenderers:
                    foreach (Renderer r in gameObject.GetComponentsInChildren<Renderer>()) {
                        allBounds.Add(r.bounds);
                    }
                    break;
                default:
                    foreach (Collider c in gameObject.GetComponentsInChildren<Collider>()) {
                        allBounds.Add(c.bounds);
                    }
                    break;
            }
            Vector3 maxPoint = Vector3.negativeInfinity;
            Vector3 minPoint = Vector3.positiveInfinity;
            foreach (Bounds b in allBounds) {
                if (b.max.x > maxPoint.x) {
                    maxPoint.x = b.max.x;
                }
                if (b.max.y > maxPoint.y) {
                    maxPoint.y = b.max.y;
                }
                if (b.max.z > maxPoint.z) {
                    maxPoint.z = b.max.z;
                }
                if (b.min.x < minPoint.x) {
                    minPoint.x = b.min.x;
                }
                if (b.min.y < minPoint.y) {
                    minPoint.y = b.min.y;
                }
                if (b.min.z < minPoint.z) {
                    minPoint.z = b.min.z;
                }
            }
            Vector3 size = new Vector3(maxPoint.x - minPoint.x, maxPoint.y - minPoint.y, maxPoint.z - minPoint.z);
            Vector3 center = new Vector3((minPoint.x + maxPoint.x) / 2, (minPoint.y + maxPoint.y) / 2, (minPoint.z + maxPoint.z) / 2);
            return new Bounds(center, size);

        }

        //Only use these two functions for object menus, because for main menus we have the SurfaceMagnetismSafetyOffset
        private bool CollideWithSpatialMappingOppositeType() {
            if (!isMainMenu) {
                return Physics.CheckBox(placementManager.GetStoredBoundingBoxOnCloseOppositeType(gameObject).center, placementManager.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"));
            }
            else {
                return false;
            }        
        }

        private bool CollideWithSpatialMappingSelf() {
            if (!isMainMenu) {
                Vector3 center = targetObject.transform.position + head.transform.right * orbitalOffsetOppositeType.x + head.transform.up * orbitalOffsetOppositeType.y + head.transform.forward * orbitalOffsetOppositeType.z;
                return Physics.CheckBox(GetBoundingBox().center, GetBoundingBox().extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"));
            }
            else {
                return false;
            }
        }

        #endregion Private Methods

    }
}
