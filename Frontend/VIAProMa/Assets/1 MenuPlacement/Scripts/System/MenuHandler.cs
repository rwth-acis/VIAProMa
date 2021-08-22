using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.UI.AppBar;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Photon.Pun.Demo.Cockpit.Forms;
using UnityEngine;

namespace MenuPlacement {
    public class MenuHandler : MonoBehaviour {

        public enum BoundsType {
            BasedOnColliders,
            BasedOnRenderers
        }

        public enum MenuOrientationType {
            CameraAligned
        }

        public enum MenuVariantType {
            MainMenu,
            ObjectMenu
        }

        #region Serialize Fields
        [Header("General Properties")]
        [Tooltip("True, if the menu is an object menu. False, if the menu is a main menu")]
        public MenuVariantType menuVariantType;
        [Tooltip("Is the menu a normal one or a compact one?")]
        public bool compact;
        [Tooltip("If enabled, the menu will be closed automatically if it is not in the users' head gaze for a while. The time threshold can be set in 'Inactivity Time Threshold'.")]
        [SerializeField] private bool inactivityDetectionEnabled = true;
        [Tooltip("If enabled, an App Bar will be instantiate for this menu object on start. For better performance and avoiding potential problems, it is better to add a BoxCollider and a BoundingBox script manually.")]
        [SerializeField] private bool manipulationEnabled = true;
        [Tooltip("If enabled, the 'ConstantViewSize' solver will be added to the menu object, and the scale option of manipulation will be deactivated, instead a slider for 'Target View Percent V' of 'ConstantViewSize;")]
        [SerializeField] private bool constantViewSizeEnabled = true;
        [Tooltip("The ID of this menu. Make sure they are different from one to another")]
        public int menuID;
        [Tooltip("The bounding box will be used to decide whether the space is enough to place the menu. It is a 'Bounds' object containing all Bounds of the corresponding base")]
        [SerializeField] private BoundsType boundingBoxType = BoundsType.BasedOnColliders;
        [Tooltip("The orientation type used on the solver attached to the object by the placement system if applicable")]
        [SerializeField] private MenuOrientationType menuOrientationType = MenuOrientationType.CameraAligned;

        [Header("Thresholds")]
        [Tooltip("The time between two placement updates")]
        [SerializeField] private float updateTimeInterval = 0.1f;
        [Tooltip("The time threshold for inactivity detection in seconds.")]
        [SerializeField] private float inactivityTimeThreshold = 10;
        [Tooltip("The least time between two suggestion dialogs in manual mode, if any")]
        [SerializeField] private float suggestionTimeInterval = 10f;
        [Tooltip("The numbers of manipulation results stored for retrieving")]
        [SerializeField] private int retrieveBufferSize = 5;

        [Header("Global Offsets")]
        [Tooltip("The max and min floating distance should be indentical for one menu variant (floating and compact)")]
        [SerializeField] private float maxFloatingDistance = 0.6f;
        [SerializeField] private float minFloatingDistance = 0.3f;
        [Tooltip("The distance when the floating menu is first instantiate. If it is smaller than Min Floating Distance, it will be set to the average of Min and Max Floating Distance")]
        [SerializeField] private float defaultFloatingDistance = 0;

        //Constant View Size Offsets
        [Tooltip("The default Target View Percent V of ConstantViewSize")]
        [Range(0,1)]
        [SerializeField] private float defaultTargetViewPercentV = 0.5f;
        [Tooltip("The 'Min Distance' of ConstantViewSize")]
        [SerializeField] private float minDistance = 0;
        [Tooltip("The 'Max Distance' of ConstantViewSize")]
        [SerializeField] private float maxDistance = 1;


        //Main Menu Offsets
        [Tooltip("Position Offset to the head for floating main menus. It will be directly added to the 'AdditionalOffset' on SolverHandler.")]
        [SerializeField] private Vector3 followOffset = Vector3.zero;
        [Tooltip("Max view degrees in horizontal direction for Follow solver of floating main menus.")]
        [SerializeField] private float followMaxViewHorizontalDegrees = 50f;
        [Tooltip("Max view degrees in vertical direction for Follow solver of floating main menus.")]
        [SerializeField] private float followMaxViewVerticalDegrees = 100f;
        [Tooltip("Offsets for the distance between the surface and the attached menu, i.e. the 'Surface Normal Offset' of the SurfaceMagnetism solver")]
        [SerializeField] private float surfaceMagnetismSafetyOffset = 0.05f;

        //Object Menu Offsets
        [Tooltip("Position Offset to the target object. For object menus, the additive inverse of the X (right) offset will be taken if the menu is on the left side of the targetObjects (e.g. 2 to -2).")]
        [SerializeField] private Vector3 orbitalOffset = Vector3.zero;



        #endregion Serialize Fields

        private MenuPlacementService placementService;
        private PlacementMessage message = new PlacementMessage();
        private PlacementMessage.SwitchType switchTo = PlacementMessage.SwitchType.NoSwitch;
        private Camera head;
        private Vector3 orbitalOffsetOppositeType;
        private float inactivityTime = 0;
        private float updateTime = 0;
        private float suggestionTime = 0;
        //The suggestionPanel will only be displayed if the collision or occlusion exists for 3 seconds.
        private float accumulatedTimeForSuggestionThreshold = 3f;
        private float accumulatedTimeForSuggestion = 0;
        private GameObject appBar;
        //Tupel<Position, Rotaion, Scale>
        private List<Tuple<Vector3, Quaternion, Vector3>> retrieveBufferManualMode = new List<Tuple<Vector3, Quaternion, Vector3>>();
        private List<Tuple<Vector3, Quaternion, Vector3, float>> retrieveBufferAutomaticModeWithoutOrbital = new List<Tuple<Vector3, Quaternion, Vector3, float>>();
        private List<Tuple<Vector3, Quaternion, Vector3, float>> retrieveBufferAutomaticModeOrbital = new List<Tuple<Vector3, Quaternion, Vector3, float>>();
        private Tuple<Vector3, Quaternion, Vector3, float> lastOffsetsInBetween = new Tuple<Vector3, Quaternion, Vector3, float>(Vector3.zero, Quaternion.identity, Vector3.one, 0);
        private Tuple<Vector3, Quaternion, Vector3, float> lastOffsetsOrbital = new Tuple<Vector3, Quaternion, Vector3, float>(Vector3.zero, Quaternion.identity, Vector3.one, 0);
        private Tuple<Vector3, Quaternion, Vector3, float> lastOffsetsHandConstraint = new Tuple<Vector3, Quaternion, Vector3, float>(Vector3.zero, Quaternion.identity, Vector3.one, 0);
        private Vector3 manualModePositionOffset;
        private Vector3 manualModeRotationOffset;
        private Vector3 manualModeScaleOffset;
        private Vector3 originalScale;

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

        public bool ConstantViewSizeEnabled
        {
            get => constantViewSizeEnabled;
        }

        public float MinFloatingDistance
        {
            get => minFloatingDistance;
        }

        public float MaxFloatingDistance
        {
            get => maxFloatingDistance;
        }

        public float DefaultFloatingDistance
        {
            get => defaultFloatingDistance;
        }

        #region MonoBehaviour Functions

        // Start is called before the first frame update
        void Start() {
            if (defaultFloatingDistance < minFloatingDistance || defaultFloatingDistance > maxFloatingDistance) {
                defaultFloatingDistance = (minFloatingDistance + maxFloatingDistance) / 2;
            }

            placementService = ServiceManager.GetService<MenuPlacementService>();
            head = CameraCache.Main;
            if (menuVariantType == MenuVariantType.MainMenu) {
                InitializeMainMenu();
            }
            retrieveBufferManualMode.Capacity = retrieveBufferSize;
            retrieveBufferAutomaticModeWithoutOrbital.Capacity = retrieveBufferSize;
            retrieveBufferAutomaticModeOrbital.Capacity = retrieveBufferSize;
            originalScale = gameObject.transform.localScale;
            manualModePositionOffset = new Vector3(0, -0.05f, defaultFloatingDistance);
            manualModeRotationOffset = Vector3.zero;
            manualModeScaleOffset = Vector3.one;

            //For Evaluation
            sumDistance = 0;
            distanceUpdateCount = 0;
            distanceUpdateTime = 0;
/*            path = Application.persistentDataPath + "/TestData_MenuID_" + menuID + ".txt";
            testData = new FileStream(path, FileMode.Create);
            sw = new StreamWriter(testData);
            sw.WriteLine("-----------------------Test Started-------------------------");
            sw.WriteLine("Start Time: " + DateTime.Now);
            sw.WriteLine("Main Menu: " + isMainMenu);
            sw.WriteLine("Compact: " + isCompact);
            sw.WriteLine("");
            SaveOnUWP();*/
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

            //For Evaluation
            CalculateAverageDistance();
        }
        private void OnDestroy() {
            /*            SaveAverageDistance();
                        SaveLastOffset();*/
            SaveOnUWP();
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
                if (menuVariantType == MenuVariantType.MainMenu) {
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
            menu.GetComponent<MenuHandler>().updateTime = -0.15f;
            if (manipulationEnabled) {
                menu.GetComponent<MenuHandler>().InitializeAppBar();
            }



            //Initialize the solvers for object menus.
            if (menuVariantType == MenuVariantType.ObjectMenu) {
                //Initialization the defaultTargetViewPercentV
                if (menu.GetComponent<MenuHandler>().lastOffsetsHandConstraint.Item4 == 0) {
                    menu.GetComponent<MenuHandler>().lastOffsetsHandConstraint = new Tuple<Vector3, Quaternion, Vector3, float>(Vector3.zero, Quaternion.identity, Vector3.one, defaultTargetViewPercentV);
                }
                if (menu.GetComponent<MenuHandler>().lastOffsetsOrbital.Item4 == 0) {
                    menu.GetComponent<MenuHandler>().lastOffsetsOrbital = new Tuple<Vector3, Quaternion, Vector3, float>(Vector3.zero, Quaternion.identity, Vector3.one, defaultTargetViewPercentV);
                }
                if (menu.GetComponent<MenuHandler>().lastOffsetsInBetween.Item4 == 0) {
                    menu.GetComponent<MenuHandler>().lastOffsetsInBetween = new Tuple<Vector3, Quaternion, Vector3, float>(Vector3.zero, Quaternion.identity, Vector3.one, defaultTargetViewPercentV);
                }
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
                if (!compact) {                 
                    (menu.GetComponent<InBetween>() ?? menu.AddComponent<InBetween>()).SecondTrackedObjectType = TrackedObjectType.CustomOverride;
                    menu.GetComponent<InBetween>().UpdateLinkedTransform = true;
                    menu.GetComponent<InBetween>().SecondTransformOverride = placementService.GetInBetweenTarget().transform;
                    menu.GetComponent<InBetween>().PartwayOffset = (defaultFloatingDistance / targetDistance);
                    (menu.GetComponent<FinalPlacementOptimizer>() ?? menu.AddComponent<FinalPlacementOptimizer>()).OrbitalOffset = orbitalOffset;
                    menu.GetComponent<FinalPlacementOptimizer>().OrientationType = menuOrientationType;
                    menu.GetComponent<FinalPlacementOptimizer>().enabled = true;
                    menu.GetComponent<FinalPlacementOptimizer>().OriginalScale = gameObject.transform.localScale;
                    (menu.GetComponent<ConstantViewSize>() ?? menu.AddComponent<ConstantViewSize>()).MinDistance = minDistance;
                    menu.GetComponent<ConstantViewSize>().MaxDistance = maxDistance;
                    if (targetDistance > maxFloatingDistance) {
                        menu.GetComponent<FinalPlacementOptimizer>().PositionOffset = menu.GetComponent<MenuHandler>().lastOffsetsInBetween.Item1;
                        menu.GetComponent<FinalPlacementOptimizer>().RotationOffset = menu.GetComponent<MenuHandler>().lastOffsetsInBetween.Item2.eulerAngles;
                        menu.GetComponent<FinalPlacementOptimizer>().ScaleOffset = menu.GetComponent<MenuHandler>().lastOffsetsInBetween.Item3;
                        menu.GetComponent<ConstantViewSize>().TargetViewPercentV = menu.GetComponent<MenuHandler>().lastOffsetsInBetween.Item4;
                        menu.GetComponent<Orbital>().enabled = false;
                    }
                    else {
                        menu.GetComponent<FinalPlacementOptimizer>().PositionOffset = menu.GetComponent<MenuHandler>().lastOffsetsOrbital.Item1;
                        menu.GetComponent<FinalPlacementOptimizer>().RotationOffset = menu.GetComponent<MenuHandler>().lastOffsetsOrbital.Item2.eulerAngles;
                        menu.GetComponent<FinalPlacementOptimizer>().ScaleOffset = menu.GetComponent<MenuHandler>().lastOffsetsOrbital.Item3;
                        menu.GetComponent<ConstantViewSize>().TargetViewPercentV = menu.GetComponent<MenuHandler>().lastOffsetsOrbital.Item4;
                        menu.GetComponent<InBetween>().enabled = false;
                    }
                }
                //For compact version, add the HandConstraint solver additionally for interaction in very narror space.
                else {
                    if (placementService.HandTrackingEnabled) {
                        (menu.GetComponent<HandConstraint>() ?? menu.AddComponent<HandConstraint>()).SafeZone = HandConstraint.SolverSafeZone.RadialSide;
                        menu.GetComponent<HandConstraint>().SafeZoneBuffer = 0.05f;
                        menu.GetComponent<HandConstraint>().UpdateLinkedTransform = true;
                        menu.GetComponent<HandConstraint>().UpdateWhenOppositeHandNear = true;
                    }
                    else {
                        (menu.GetComponent<Follow>() ?? menu.AddComponent<Follow>()).MinDistance = 0.3f;
                        menu.GetComponent<Follow>().MaxDistance = 0.3f;
                        menu.GetComponent<Follow>().DefaultDistance = 0.3f;
                        menu.GetComponent<Follow>().MaxViewHorizontalDegrees = 40f;
                        menu.GetComponent<Follow>().MaxViewVerticalDegrees = 40f;
                        menu.GetComponent<Follow>().OrientToControllerDeadzoneDegrees = 20f;
                        menu.GetComponent<Follow>().OrientationType = SolverOrientationType.Unmodified;
                        menu.GetComponent<Follow>().UpdateLinkedTransform = true;
                    }
                    (menu.GetComponent<FinalPlacementOptimizer>() ?? menu.AddComponent<FinalPlacementOptimizer>()).OrbitalOffset = orbitalOffset;
                    menu.GetComponent<FinalPlacementOptimizer>().OrientationType = menuOrientationType;
                    menu.GetComponent<FinalPlacementOptimizer>().enabled = true;
                    menu.GetComponent<FinalPlacementOptimizer>().OriginalScale = gameObject.transform.localScale;
                    (menu.GetComponent<ConstantViewSize>() ?? menu.AddComponent<ConstantViewSize>()).MinDistance = minDistance;
                    menu.GetComponent<ConstantViewSize>().MaxDistance = maxDistance;
                    if (targetDistance > maxFloatingDistance) {
                        menu.GetComponent<FinalPlacementOptimizer>().PositionOffset = menu.GetComponent<MenuHandler>().lastOffsetsHandConstraint.Item1;
                        menu.GetComponent<FinalPlacementOptimizer>().RotationOffset = menu.GetComponent<MenuHandler>().lastOffsetsHandConstraint.Item2.eulerAngles;
                        menu.GetComponent<FinalPlacementOptimizer>().ScaleOffset = menu.GetComponent<MenuHandler>().lastOffsetsHandConstraint.Item3;
                        menu.GetComponent<ConstantViewSize>().TargetViewPercentV = menu.GetComponent<MenuHandler>().lastOffsetsHandConstraint.Item4;
                        if (placementService.HandTrackingEnabled) {
                            if (placementService.ArticulatedHandSupported) {
                                menu.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                                menu.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                            }
                            else {
                                menu.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.ControllerRay;
                                menu.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                            }
                            menu.GetComponent<HandConstraint>().enabled = true;
                        }
                        else {
                            menu.GetComponent<Follow>().enabled = true;
                        }

                        menu.GetComponent<Orbital>().enabled = false;
                    }
                    else {
                        menu.GetComponent<FinalPlacementOptimizer>().PositionOffset = menu.GetComponent<MenuHandler>().lastOffsetsOrbital.Item1;
                        menu.GetComponent<FinalPlacementOptimizer>().RotationOffset = menu.GetComponent<MenuHandler>().lastOffsetsOrbital.Item2.eulerAngles;
                        menu.GetComponent<FinalPlacementOptimizer>().ScaleOffset = menu.GetComponent<MenuHandler>().lastOffsetsOrbital.Item3;
                        menu.GetComponent<ConstantViewSize>().TargetViewPercentV = menu.GetComponent<MenuHandler>().lastOffsetsOrbital.Item4;                       
                        if (placementService.HandTrackingEnabled) {
                            menu.GetComponent<HandConstraint>().enabled = false;
                        }
                        else {
                            menu.GetComponent<Follow>().enabled = false;
                        }
                    }                    
                }

                if (constantViewSizeEnabled) {
                    menu.GetComponent<ConstantViewSize>().enabled = true;
                }
                else {
                    menu.GetComponent<ConstantViewSize>().enabled = false;
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
            if(menuVariantType == MenuVariantType.ObjectMenu) {
                if (!compact) {
                    if (gameObject.GetComponent<Orbital>().enabled) {
                        lastOffsetsOrbital = new Tuple<Vector3, Quaternion, Vector3, float>(gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset, Quaternion.Euler(gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset), gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset, gameObject.GetComponent<ConstantViewSize>().TargetViewPercentV);
                        placementService.StoreOrbitalPositionOffsetOnClose(gameObject, gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset);
                        placementService.StoreInBetweenPositionOffsetOnClose(gameObject, lastOffsetsInBetween.Item1);
                    }
                    else {
                        lastOffsetsInBetween = new Tuple<Vector3, Quaternion, Vector3, float>(gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset, Quaternion.Euler(gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset), gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset, gameObject.GetComponent<ConstantViewSize>().TargetViewPercentV);
                        placementService.StoreInBetweenPositionOffsetOnClose(gameObject, gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset);
                        placementService.StoreOrbitalPositionOffsetOnClose(gameObject, lastOffsetsOrbital.Item1);
                    }
                }
                else {
                    if (gameObject.GetComponent<Orbital>().enabled) {
                        lastOffsetsOrbital = new Tuple<Vector3, Quaternion, Vector3, float>(gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset, Quaternion.Euler(gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset), gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset, gameObject.GetComponent<ConstantViewSize>().TargetViewPercentV);
                        placementService.StoreOrbitalPositionOffsetOnClose(gameObject, gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset);
                    }
                    else {                      
                        lastOffsetsHandConstraint = new Tuple<Vector3, Quaternion, Vector3, float>(gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset, Quaternion.Euler(gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset), gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset, gameObject.GetComponent<ConstantViewSize>().TargetViewPercentV);                       
                    }
                }
            }
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
        public void SaveOffsetBeforeManipulation(Tuple<Vector3, Quaternion, Vector3, float> newOffsets, Tuple<Vector3, Quaternion, Vector3, float> oldOffsets)
        {      
            if (placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Automatic) {
                if (menuVariantType == MenuVariantType.ObjectMenu && gameObject.GetComponent<Orbital>() != null && gameObject.GetComponent<Orbital>().enabled) {
                    Vector3 scaleOffset = new Vector3(oldOffsets.Item3.x / originalScale.x, oldOffsets.Item3.y / originalScale.y, oldOffsets.Item3.z / originalScale.z);
                    if (retrieveBufferAutomaticModeOrbital.Count == retrieveBufferAutomaticModeOrbital.Capacity) {
                        retrieveBufferAutomaticModeOrbital.RemoveAt(0);
                    }
                    Tuple<Vector3, Quaternion, Vector3, float> oldOffset = new Tuple<Vector3, Quaternion, Vector3, float>(gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset, Quaternion.Euler(gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset), scaleOffset, oldOffsets.Item4);
                    retrieveBufferAutomaticModeOrbital.Add(oldOffset);
                    gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset += Camera.main.transform.InverseTransformVector(newOffsets.Item1 - oldOffsets.Item1);
                    gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset = new Vector3(Math.Abs(gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.x), gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.y, gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.z);
                    gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset += newOffsets.Item2.eulerAngles - oldOffsets.Item2.eulerAngles;
                    gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset = new Vector3(newOffsets.Item3.x / originalScale.x, newOffsets.Item3.y / originalScale.y, newOffsets.Item3.z / originalScale.z);
                }
                else {
                    Vector3 scaleOffset = new Vector3(oldOffsets.Item3.x / originalScale.x, oldOffsets.Item3.y / originalScale.y, oldOffsets.Item3.z / originalScale.z);
                    if (retrieveBufferAutomaticModeWithoutOrbital.Count == retrieveBufferAutomaticModeWithoutOrbital.Capacity) {
                        retrieveBufferAutomaticModeWithoutOrbital.RemoveAt(0);
                    }
                    Tuple<Vector3, Quaternion, Vector3, float> oldOffset = new Tuple<Vector3, Quaternion, Vector3, float>(gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset, Quaternion.Euler(gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset), scaleOffset, oldOffsets.Item4);
                    retrieveBufferAutomaticModeWithoutOrbital.Add(oldOffset);
                    gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset += Camera.main.transform.InverseTransformVector(newOffsets.Item1 - oldOffsets.Item1);
                    gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset += newOffsets.Item2.eulerAngles - oldOffsets.Item2.eulerAngles;
                    gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset = new Vector3(newOffsets.Item3.x / originalScale.x, newOffsets.Item3.y / originalScale.y, newOffsets.Item3.z / originalScale.z);
                }

                
            }
            else {
                Vector3 scaleOffset = new Vector3(oldOffsets.Item3.x / originalScale.x, oldOffsets.Item3.y / originalScale.y, oldOffsets.Item3.z / originalScale.z);
                if (retrieveBufferManualMode.Count == retrieveBufferManualMode.Capacity) {
                    retrieveBufferManualMode.RemoveAt(0);
                }
                Tuple<Vector3, Quaternion, Vector3> oldOffset = new Tuple<Vector3, Quaternion, Vector3>(oldOffsets.Item1, oldOffsets.Item2, scaleOffset);
                retrieveBufferManualMode.Add(oldOffset);
            }
        }

        /// <summary>
        /// Retrieve the last manipulation of the menu object.
        /// </summary>
        public void Retrieve() {
            if(placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Automatic){
                if (menuVariantType == MenuVariantType.ObjectMenu && gameObject.GetComponent<Orbital>() != null && gameObject.GetComponent<Orbital>().enabled) {
                    if (retrieveBufferAutomaticModeOrbital.Count > 0) {
                        Tuple<Vector3, Quaternion, Vector3, float> oldOffset = retrieveBufferAutomaticModeOrbital.Last();
                        Vector3 directionToHead = head.transform.position - TargetObject.transform.position;
                        bool rightSide = Vector3.Dot(directionToHead, head.transform.right) > 0 ? true : false;
                        if (rightSide) {
                            gameObject.transform.localPosition = gameObject.transform.localPosition - (head.transform.right * (gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.x - oldOffset.Item1.x))
                                - head.transform.up * (gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.y - oldOffset.Item1.y) - head.transform.forward * (gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.z - oldOffset.Item1.z);
                                
                        }
                        else {
                            gameObject.transform.localPosition = gameObject.transform.localPosition - (-head.transform.right * (gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.x - oldOffset.Item1.x))
                                - head.transform.up * (gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.y - oldOffset.Item1.y) - head.transform.forward * (gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.z - oldOffset.Item1.z);
                        }
                        //gameObject.transform.localPosition = gameObject.transform.localPosition - gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset + oldOffset.Item1;
                        gameObject.transform.localEulerAngles = gameObject.transform.localEulerAngles - gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset + oldOffset.Item2.eulerAngles;
                        gameObject.transform.localScale = new Vector3(oldOffset.Item3.x * originalScale.x, oldOffset.Item3.y * originalScale.y, oldOffset.Item3.z * originalScale.z);
                        gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset = oldOffset.Item1;
                        gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset = oldOffset.Item2.eulerAngles;
                        gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset = oldOffset.Item3;
                        gameObject.GetComponent<ConstantViewSize>().TargetViewPercentV = oldOffset.Item4;
                        retrieveBufferAutomaticModeOrbital.RemoveAt(retrieveBufferAutomaticModeOrbital.Count - 1);
                    }
                }
                else {
                    if (retrieveBufferAutomaticModeWithoutOrbital.Count > 0) {
                        Tuple<Vector3, Quaternion, Vector3, float> oldOffset = retrieveBufferAutomaticModeWithoutOrbital.Last();
                        gameObject.transform.localPosition = gameObject.transform.localPosition - head.transform.right * (gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.x - oldOffset.Item1.x)
                                - head.transform.up * (gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.y - oldOffset.Item1.y) - head.transform.forward * (gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset.z - oldOffset.Item1.z);
                        gameObject.transform.localEulerAngles = gameObject.transform.localEulerAngles - gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset + oldOffset.Item2.eulerAngles;
                        gameObject.transform.localScale = new Vector3(oldOffset.Item3.x * originalScale.x, oldOffset.Item3.y * originalScale.y, oldOffset.Item3.z * originalScale.z);
                        gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset = oldOffset.Item1;
                        gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset = oldOffset.Item2.eulerAngles;
                        gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset = oldOffset.Item3;
                        gameObject.GetComponent<ConstantViewSize>().TargetViewPercentV = oldOffset.Item4;
                        retrieveBufferAutomaticModeWithoutOrbital.RemoveAt(retrieveBufferAutomaticModeWithoutOrbital.Count - 1);
                    }
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

        public void UpdateOffset(Tuple<Vector3, Quaternion, Vector3, float> newOffsets, Tuple<Vector3, Quaternion, Vector3, float> oldOffsets) {
            if(placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Automatic) {
                //Done in SaveOffsetBeforeMenipulation
            }
            else {
                manualModePositionOffset = newOffsets.Item1;
                manualModeRotationOffset = newOffsets.Item2.eulerAngles;
                manualModeScaleOffset = new Vector3(newOffsets.Item3.x / originalScale.x, newOffsets.Item3.y / originalScale.y, newOffsets.Item3.z / originalScale.z);
            }
        }

        public void EnterManualMode() {
            gameObject.transform.parent = head.transform;
            gameObject.transform.localPosition = manualModePositionOffset;
            gameObject.transform.localEulerAngles = manualModeRotationOffset;
            gameObject.transform.localScale = new Vector3(originalScale.x * manualModeScaleOffset.x, originalScale.y * manualModeScaleOffset.y, originalScale.z * manualModeScaleOffset.z);
            gameObject.transform.parent = null;
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
            gameObject.GetComponent<MenuHandler>().updateTime = 0;
            if (!compact) {
                gameObject.AddComponent<SolverHandler>().AdditionalOffset = followOffset;
                gameObject.AddComponent<SurfaceMagnetism>().SurfaceNormalOffset = surfaceMagnetismSafetyOffset;
                gameObject.GetComponent<SurfaceMagnetism>().UpdateLinkedTransform = true;
                gameObject.GetComponent<SurfaceMagnetism>().CurrentOrientationMode = SurfaceMagnetism.OrientationMode.None;
                gameObject.GetComponent<SurfaceMagnetism>().MagneticSurfaces[0] = LayerMask.GetMask("Spatial Mapping");
                gameObject.GetComponent<SurfaceMagnetism>().MaxRaycastDistance = maxFloatingDistance;
                gameObject.GetComponent<SurfaceMagnetism>().ClosestDistance = minFloatingDistance;
                gameObject.AddComponent<Follow>().MinDistance = minFloatingDistance;
                gameObject.GetComponent<Follow>().MaxDistance = maxFloatingDistance;
                gameObject.GetComponent<Follow>().DefaultDistance = defaultFloatingDistance;
                gameObject.GetComponent<Follow>().MaxViewHorizontalDegrees = followMaxViewHorizontalDegrees;
                gameObject.GetComponent<Follow>().MaxViewVerticalDegrees = followMaxViewVerticalDegrees;
                gameObject.GetComponent<Follow>().OrientToControllerDeadzoneDegrees = 20f;
                gameObject.GetComponent<Follow>().OrientationType = SolverOrientationType.Unmodified;
                gameObject.GetComponent<Follow>().UpdateLinkedTransform = true;
                gameObject.AddComponent<FinalPlacementOptimizer>().OriginalScale = gameObject.transform.localScale;
                gameObject.GetComponent<FinalPlacementOptimizer>().OrientationType = menuOrientationType;
                gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
                
            }
            else {
                if (placementService.HandTrackingEnabled) {
                    if (placementService.ArticulatedHandSupported) {
                        gameObject.AddComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                        gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                    }else{
                        gameObject.AddComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.ControllerRay;
                        gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                    }

                    gameObject.AddComponent<HandConstraint>().SafeZone = HandConstraint.SolverSafeZone.RadialSide;
                    gameObject.GetComponent<HandConstraint>().SafeZoneBuffer = 0.05f;
                    gameObject.GetComponent<HandConstraint>().UpdateLinkedTransform = true;
                    gameObject.GetComponent<HandConstraint>().UpdateWhenOppositeHandNear = true;
                    gameObject.AddComponent<FinalPlacementOptimizer>().OriginalScale = gameObject.transform.localScale;
                    gameObject.GetComponent<FinalPlacementOptimizer>().OrientationType = menuOrientationType;
                    gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
                }
                else {
                    gameObject.AddComponent<SolverHandler>();
                    gameObject.AddComponent<Follow>().MinDistance = 0.3f;
                    gameObject.GetComponent<Follow>().MaxDistance = 0.3f;
                    gameObject.GetComponent<Follow>().DefaultDistance = 0.3f;
                    gameObject.GetComponent<Follow>().MaxViewHorizontalDegrees = 40f;
                    gameObject.GetComponent<Follow>().MaxViewVerticalDegrees = 40f;
                    gameObject.GetComponent<Follow>().OrientToControllerDeadzoneDegrees = 20f;
                    gameObject.GetComponent<Follow>().OrientationType = SolverOrientationType.Unmodified;
                    gameObject.GetComponent<Follow>().UpdateLinkedTransform = true;
                    gameObject.AddComponent<FinalPlacementOptimizer>().OriginalScale = gameObject.transform.localScale;
                    gameObject.GetComponent<FinalPlacementOptimizer>().OrientationType = menuOrientationType;
                    gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
                }

            }
            (gameObject.GetComponent<ConstantViewSize>() ?? gameObject.AddComponent<ConstantViewSize>()).TargetViewPercentV = defaultTargetViewPercentV;
            gameObject.GetComponent<ConstantViewSize>().MinDistance = minDistance;
            gameObject.GetComponent<ConstantViewSize>().MaxDistance = maxDistance;
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
            if (constantViewSizeEnabled) {
                gameObject.GetComponent<BoundingBox>().ShowScaleHandles = false;
            }
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
            if (menuVariantType == MenuVariantType.MainMenu && !compact) {
                if(Physics.Raycast(head.transform.position,head.transform.forward,maxFloatingDistance,LayerMask.GetMask("Spatial Mapping"))) {
                    gameObject.GetComponent<SurfaceMagnetism>().enabled = false;
                }
                else {
                    gameObject.GetComponent<SurfaceMagnetism>().enabled = true;
                }
            }
            if (CollideWithSpatialMapping() || (distanceToMenu < minFloatingDistance + surfaceMagnetismSafetyOffset && closeToSpatialMapping)) {
                if (menuVariantType == MenuVariantType.MainMenu) {
                    if (!compact) {
                        
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
                        if (!compact) {
                            message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                            switchTo = message.switchType;
                            //Because the object is far, the HandConstraint solver should be activated on next call.
                        }
                        //Activate the HandConstraint solver or Follow solver
                        else {
                            if (placementService.HandTrackingEnabled) {
                                if (placementService.ArticulatedHandSupported) {
                                    gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                                    gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                                }
                                else {
                                    gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.ControllerRay;
                                    gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                                }
                                gameObject.GetComponent<HandConstraint>().enabled = true;
                                gameObject.GetComponent<Orbital>().enabled = false;
                            }
                            else {
                                gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.Head;
                                gameObject.transform.position = head.transform.position + head.transform.forward * 0.3f;
                                gameObject.GetComponent<Follow>().enabled = true;
                                gameObject.GetComponent<Orbital>().enabled = false;
                            }

                        }
                    }
                    else {
                        //The object is close, namely the Beside and Orbital solvers are activated.
                        //Begin the collision handling process:
                        //First step: Try to switch to compact version if it is a floating menu
                        if (!compact) {
                            message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                            switchTo = message.switchType;
                        }
                        //Second step: If it is already compact and there is still a collision, then activate the HandConstraint solver or Follow solver.
                        else {
                            if (placementService.HandTrackingEnabled) {
                                if (placementService.ArticulatedHandSupported) {
                                    gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                                    gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                                }
                                else {
                                    gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.ControllerRay;
                                    gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                                }
                                gameObject.GetComponent<HandConstraint>().enabled = true;
                                gameObject.GetComponent<Orbital>().enabled = false;
                            }
                            else {
                                gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.Head;
                                gameObject.transform.position = head.transform.position + head.transform.forward * 0.3f;
                                gameObject.GetComponent<Follow>().enabled = true;
                                gameObject.GetComponent<Orbital>().enabled = false;
                            }

                        }
                    }                    
                }
            }
            else {
                if (menuVariantType == MenuVariantType.MainMenu) {
                    if (compact) {
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
                    if (!compact) {                
                        //The object is between maxFloatingDistance and minFloatingDistance, use the Orbital solver.
                        if (targetDistance <= maxFloatingDistance && targetDistance >= minFloatingDistance) {

                            //If a switch between solvers is needed, save the current offsets for next switch.
                            if (gameObject.GetComponent<InBetween>().enabled) {
                                lastOffsetsInBetween = new Tuple<Vector3, Quaternion, Vector3, float>(gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset, Quaternion.Euler(gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset), gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset, gameObject.GetComponent<ConstantViewSize>().TargetViewPercentV);
                                gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset = lastOffsetsOrbital.Item1;
                                gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset = lastOffsetsOrbital.Item2.eulerAngles;
                                gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset = lastOffsetsOrbital.Item3;
                                gameObject.GetComponent<ConstantViewSize>().TargetViewPercentV = lastOffsetsOrbital.Item4;
                            }                        
                            gameObject.GetComponent<Orbital>().enabled = true;
                            gameObject.GetComponent<InBetween>().enabled = false;
                            gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;                           
                        }
                        else if (targetDistance < minFloatingDistance) {
                            message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                            switchTo = message.switchType;
                        }
                        else if (targetDistance > maxFloatingDistance) {

                            //If a switch between solvers is needed, save the current offsets for next switch.
                            if (gameObject.GetComponent<Orbital>().enabled) {
                                lastOffsetsOrbital = new Tuple<Vector3, Quaternion, Vector3, float>(gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset, Quaternion.Euler(gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset), gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset, gameObject.GetComponent<ConstantViewSize>().TargetViewPercentV);
                                gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset = lastOffsetsInBetween.Item1;
                                gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset = lastOffsetsInBetween.Item2.eulerAngles;
                                gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset = lastOffsetsInBetween.Item3;
                                gameObject.GetComponent<ConstantViewSize>().TargetViewPercentV = lastOffsetsInBetween.Item4;
                            }                          
                            //We should enable InBetween solver;
                            gameObject.GetComponent<Orbital>().enabled = false;
                            gameObject.GetComponent<InBetween>().enabled = true;
                            gameObject.GetComponent<InBetween>().PartwayOffset = (defaultFloatingDistance / targetDistance);
                            gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
                        }
                    }
                    //Compact 
                    else {
                        Vector3 inBetweenPositionOffsetFloatingVariant = placementService.GetInBetweenPositionOffsetOppositeType(gameObject);
                        Vector3 orbitalPositionOffsetFloatingVariant = placementService.GetOrbitalPositionOffsetOppositeType(gameObject);
                        //Check space around and try to switch to floating version.
                        if (targetDistance > maxFloatingDistance) {
                            //Check the space in front of user because InBetween solver should be activated
                            Vector3 center = placementService.GetInBetweenTarget().transform.position + Vector3.Normalize(TargetObject.transform.position - placementService.GetInBetweenTarget().transform.position) * defaultFloatingDistance 
                                + head.transform.right * inBetweenPositionOffsetFloatingVariant.x + head.transform.up * inBetweenPositionOffsetFloatingVariant.y + head.transform.forward * inBetweenPositionOffsetFloatingVariant.z;
                            if (!Physics.CheckBox(center, placementService.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))) {
                                Debug.Log("Front Free for InBetween");
                                message.switchType = PlacementMessage.SwitchType.CompactToFloating;
                                switchTo = message.switchType;
                            }
                            else {
                                if (placementService.HandTrackingEnabled) {
                                    if (placementService.ArticulatedHandSupported) {
                                        gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                                        gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                                    }
                                    else {
                                        gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.ControllerRay;
                                        gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                                    }
                                    gameObject.GetComponent<HandConstraint>().enabled = true;
                                    gameObject.GetComponent<Orbital>().enabled = false;
                                }
                                else {
                                    gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.Head;
                                    gameObject.transform.position = head.transform.position + head.transform.forward * 0.3f;
                                    gameObject.GetComponent<Follow>().enabled = true;
                                    gameObject.GetComponent<Orbital>().enabled = false;
                                }
                            }
                        }
                        else {
                            Vector3 directionToHead = head.transform.position - TargetObject.transform.position;
                            bool rightSide = Vector3.Dot(directionToHead, head.transform.right) > 0 ? true : false;
                            Vector3 centerFloating = Vector3.zero;
                            Vector3 centerCompact = Vector3.zero;
                            if (rightSide) {
                                centerFloating = TargetObject.transform.position + head.transform.right * (orbitalOffsetOppositeType.x + orbitalPositionOffsetFloatingVariant.x) + head.transform.up * (orbitalOffsetOppositeType.y + orbitalPositionOffsetFloatingVariant.y) + head.transform.forward * (orbitalOffsetOppositeType.z + orbitalPositionOffsetFloatingVariant.z);
                                centerCompact = TargetObject.transform.position + head.transform.right * (orbitalOffset.x + lastOffsetsOrbital.Item1.x) + head.transform.up * (orbitalOffset.y + lastOffsetsOrbital.Item1.y) + head.transform.forward * (orbitalOffset.z + lastOffsetsOrbital.Item1.z);
                            }
                            else {
                                centerFloating = TargetObject.transform.position + (-head.transform.right * (orbitalOffsetOppositeType.x + orbitalPositionOffsetFloatingVariant.x)) + head.transform.up * (orbitalOffsetOppositeType.y + orbitalPositionOffsetFloatingVariant.y) + head.transform.forward * (orbitalOffsetOppositeType.z + orbitalPositionOffsetFloatingVariant.z);
                                centerCompact = TargetObject.transform.position + (-head.transform.right * (orbitalOffset.x + lastOffsetsOrbital.Item1.x)) + head.transform.up * (orbitalOffset.y + lastOffsetsOrbital.Item1.y) + head.transform.forward * (orbitalOffset.z + lastOffsetsOrbital.Item1.z);
                            }
                            if (targetDistance >= minFloatingDistance && targetDistance <= maxFloatingDistance) {
                                //Orbital and Beside solver should be activated on the floating menu.
                                //Check the space "besides" the targetObject on the left or right side according to user's position, similar to Beside solver.

                                if (!Physics.CheckBox(centerFloating, placementService.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))
                                    && !Physics.Raycast(TargetObject.transform.position, centerFloating - TargetObject.transform.position, (centerFloating - TargetObject.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                                    Debug.Log("Besides Free for Floating Orbital");
                                    message.switchType = PlacementMessage.SwitchType.CompactToFloating;
                                    switchTo = message.switchType;
                                }
                                else if (!Physics.CheckBox(centerCompact, placementService.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))
                                   && !Physics.Raycast(TargetObject.transform.position, centerCompact - TargetObject.transform.position, (centerCompact - TargetObject.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                                    Debug.Log("Beside Free for Compact Orbital");
                                    gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.CustomOverride;
                                    gameObject.GetComponent<SolverHandler>().TransformOverride = TargetObject.transform;
                                    gameObject.GetComponent<Orbital>().enabled = true;
                                    gameObject.GetComponent<FinalPlacementOptimizer>().OrbitalOffset = orbitalOffset;
                                    gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset = lastOffsetsOrbital.Item1;
                                    gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset = lastOffsetsOrbital.Item2.eulerAngles;
                                    gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset = lastOffsetsOrbital.Item3;
                                    if (placementService.HandTrackingEnabled) {
                                        gameObject.GetComponent<HandConstraint>().enabled = false;
                                    }
                                    else {
                                        gameObject.GetComponent<Follow>().enabled = false;
                                    }
                                    gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
                                }
                            }
                            //If targetDistance < minFloatingDistance, the menu should remain compact.
                            if (targetDistance < minFloatingDistance) {
                                if (!Physics.CheckBox(centerCompact, placementService.GetStoredBoundingBoxOnCloseOppositeType(gameObject).extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"))
                                    && !Physics.Raycast(TargetObject.transform.position, centerCompact - TargetObject.transform.position, (centerCompact - TargetObject.transform.position).magnitude, LayerMask.GetMask("Spatial Mapping"))) {
                                    Debug.Log("Beside Free for Compact Orbital");
                                    gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.CustomOverride;
                                    gameObject.GetComponent<SolverHandler>().TransformOverride = TargetObject.transform;
                                    gameObject.GetComponent<Orbital>().enabled = true;
                                    gameObject.GetComponent<FinalPlacementOptimizer>().OrbitalOffset = orbitalOffset;
                                    if (placementService.HandTrackingEnabled) {
                                        gameObject.GetComponent<HandConstraint>().enabled = false;
                                    }
                                    else {
                                        gameObject.GetComponent<Follow>().enabled = false;
                                    }
                                    gameObject.GetComponent<FinalPlacementOptimizer>().enabled = true;
                                }
                            }
                        }
                        
                        
                    }
                    
                }
            }
        }

        /// <summary>
        /// Check Occlusion using raycast. 
        /// We don't need to check occlusion for main menu because of the SurfaceMagnetism solver and all menus with HandConstraint solver activated.
        /// </summary>
        private void CheckOcclusion() {
            if (menuVariantType == MenuVariantType.ObjectMenu) {
                float targetDistance = (head.transform.position - TargetObject.transform.position).magnitude;
                Ray ray = new Ray(head.transform.position, gameObject.transform.position - head.transform.position);
                float headMenuDistance = (gameObject.transform.position - head.transform.position).magnitude;
                if (!compact) {      
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
                        //If orbital activated，turn on the HandConstraint or Follow solver, else we do not need to do anything.
                        if (placementService.HandTrackingEnabled) {
                            if (!gameObject.GetComponent<HandConstraint>().enabled) {
                                if (placementService.ArticulatedHandSupported) {
                                    gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.HandJoint;
                                    gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                                }
                                else {
                                    gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.ControllerRay;
                                    gameObject.GetComponent<SolverHandler>().TrackedHandness = Handedness.Right;
                                }
                                gameObject.GetComponent<HandConstraint>().enabled = true;
                                gameObject.GetComponent<Orbital>().enabled = false;
                            }
                        }
                        else {
                            gameObject.GetComponent<SolverHandler>().TrackedTargetType = TrackedObjectType.Head;
                            gameObject.transform.position = head.transform.position + head.transform.forward * 0.3f;
                            gameObject.GetComponent<Follow>().enabled = true;
                            gameObject.GetComponent<Orbital>().enabled = false;
                        }

                    }
                    //Check if there is no (potential) occlusion anymore
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
                accumulatedTimeForSuggestion += Time.deltaTime;
                if (suggestionTime > suggestionTimeInterval && accumulatedTimeForSuggestion > accumulatedTimeForSuggestionThreshold) {
                    placementService.EnterAdjustmentMode();
                    Component suggestionPanel = menuVariantType == MenuVariantType.MainMenu? 
                                Dialog.Open(placementService.SuggestionPanel, DialogButtonType.Close | DialogButtonType.Accept, "Menu Needs Adjustment",
                                "Collision Detected for the main menu! You might need to move to another location, move it closer or switch it to compact version if possible. You can click 'Accept' to switch to automatic mode.", true)
                                :
                                Dialog.Open(placementService.SuggestionPanel, DialogButtonType.Close | DialogButtonType.Accept, "Menu Needs Adjustment",
                                "Collision Detected for an object menu! You might need to move to another location, move it closer or switch it to compact version if possible. You can click 'Accept' to switch to automatic mode.", true);                        
                    suggestionPanel.gameObject.GetComponent<Follow>().MinDistance = 0.4f;
                    suggestionPanel.gameObject.GetComponent<Follow>().MaxDistance = 0.4f;
                    suggestionPanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.4f;
                    suggestionPanel.gameObject.transform.forward = head.transform.forward;
                    suggestionTime = 0;
                    accumulatedTimeForSuggestion = 0;
                }
            }
            //Check occlusion
            else {
                Ray ray = new Ray(head.transform.position, gameObject.transform.position - head.transform.position);
                float headMenuDistance = (gameObject.transform.position - head.transform.position).magnitude;
                if(Physics.Raycast(ray, headMenuDistance, LayerMask.GetMask("Spatial Mapping"))) {
                    accumulatedTimeForSuggestion += Time.deltaTime;
                    if (suggestionTime > suggestionTimeInterval && accumulatedTimeForSuggestion > accumulatedTimeForSuggestionThreshold) {
                        placementService.EnterAdjustmentMode();
                        Component suggestionPanel = menuVariantType == MenuVariantType.MainMenu?
                                Dialog.Open(placementService.SuggestionPanel, DialogButtonType.Close | DialogButtonType.Accept, "Menu Needs Adjustment",
                                    "Occlusion Detected for the main menu! You might need to move to another location, move it closer or switch it to compact version if possible. You can click 'Accept' to switch to automatic mode.", true)
                                :
                                Dialog.Open(placementService.SuggestionPanel, DialogButtonType.Close | DialogButtonType.Accept, "Menu Needs Adjustment",
                                    "Occlusion Detected for an object menu! You might need to move to another location, move it closer or switch it to compact version if possible. You can click 'Accept' to switch to automatic mode.", true);
                        suggestionPanel.gameObject.GetComponent<Follow>().MinDistance = 0.4f;
                        suggestionPanel.gameObject.GetComponent<Follow>().MaxDistance = 0.4f;
                        suggestionPanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.4f;
                        suggestionPanel.gameObject.transform.forward = head.transform.forward;
                        suggestionTime = 0;
                        accumulatedTimeForSuggestion = 0;
                    }
                }
                else {
                    accumulatedTimeForSuggestion = 0;
                }                
            }

        }
        private void CheckInactivity() {
            GameObject gazeTarget = CoreServices.InputSystem.GazeProvider.GazeTarget;
            if (gazeTarget != null) {
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
            switch (boundingBoxType) {
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
            return Physics.CheckBox(GetBoundingBox().center, GetBoundingBox().extents, Quaternion.identity, LayerMask.GetMask("Spatial Mapping"));
        }


#endregion Private Methods

        //Methods For EVALUATION
        private float sumDistance = 0;
        private int distanceUpdateCount = 0;
        private float distanceUpdateTime = 0;
        private float distanceUpdateTimeThreshold = 3;
        private string path;
        FileStream testData;

        StreamWriter sw;

        private void CalculateAverageDistance() {
            if(placementService.PlacementMode != MenuPlacementService.MenuPlacementServiceMode.Adjustment) {
                if (distanceUpdateTime > distanceUpdateTimeThreshold) {
                    distanceUpdateCount++;
                    sumDistance += (gameObject.transform.position - head.transform.position).magnitude;
                    distanceUpdateTime = 0;
                }
                else {
                    distanceUpdateTime += Time.deltaTime;
                }
            }
        }

        private void SaveAverageDistance() {
            float aveDis = sumDistance / distanceUpdateCount;
            sumDistance = 0;
            distanceUpdateCount = 0;
            sw.WriteLine("The average distance between the user and this menu is:   " + aveDis + " meter.");
            sw.WriteLine("");
        }

        private void SaveLastOffset() {
            sw.WriteLine("The best suited offsets in AUTOMATIC mode:  ");
            sw.WriteLine("Position Offset: " + gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset);
            sw.WriteLine("Rotation Offset: " + gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset);
            sw.WriteLine("Scale Offset: " + gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset);
            sw.WriteLine("");
            sw.WriteLine("The best suited offsets in MANUAL mode:  ");
            sw.WriteLine("Position Offset: " + manualModePositionOffset);
            sw.WriteLine("Rotation Offset: " + manualModeRotationOffset);
            sw.WriteLine("Scale Offset: " + manualModeScaleOffset);
            sw.WriteLine("");
            sw.Close();
            sw.Dispose();

        }

        private void SaveOnUWP() {
            float aveDis = sumDistance / distanceUpdateCount;
            sumDistance = 0;
            distanceUpdateCount = 0;
            path = Application.persistentDataPath + "/" + menuID + "_" + aveDis + "_" + gameObject.GetComponent<FinalPlacementOptimizer>().PositionOffset + "_" + 
                gameObject.GetComponent<FinalPlacementOptimizer>().RotationOffset + "_" + gameObject.GetComponent<FinalPlacementOptimizer>().ScaleOffset + "_" + manualModePositionOffset + "_" +
                manualModeRotationOffset + "_" + manualModeScaleOffset + "F";
            testData = new FileStream(path, FileMode.Create);
        }
    }
}
