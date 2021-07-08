using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using ExitGames.Client.Photon.StructWrapping;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.Utilities;
using i5.VIAProMa.UI.AppBar;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
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
        [Tooltip("The time between two placement updates")]
        [SerializeField] private float updateTimeInterval = 0.1f;
        [Tooltip("The time threshold for inactivity detection (in seconds).")]
        [SerializeField] private float inactivityTimeThreshold = 5;
        [Tooltip("The least time between two suggestion dialogs in manual mode, if any")]
        [SerializeField] private float suggestionTimeInterval = 5f;
        [SerializeField] private bool inactivityDetectionEnabled = true;
        [Tooltip("If enabled, an App Bar will be instantiate for this menu object on start. For better performance and avoiding potential problems, it is better to add a BoxCollider and a BoundingBox script manually.")]
        [SerializeField] private bool manipulationEnabled = false;
        [Tooltip("The numbers of manipulation results stored for retrieving")]
        [SerializeField] private int retrieveBufferSize = 5;

        [Header("Offsets")]
        [Tooltip("The max and min floating distance should be indentical for one menu variant (floating and compact)")]
        [SerializeField] private float maxFloatingDistance = 0.6f;
        [SerializeField] private float minFloatingDistance = 0.3f;
        [Tooltip("Position Offsets To The Target Object. For object menus, the additive inverse of the X (right) offset will be taken if the menu is on the left side of the targetObjects (e.g. 2 to -2).")]
        [SerializeField] private Vector3 orbitalOffset = Vector3.zero;
        [Tooltip("Offsets for the distance between the surface and the attached menu, i.e. the 'Surface Normal Offset' of the SurfaceMagnetism solver")]
        [SerializeField] private float surfaceMagnetismSafetyOffset = 0.05f;



        #endregion Serialize Fields

        private MenuPlacementService placementService;
        private PlacementMessage message = new PlacementMessage();
        private PlacementMessage.SwitchType switchTo = PlacementMessage.SwitchType.NoSwitch;
        private Camera head;
        private float defaultFloatingDistance;
        private Vector3 orbitalOffsetOppositeType;
        private float inactivityTime = 0;
        private float updateTime = 0;
        private float suggestionTime = 0;
        private GameObject appBar;
        //Tupel<Position, Rotaion, Scale>
        private List<Tuple<Vector3, Quaternion, Vector3>> retrieveBufferManualMode = new List<Tuple<Vector3, Quaternion, Vector3>>();
        private List<Tuple<Vector3, Quaternion, Vector3>> retrieveBufferAutomaticMode = new List<Tuple<Vector3, Quaternion, Vector3>>();
        private Vector3 manualModePositionOffset;
        private Vector3 manualModeRotationOffset;
        private Vector3 manualModeScaleOffset;
        private Vector3 originalScale;
        private bool headReferenced = true;
        
        private bool manualModeEntered = false;
        public GameObject TargetObject { get; private set; }
        public Vector3 OrbitalOffset
        {
            get => orbitalOffset;
            set
            {
                orbitalOffset = value;
            }
        }

        public bool HeadReferenced 
        {
            get => headReferenced;
            set
            {
                headReferenced = value;
            }
        }

        #region MonoBehaviour Functions

        // Start is called before the first frame update
        void Start() {
            defaultFloatingDistance = (minFloatingDistance + maxFloatingDistance) / 2;
            placementService = ServiceManager.GetService<MenuPlacementService>();
            head = CameraCache.Main;
            if (isMainMenu) {
                InitializeMainMenu();
            }
            retrieveBufferManualMode.Capacity = retrieveBufferSize;
            retrieveBufferAutomaticMode.Capacity = retrieveBufferSize;
            originalScale = gameObject.transform.localScale;
            manualModePositionOffset = new Vector3(0, 0, defaultFloatingDistance);
            manualModeRotationOffset = Vector3.zero;
            manualModeScaleOffset = Vector3.one;
        }

        // Update is called once per frame
        void Update() {
            if (placementService.PlacementMode == MenuPlacementService.MenuPlacementServiceMode.Automatic) {
                if (manualModeEntered) {
                    ExitManualMode();
                }
                if (updateTime > updateTimeInterval) {                
                    gameObject.GetComponent<SolverHandler>().enabled = true;
                    CheckSpatialMapping();
                    CheckOcclusion();
                    if (switchTo != PlacementMessage.SwitchType.NoSwitch) {
                        Debug.Log(message.switchType);
                        placementService.UpdatePlacement(message, gameObject);
                        switchTo = PlacementMessage.SwitchType.NoSwitch;
                    }
                    updateTime = 0;
                }
            }else if(placementService.PlacementMode == MenuPlacementService.MenuPlacementServiceMode.Manual){
                gameObject.GetComponent<SolverHandler>().enabled = false;
                if (!manualModeEntered) {
                    EnterManualMode();
                }
                /*gameObject.transform.position = TargetObject.transform.position + manualModePositionOffset;
                gameObject.transform.eulerAngles = TargetObject.transform.eulerAngles + manualModeRotationOffset;
                gameObject.transform.localScale = gameObject.transform.localScale = new Vector3(originalScale.x * manualModeScaleOffset.x, originalScale.y * manualModeScaleOffset.y, originalScale.z * manualModeScaleOffset.z);*/
                ShowSuggestion();
            }
            else {
                gameObject.GetComponent<SolverHandler>().enabled = false;
            }
        }


        private void FixedUpdate() {
            if(placementService.PlacementMode == MenuPlacementService.MenuPlacementServiceMode.Automatic) {
                if (inactivityDetectionEnabled) {
                    CheckInactivity();
                }
                if (updateTime <= updateTimeInterval) {
                    updateTime += Time.deltaTime;
                }
            }
            if(placementService.PlacementMode == MenuPlacementService.MenuPlacementServiceMode.Manual) {
                if (suggestionTime <= suggestionTimeInterval) {
                    suggestionTime += Time.deltaTime;
                }
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

        #endregion MonoBehaviour Functions

        #region Public Methods
        /// <summary>
        /// Open the menu with the target object.
        /// </summary>
        /// <param name="targetObject">The object controlled by this menu</param>
        public void Open(GameObject targetObject) {
            //Find all menus in the scene. If there is already one menu for the targetObject, no menu will be opened.
            MenuHandler [] menus = FindObjectsOfType<MenuHandler>();
            foreach(MenuHandler m in menus) {
                if (isMainMenu) {
                    if(m.menuID == menuID) {
                        return;
                    }
                }
                else {
                    if (m.menuID == menuID && m.TargetObject == targetObject) {
                        return;
                    }
                }
            }
            if (placementService == null) {
                placementService = ServiceManager.GetService<MenuPlacementService>();
            }
            GameObject menu = placementService.OpenMenu(gameObject);
            menu.GetComponent<MenuHandler>().orbitalOffsetOppositeType = placementService.GetOrbitalOffsetOppositeType(menu);
            menu.GetComponent<MenuHandler>().head = CameraCache.Main;
            if (manipulationEnabled) {
                menu.GetComponent<MenuHandler>().InitializeAppBar();
            }

            //Initialize the solvers for object menus.
            if (!isMainMenu) {
                //To avoid collision at the beginning, or the menu can switch between two variants all the time.
                menu.transform.position = placementService.GetInBetweenTarget().transform.position;
                menu.GetComponent<MenuHandler>().TargetObject = targetObject;
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
                    menu.GetComponent<InBetween>().SecondTransformOverride = placementService.GetInBetweenTarget().transform;
                    menu.GetComponent<InBetween>().PartwayOffset = (defaultFloatingDistance / targetDistance);
                    (menu.GetComponent<FinalPlacementOptimizer>() ?? menu.AddComponent<FinalPlacementOptimizer>()).OrbitalOffset = orbitalOffset;
                    menu.GetComponent<FinalPlacementOptimizer>().OrientationType = menuOrientationType;
                    menu.GetComponent<FinalPlacementOptimizer>().enabled = true;
                    menu.GetComponent<FinalPlacementOptimizer>().OriginalScale = gameObject.transform.localScale;
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
                    menu.GetComponent<HandConstraint>().UpdateLinkedTransform = true;
                    menu.GetComponent<HandConstraint>().UpdateWhenOppositeHandNear = true;
                    (menu.GetComponent<FinalPlacementOptimizer>() ?? menu.AddComponent<FinalPlacementOptimizer>()).OrbitalOffset = orbitalOffset;
                    menu.GetComponent<FinalPlacementOptimizer>().OrientationType = menuOrientationType;
                    menu.GetComponent<FinalPlacementOptimizer>().enabled = true;
                    menu.GetComponent<FinalPlacementOptimizer>().OriginalScale = gameObject.transform.localScale;
                    if (targetDistance > maxFloatingDistance) {
                        menu.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                        menu.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                        menu.GetComponent<HandConstraint>().enabled = true;
                        menu.GetComponent<Orbital>().enabled = false;
                    }
                    else {
                        menu.GetComponent<HandConstraint>().enabled = false;
                    }                    
                }
            }
            menu.SetActive(true);
            menu.GetComponent<MenuBase>().Initialize();
            if(placementService.PlacementMode == MenuPlacementService.MenuPlacementServiceMode.Manual) {
                menu.GetComponent<MenuHandler>().EnterManualMode();
            }
        }

        /// <summary>
        /// Close the menu object.
        /// </summary>
        public void Close() {
            placementService.StoreBoundingBoxOnClose(gameObject, GetBoundingBox());
            placementService.CloseMenu(gameObject);
            gameObject.GetComponent<MenuBase>().OnClose();
            if (manualModeEntered) {
                ExitManualMode();
            }
            gameObject.SetActive(false);
            if (appBar) {
                appBar.SetActive(false);
            }
        }

        /// <summary>
        /// Save the old offsets of the menu object for later retrieve in the order of position, rotation, and scale.
        /// </summary>
        public void SaveOffsetBeforeManipulation(Tuple<Vector3, Quaternion, Vector3> newTransform, Tuple<Vector3, Quaternion, Vector3> oldTransform)
        {      
            if (placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Automatic) {
                Vector3 scaleOffset = new Vector3(oldTransform.Item3.x / originalScale.x, oldTransform.Item3.y / originalScale.y, oldTransform.Item3.z / originalScale.z);
                if (retrieveBufferAutomaticMode.Count == retrieveBufferAutomaticMode.Capacity) {
                    retrieveBufferAutomaticMode.RemoveAt(0);
                }
                Tuple<Vector3, Quaternion, Vector3> oldOffset = new Tuple<Vector3, Quaternion, Vector3>(gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset, Quaternion.Euler(gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset), scaleOffset);
                retrieveBufferAutomaticMode.Add(oldOffset);
                gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset += newTransform.Item1 - oldTransform.Item1;
                gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset += newTransform.Item2.eulerAngles - oldTransform.Item2.eulerAngles;
                gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset = new Vector3(newTransform.Item3.x / originalScale.x, newTransform.Item3.y / originalScale.y, newTransform.Item3.z / originalScale.z);
            }
            else {
                Vector3 scaleOffset = new Vector3(oldTransform.Item3.x / originalScale.x, oldTransform.Item3.y / originalScale.y, oldTransform.Item3.z / originalScale.z);
                //Debug.Log("Capacity: " + retrieveBufferManualMode.Capacity);
                if (retrieveBufferManualMode.Count == retrieveBufferManualMode.Capacity) {
                    retrieveBufferManualMode.RemoveAt(0);
                }
                Tuple<Vector3, Quaternion, Vector3> oldOffset = new Tuple<Vector3, Quaternion, Vector3>(oldTransform.Item1, oldTransform.Item2, scaleOffset);
                retrieveBufferManualMode.Add(oldOffset);
                //Debug.Log("Count after add: " + retrieveBufferManualMode.Count);
            }
        }

        /// <summary>
        /// Retrieve the last manipulation of the menu object.
        /// </summary>
        public void Retrieve() {
            if(placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Automatic){
                if (retrieveBufferAutomaticMode.Count > 0) {
                    Tuple<Vector3, Quaternion, Vector3> oldOffset = retrieveBufferAutomaticMode.Last();
                    gameObject.transform.localPosition = gameObject.transform.localPosition - gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset + oldOffset.Item1;
                    gameObject.transform.localEulerAngles = gameObject.transform.localEulerAngles - gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset + oldOffset.Item2.eulerAngles;
                    gameObject.transform.localScale = new Vector3(oldOffset.Item3.x * originalScale.x, oldOffset.Item3.y * originalScale.y, oldOffset.Item3.z * originalScale.z);
                    gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset = oldOffset.Item1;
                    gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset = oldOffset.Item2.eulerAngles;
                    gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset = oldOffset.Item3;                   
                    retrieveBufferAutomaticMode.RemoveAt(retrieveBufferAutomaticMode.Count - 1);
                }
            }
            else {
                if (retrieveBufferManualMode.Count > 0) {
                    Tuple<Vector3, Quaternion, Vector3> oldOffset = retrieveBufferManualMode.Last();
                    gameObject.transform.localPosition = oldOffset.Item1;
                    gameObject.transform.localRotation = oldOffset.Item2;
                    gameObject.transform.localScale = new Vector3(oldOffset.Item3.x * originalScale.x, oldOffset.Item3.y * originalScale.y, oldOffset.Item3.z * originalScale.z);
                    retrieveBufferManualMode.RemoveAt(retrieveBufferManualMode.Count - 1);
                }
            }

        }

        public void UpdateOffset(Tuple<Vector3, Quaternion, Vector3> newTransform, Tuple<Vector3, Quaternion, Vector3> oldTransform) {
            if(placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Automatic) {

            }
            else {
                manualModePositionOffset = newTransform.Item1;
                manualModeRotationOffset = newTransform.Item2.eulerAngles;
                manualModeScaleOffset = new Vector3(newTransform.Item3.x / originalScale.x, newTransform.Item3.y / originalScale.y, newTransform.Item3.z / originalScale.z);
            }
        }

        public void EnterManualMode() {
            gameObject.transform.parent = head.transform;
            gameObject.transform.localPosition = manualModePositionOffset;
            gameObject.transform.localScale = new Vector3(originalScale.x * manualModeScaleOffset.x, originalScale.y * manualModeScaleOffset.y, originalScale.z * manualModeScaleOffset.z);
            gameObject.transform.localEulerAngles = manualModeRotationOffset;
            manualModeEntered = true;
        }

        public void ExitManualMode() {
            gameObject.transform.parent = null;
            gameObject.transform.localScale = originalScale;
            manualModeEntered = false;
        }

        #endregion Public Methods

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
                gameObject.GetComponent<Follow>().OrientationType = SolverOrientationType.Unmodified;
                gameObject.GetComponent<Follow>().UpdateLinkedTransform = true;
                gameObject.AddComponent<FinalPlacementOptimizer>().OriginalScale = gameObject.transform.localScale;
                gameObject.GetComponent<FinalPlacementOptimizer>().OrientationType = menuOrientationType;
                gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
                
            }
            else {
                gameObject.AddComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                gameObject.AddComponent<HandConstraint>().SafeZone = HandConstraint.SolverSafeZone.RadialSide;
                gameObject.GetComponent<HandConstraint>().SafeZoneBuffer = 0.05f;
                gameObject.GetComponent<HandConstraint>().UpdateLinkedTransform = true;
                gameObject.GetComponent<HandConstraint>().UpdateWhenOppositeHandNear = true;
                gameObject.AddComponent<FinalPlacementOptimizer>().OriginalScale = gameObject.transform.localScale;
                gameObject.GetComponent<FinalPlacementOptimizer>().OrientationType = menuOrientationType;
                gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
            }
            if (manipulationEnabled) {
                InitializeAppBar();
            }
            gameObject.GetComponent<MenuBase>().Initialize();
        }

        private void InitializeAppBar() {
            if (!gameObject.GetComponent<BoundingBoxStateController>()) {
                gameObject.AddComponent<BoundingBoxStateController>();
            }
            (gameObject.GetComponent<BoundingBox>() ?? gameObject.AddComponent<BoundingBox>()).BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateManually;
            gameObject.GetComponent<BoundingBox>().CalculationMethod = BoundingBox.BoundsCalculationMethod.ColliderOnly;
            gameObject.GetComponent<BoundingBox>().Target = gameObject;
            if (!appBar) {
                appBar = Instantiate(ServiceManager.GetService<MenuPlacementService>().AppBar);
                appBar.GetComponent<AppBarPlacer>().TargetBoundingBox = gameObject.GetComponent<BoundingBox>();
            }
            else {
                appBar.SetActive(true);
            }
        }

        private void CheckSpatialMapping() {
            float distanceToMenu = Vector3.Dot(gameObject.transform.position - head.transform.position, head.transform.forward);
            //Use to check main menu, 2 * surfaceMagnetismSafetyOffset for better accuracy
            bool closeToSpatialMapping = Physics.Raycast(head.transform.position, head.transform.forward, minFloatingDistance + 2 * surfaceMagnetismSafetyOffset, LayerMask.GetMask("Spatial Mapping"));
            //Debug.Log(distanceToMenu);
            //Debug.Log(closeToSpatialMapping);
            //Debug.Log("Collide with Spatial Mapping: " + CollideWithSpatialMapping());
            if (CollideWithSpatialMapping() || (distanceToMenu < minFloatingDistance + surfaceMagnetismSafetyOffset && closeToSpatialMapping)) {
                if (isMainMenu) {
                    if (!isCompact) {
                        
                        // Cast a ray towards the position of the menu from the camera, if the menu is closer than the minFloatingDistance, switch to the compact version.
                        Ray ray = new Ray(head.transform.position, GetBoundingBox().center - head.transform.position);
                        Debug.Log(Physics.Raycast(ray, minFloatingDistance, LayerMask.GetMask("Menu")));
                        if (Physics.Raycast(ray, minFloatingDistance, LayerMask.GetMask("Menu"))) {
                            message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                            switchTo = message.switchType;
                        }
                    }
                    //Compact menus don't need to be handled here.
                }
                else {
                    float targetDistance = (head.transform.position - TargetObject.transform.position).magnitude;
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
                        gameObject.GetComponent<Orbital>().enabled = false;
                    }
                    
                }
            }
            else {
                if (isMainMenu) {
                    if (isCompact) {
                        //check the bounding box of the floating menu from GetBoundingBox() in front of the user's head.
                        Vector3 center = head.transform.position + head.transform.forward * minFloatingDistance;
                        if (!Physics.CheckBox(center, placementService.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))) {
                            Debug.Log("Front Free for Floating main menu");
                            message.switchType = PlacementMessage.SwitchType.CompactToFloating;
                            switchTo = message.switchType;
                        }
                    }
                    //Floating menus don't need to be handled here.
                }
                else {
                    float targetDistance = (head.transform.position - TargetObject.transform.position).magnitude;
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
                            Vector3 center = placementService.GetInBetweenTarget().transform.position + Vector3.Normalize(TargetObject.transform.position - placementService.GetInBetweenTarget().transform.position) * defaultFloatingDistance;
                            if (!Physics.CheckBox(center, placementService.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))) {
                                Debug.Log("Front Free for InBetween");
                                message.switchType = PlacementMessage.SwitchType.CompactToFloating;
                                switchTo = message.switchType;
                            }
                            else {
                                gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                                gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                                gameObject.GetComponent<HandConstraint>().enabled = true;
                                gameObject.GetComponent<Orbital>().enabled = false;
                            }                 
                        }                       
                        if (targetDistance >= minFloatingDistance && targetDistance <= maxFloatingDistance) {
                            //Orbital and Beside solver should be activated on the floating menu.
                            //Check the space "besides" the targetObject on the left or right side according to user's position, similar to Beside solver.
                            Vector3 directionToHead = head.transform.position - TargetObject.transform.position;
                            bool rightSide = Vector3.Dot(directionToHead, head.transform.right) > 0 ? true : false;
                            Vector3 centerFloating = Vector3.zero;
                            Vector3 centerCompact = Vector3.zero;
                            if (rightSide) {
                                centerFloating = TargetObject.transform.position + head.transform.right * orbitalOffsetOppositeType.x + head.transform.up * orbitalOffsetOppositeType.y + head.transform.forward * orbitalOffsetOppositeType.z;
                                centerCompact = TargetObject.transform.position + head.transform.right * orbitalOffset.x + head.transform.up * orbitalOffset.y + head.transform.forward * orbitalOffset.z;                               
                            }
                            else {
                                centerFloating = TargetObject.transform.position + ( - head.transform.right * orbitalOffsetOppositeType.x) + head.transform.up * orbitalOffsetOppositeType.y + head.transform.forward * orbitalOffsetOppositeType.z;
                                centerCompact = TargetObject.transform.position + ( - head.transform.right * orbitalOffset.x) + head.transform.up * orbitalOffset.y + head.transform.forward * orbitalOffset.z;
                            }
                            if (!Physics.CheckBox(centerFloating, placementService.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))
                                && !Physics.Raycast(TargetObject.transform.position, centerFloating - TargetObject.transform.position, (centerFloating - TargetObject.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                                Debug.Log("Besides Free for Floating Orbital");
                                message.switchType = PlacementMessage.SwitchType.CompactToFloating;
                                switchTo = message.switchType;
                            }else if (!Physics.CheckBox(centerCompact, placementService.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))
                                && !Physics.Raycast(TargetObject.transform.position, centerCompact - TargetObject.transform.position, (centerCompact - TargetObject.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {                       
                                Debug.Log("Beside Free for Compact Orbital");
                                gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.CustomOverride;
                                gameObject.GetComponent<SolverHandler>().TransformOverride = TargetObject.transform;
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

        /// <summary>
        /// Check Occlusion using raycast. 
        /// We don't need to check occlusion for main menu because of the SurfaceMagnetism solver and all menus with HandConstraint solver activated.
        /// </summary>
        private void CheckOcclusion() {
            if (!isMainMenu) {
                float targetDistance = (head.transform.position - TargetObject.transform.position).magnitude;
                Ray ray = new Ray(head.transform.position, gameObject.transform.position - head.transform.position);
                float headMenuDistance = (gameObject.transform.position - head.transform.position).magnitude;
                if (!isCompact) {      
                    //InBetween or Orbital activated
                    if(Physics.Raycast(ray, headMenuDistance, LayerMask.GetMask("Spatial Mapping"))){
                        message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                        switchTo = message.switchType;
                        Debug.Log("Occlusion Detected for Floating menu");
                    }
                }
                else {
                    if(Physics.Raycast(ray, headMenuDistance, LayerMask.GetMask("Spatial Mapping"))) {
                        Debug.Log("Occlusion Detected for Compact Menu");
                        message.switchType = PlacementMessage.SwitchType.NoSwitch;
                        switchTo = message.switchType;
                        //If orbital activated，turn on the HandConstraint solver, else we do not need to do anything.
                        if (!gameObject.GetComponent<HandConstraint>().enabled) {
                            gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                            gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                            gameObject.GetComponent<HandConstraint>().enabled = true;
                            gameObject.GetComponent<Orbital>().enabled = false;
                        }
                    }
                    //Check if there is no occlusion anymore (and have space)
                    if (targetDistance > maxFloatingDistance) {
                        //For InBetween
                        Vector3 inBetweenPostion = placementService.GetInBetweenTarget().transform.position + Vector3.Normalize(TargetObject.transform.position - placementService.GetInBetweenTarget().transform.position) * defaultFloatingDistance;
                        if (Physics.Raycast(head.transform.position, inBetweenPostion - head.transform.position, (inBetweenPostion - head.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                            Debug.Log("Occlusion detected if switch to floating menu InBetween");
                            message.switchType = PlacementMessage.SwitchType.NoSwitch;
                            switchTo = message.switchType;
                        }
                    }
                    if(targetDistance <= maxFloatingDistance && targetDistance >= minFloatingDistance) {
                        Vector3 directionToHead = head.transform.position - TargetObject.transform.position;
                        bool rightSide = Vector3.Dot(directionToHead, head.transform.right) > 0 ? true : false;
                        Vector3 centerFloating = Vector3.zero;
                        if (rightSide) {
                            centerFloating = TargetObject.transform.position + head.transform.right * orbitalOffsetOppositeType.x + head.transform.up * orbitalOffsetOppositeType.y + head.transform.forward * orbitalOffsetOppositeType.z;
                        }
                        else {
                            centerFloating = TargetObject.transform.position + (-head.transform.right * orbitalOffsetOppositeType.x) + head.transform.up * orbitalOffsetOppositeType.y + head.transform.forward * orbitalOffsetOppositeType.z;
                        }
                        if (Physics.Raycast(head.transform.position, centerFloating - head.transform.position, (centerFloating - head.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                            Debug.Log("Occlusion detected if switch to floating menu Orbital");
                            message.switchType = PlacementMessage.SwitchType.NoSwitch;
                            switchTo = message.switchType;
                        }
                        
                    }
                }
                
            }

        }

        /// <summary>
        /// Provide suggestions to the user in manual mode.
        /// </summary>
        private void ShowSuggestion() {
            if (CollideWithSpatialMapping()) {
                if (isMainMenu) {
                    if (suggestionTime > suggestionTimeInterval) {
                        placementService.EnterAdjustmentMode();
                        Component suggestionPanel = Dialog.Open(placementService.SuggestionPanel, DialogButtonType.Close | DialogButtonType.Accept , "Menu Needs Adjustment",
                                    "Collision Detected for the main menu! You might need to move to another location, move it closer or switch it to compact version if possible. You can click 'Accept' to switch to automatic mode", true);
                        suggestionPanel.gameObject.GetComponent<Follow>().MinDistance = 0.2f;
                        suggestionPanel.gameObject.GetComponent<Follow>().MaxDistance = 0.2f;
                        suggestionPanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.2f;
                        suggestionTime = 0;
                    }
                }
                else {
                    float targetDistance = (head.transform.position - TargetObject.transform.position).magnitude;
                    //If the target object is far away, namely the InBetween solver is activated.
                    if (targetDistance > maxFloatingDistance) {
                    }
                    if (suggestionTime > suggestionTimeInterval) {
                        placementService.EnterAdjustmentMode();
                        Component suggestionPanel = Dialog.Open(placementService.SuggestionPanel, DialogButtonType.Close | DialogButtonType.Accept, "Menu Needs Adjustment",
                                    "Collision Detected for an object menu! You might need to move to another location, move it closer or switch it to compact version if possible. You can click 'Accept' to switch to automatic mode", true);
                        suggestionPanel.gameObject.GetComponent<Follow>().MinDistance = 0.2f;
                        suggestionPanel.gameObject.GetComponent<Follow>().MaxDistance = 0.2f;
                        suggestionPanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.2f;
                        suggestionTime = 0;
                    }
                }
            }
            //Check occlusion
            else {
                Ray ray = new Ray(head.transform.position, gameObject.transform.position - head.transform.position);
                float headMenuDistance = (gameObject.transform.position - head.transform.position).magnitude;
                if (isMainMenu) {
                    if (Physics.Raycast(ray, headMenuDistance, LayerMask.GetMask("Spatial Mapping"))) {
                        if (suggestionTime > suggestionTimeInterval) {
                            placementService.EnterAdjustmentMode();
                            Component suggestionPanel = Dialog.Open(placementService.SuggestionPanel, DialogButtonType.Close | DialogButtonType.Accept, "Menu Needs Adjustment",
                                        "Occlusion Detected for the main menu! You might need to move to another location, move it closer or switch it to compact version if possible. You can click 'Accept' to switch to automatic mode", true);
                            suggestionPanel.gameObject.GetComponent<Follow>().MinDistance = 0.2f;
                            suggestionPanel.gameObject.GetComponent<Follow>().MaxDistance = 0.2f;
                            suggestionPanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.2f;
                            suggestionTime = 0;
                        }
                    }
                }
                else {
                    if (Physics.Raycast(ray, headMenuDistance, LayerMask.GetMask("Spatial Mapping"))) {
                        if (suggestionTime > suggestionTimeInterval) {
                            placementService.EnterAdjustmentMode();
                            Component suggestionPanel = Dialog.Open(placementService.SuggestionPanel, DialogButtonType.Close | DialogButtonType.Accept, "Menu Needs Adjustment",
                                        "Occlusion Detected for an object menu! You might need to move to another location, move it closer or switch it to compact version if possible. You can click 'Accept' to switch to automatic mode", true);
                            suggestionPanel.gameObject.GetComponent<Follow>().MinDistance = 0.2f;
                            suggestionPanel.gameObject.GetComponent<Follow>().MaxDistance = 0.2f;
                            suggestionPanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.2f;
                            suggestionTime = 0;
                        }
                    }
                    
                }
            }

        }

        private void CheckInactivity() {
            GameObject gazeTarget = CoreServices.InputSystem.GazeProvider.GazeTarget;
            if (gazeTarget != null) {
                Debug.Log(gazeTarget.transform.IsChildOf(gameObject.transform));
                if (gazeTarget.transform.IsChildOf(gameObject.transform)) {
                    inactivityTime = 0;
                }
                else {
                    inactivityTime += Time.deltaTime;
                }              
            }
            else {
                inactivityTime += Time.deltaTime;
            }
            if (inactivityTime > inactivityTimeThreshold) {
                inactivityTime = 0;
                Close();
            }
        }

        /// <summary>
        /// //Get the bounding box which contains all renderers or colliders of the object depending on "boundingBoxType"
        /// </summary>
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

        //Only this functions for object menus, because for main menus we have the SurfaceMagnetismSafetyOffset
        private bool CollideWithSpatialMapping() {
            if (!isMainMenu) {                
                return Physics.CheckBox(GetBoundingBox().center, GetBoundingBox().extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"));
            }
            else {
                return false;
            }
        }

        #endregion Private Methods

    }
}
