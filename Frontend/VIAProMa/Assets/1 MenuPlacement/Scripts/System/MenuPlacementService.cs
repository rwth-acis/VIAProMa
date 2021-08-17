using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.Utilities;
using i5.VIAProMa.UI.AppBar;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.WindowsMixedReality.Input;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace MenuPlacement {
    [CreateAssetMenu(menuName = "Scriptable Objects/Menu Placement Service ")]
    public class MenuPlacementService : ScriptableObject, IService {

        public enum MenuPlacementServiceMode {
            Automatic,
            Manual,
            Adjustment
        }

        //Use to switch between the floating and campact version
        [Header("Menus")]
        [Tooltip("Drag the menu objects here. If you don't want to use one type of them, just leave it.")]
        [SerializeField] private MenuVariants mainMenu;
        [Tooltip("Drag the menu objects here. If you don't want to use one type of them, just leave it.")]
        [SerializeField] private List<MenuVariants> objectMenus;
        [Header("User Interface Components")]
        [Tooltip("The Menu Controller enables the manipulation of created menus and interaction with the system.")]
        [SerializeField] private GameObject menuController;
        [SerializeField] private GameObject appBar;
        [Tooltip("The dialog prefab for suggestions in manual mode")]
        [SerializeField] private GameObject suggestionPanel;
        [Tooltip("The default placement mode")]
        [SerializeField] private DefaultMode defaultPlacementMode = DefaultMode.Automatic;

        private MenuPlacementServiceMode placementMode;
        public bool ArticulatedHandSupported { get; private set; }
        public bool MotionControllerSupported { get; private set; }
        public bool GGVHandSupported { get; private set; }

        public bool HandTrackingEnabled { get; private set; }

        public MenuPlacementServiceMode PlacementMode
        {
            get => placementMode;
            set
            {
                placementMode = value;
            }
        }

        private enum DefaultMode {
            Automatic,
            Manual
        }

        public GameObject AppBar
        {
            get => appBar;
        }

        public GameObject SuggestionPanel
        {
            get => suggestionPanel;
        }

        private Bounds floatingMainMenuBoundingBoxOnClose;
        private Bounds compactMainMenuBoundingBoxOnClose;
        //Dictionary<menuID, Bounds>
        private Dictionary<int, Bounds> floatingObjectMenuBoundingBoxOnClose = new Dictionary<int, Bounds>();
        private Dictionary<int, Bounds> compactObjectMenuBoundingBoxOnClose = new Dictionary<int, Bounds>();


        //Use the corresponding ID of a MenuVariants to get the ObjectPool for it.
        private int floatingMainMenuPoolID;
        private int compactMainMenuPoolID;
        //Dictionary<menuID, menuPoolID>
        private Dictionary<int, int> floatingObjectMenuPoolIDs = new Dictionary<int, int>();
        private Dictionary<int, int> compactObjectMenuPoolIDs = new Dictionary<int, int>();

        //Dictionary<menuID, PositionOffset>
        private Dictionary<int, Vector3> inBetweenPositionOffsetOnClose = new Dictionary<int, Vector3>();
        private Dictionary<int, Vector3> orbitalPositionOffsetOnClose = new Dictionary<int, Vector3>();

        private GameObject inBetweenTarget;
        public bool SuggestionPanelOn { get; set; }
        public MenuPlacementServiceMode PreviousMode { get; set; }



        public void Initialize(IServiceManager owner) {
            CheckMenuInitialization();
            //make a pool for each MenuVariants and assign the index of the pool to the corresponding variable. Ignore the not assigned menu.
            if (mainMenu.floatingMenu != null) {
                floatingMainMenuPoolID = ObjectPool<GameObject>.CreateNewPool(1);
            }
            if (mainMenu.compactMenu != null) {
                compactMainMenuPoolID = ObjectPool<GameObject>.CreateNewPool(1);
            }

            foreach (MenuVariants m in objectMenus) {
                if (m.floatingMenu != null) { 
                    floatingObjectMenuPoolIDs.Add(m.floatingMenu.GetComponent<MenuHandler>().menuID, ObjectPool<GameObject>.CreateNewPool(5));
                }
                if (m.compactMenu != null) {
                    compactObjectMenuPoolIDs.Add(m.compactMenu.GetComponent<MenuHandler>().menuID, ObjectPool<GameObject>.CreateNewPool(5));
                }
            }

            if (defaultPlacementMode == DefaultMode.Automatic) {
                placementMode = MenuPlacementServiceMode.Automatic;
            }
            else {
                placementMode = MenuPlacementServiceMode.Manual;
            }

            SuggestionPanelOn = false;
            CreateInBetweenTarget();
            CreateMenuController();
            CheckCapability();

        }

        public void Cleanup() {
            placementMode = MenuPlacementServiceMode.Automatic;
        }
        

        #region Public Methods
        public GameObject GetInBetweenTarget() {
            return inBetweenTarget;
        }
        
        public void Quit() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }

        /// <summary>
        /// Switch between automatic and manual mode
        /// </summary>
        public void SwitchMode() {
            if(placementMode == MenuPlacementServiceMode.Automatic) {
                placementMode = MenuPlacementServiceMode.Manual;
            }
            else {
                placementMode = MenuPlacementServiceMode.Automatic;
            }
        }
        
        public void EnterManualMode() {
            placementMode = MenuPlacementServiceMode.Manual;
        }

        public void EnterAutomaticMode() {
            placementMode = MenuPlacementServiceMode.Automatic;
        }

        public void EnterAdjustmentMode() {
            PreviousMode = placementMode;
            placementMode = MenuPlacementServiceMode.Adjustment;
        }

        public void ExitAdjustmentMode() {
            placementMode = PreviousMode;
        }

        /// <summary>
        /// Fetch a menu object from the ObjectPool of the origin menu
        /// </summary>
        public GameObject OpenMenu(GameObject origin) {
            MenuHandler handler = origin.GetComponent<MenuHandler>();
            if (handler.menuVariantType == MenuHandler.MenuVariantType.MainMenu) {
                if (handler.compact) {
                    return ObjectPool<GameObject>.RequestResource(compactMainMenuPoolID, () => { return Instantiate(mainMenu.compactMenu); });
                }
                else {
                    return ObjectPool<GameObject>.RequestResource(floatingMainMenuPoolID, () => { return Instantiate(mainMenu.floatingMenu); });
                }
            }
            else {
                if (handler.compact) {
                    return ObjectPool<GameObject>.RequestResource(compactObjectMenuPoolIDs[handler.menuID], () => { return Instantiate(GetObjectMenuWithID(handler.menuID)); });
                }
                else {
                    return ObjectPool<GameObject>.RequestResource(floatingObjectMenuPoolIDs[handler.menuID], () => { return Instantiate(GetObjectMenuWithID(handler.menuID)); });
                }  
            }
        }

        /// <summary>
        /// Return a menu object to its ObjectPool
        /// </summary>
        public void CloseMenu(GameObject menu) {
            MenuHandler handler = menu.GetComponent<MenuHandler>();
            if (handler.menuVariantType == MenuHandler.MenuVariantType.MainMenu) {
                if (handler.compact) {
                    Debug.Log("Close Compact Main Menu");
                    ObjectPool<GameObject>.ReleaseResource(compactMainMenuPoolID, menu);
                }
                else {
                    ObjectPool<GameObject>.ReleaseResource(floatingMainMenuPoolID, menu);
                }    
            }
            else {
                if (handler.compact) {
                    ObjectPool<GameObject>.ReleaseResource(compactObjectMenuPoolIDs[handler.menuID], menu);
                }
                else {
                    ObjectPool<GameObject>.ReleaseResource(floatingObjectMenuPoolIDs[handler.menuID], menu);
                }
                
            }
        }

        public void UpdatePlacement(PlacementMessage message, GameObject menu) {
            switch (message.switchType) {
                case PlacementMessage.SwitchType.FloatingToCompact:
                    Debug.Log("The menu is floating: " + !menu.GetComponent<MenuHandler>().compact);
                    if (!menu.GetComponent<MenuHandler>().compact) {
                        if (SwitchToCompact(menu) != menu) {
                            menu.GetComponent<MenuHandler>().Close();
                            SwitchToCompact(menu).GetComponent<MenuHandler>().Open(menu.GetComponent<MenuHandler>().TargetObject);
                        }
                    }
                    break;
                case PlacementMessage.SwitchType.CompactToFloating:
                    Debug.Log("The menu is compact: " + menu.GetComponent<MenuHandler>().compact);
                    if (menu.GetComponent<MenuHandler>().compact) {
                        if (SwitchToFloating(menu) != menu) {
                            menu.GetComponent<MenuHandler>().Close();
                            SwitchToFloating(menu).GetComponent<MenuHandler>().Open(menu.GetComponent<MenuHandler>().TargetObject);
                        }
                    }
                    break;
            }
        }

        public void SwitchVariant(PlacementMessage message, GameObject menu, GameObject targetObject) {
            switch (message.switchType) {
                case PlacementMessage.SwitchType.FloatingToCompact:
                    Debug.Log("The menu is floating: " + !menu.GetComponent<MenuHandler>().compact);
                    if (!menu.GetComponent<MenuHandler>().compact) {
                        if (SwitchToCompact(menu) != menu) {
                            menu.GetComponent<MenuHandler>().Close();
                            SwitchToCompact(menu).GetComponent<MenuHandler>().Open(menu.GetComponent<MenuHandler>().TargetObject);
                        }
                    }
                    break;
                case PlacementMessage.SwitchType.CompactToFloating:
                    Debug.Log("The menu is compact: " + menu.GetComponent<MenuHandler>().compact);
                    if (menu.GetComponent<MenuHandler>().compact) {
                        if (SwitchToFloating(menu) != menu) {
                            menu.GetComponent<MenuHandler>().Close();
                            SwitchToFloating(menu).GetComponent<MenuHandler>().Open(menu.GetComponent<MenuHandler>().TargetObject);
                        }
                    }
                    break;
            }
        }

        public Bounds GetStoredBoundingBoxOnCloseOppositeType(GameObject menu) {
            MenuHandler handler = menu.GetComponent<MenuHandler>();
            if (handler.menuVariantType == MenuHandler.MenuVariantType.MainMenu) {
                if (handler.compact) {
                    return floatingMainMenuBoundingBoxOnClose;
                }
                else {                  
                    return compactMainMenuBoundingBoxOnClose;
                }
            }
            else {
                Bounds res;
                if (handler.compact) {
                    int menuID = SwitchToFloating(menu).GetComponent<MenuHandler>().menuID;
                    if (floatingObjectMenuBoundingBoxOnClose.TryGetValue(menuID, out res)){
                        return res;
                    }
                    else {
                        Debug.LogError("Bounding Box Not Fund");
                        return new Bounds();
                    }
                }
                else {
                    int menuID = SwitchToCompact(menu).GetComponent<MenuHandler>().menuID;
                    if(compactObjectMenuBoundingBoxOnClose.TryGetValue(menuID, out res)) {
                        return res;
                    }
                    else{
                        Debug.LogError("Bounding Box Not Fund");
                        return new Bounds();
                    }
                }
            }
        }

        public void StoreBoundingBoxOnClose(GameObject menu, Bounds boundingBox) {
            MenuHandler handler = menu.GetComponent<MenuHandler>();
            if (handler.menuVariantType == MenuHandler.MenuVariantType.MainMenu) {
                if (handler.compact) {
                    compactMainMenuBoundingBoxOnClose = boundingBox;
                }
                else {
                    floatingMainMenuBoundingBoxOnClose = boundingBox;
                }
            }
            else {
                if (handler.compact) {
                    if (compactObjectMenuBoundingBoxOnClose.ContainsKey(handler.menuID)) {
                        compactObjectMenuBoundingBoxOnClose[handler.menuID] = boundingBox;
                    }
                    else {
                        compactObjectMenuBoundingBoxOnClose.Add(handler.menuID, boundingBox);
                    }
                }
                else {
                    if (floatingObjectMenuBoundingBoxOnClose.ContainsKey(handler.menuID)) {
                        floatingObjectMenuBoundingBoxOnClose[handler.menuID] = boundingBox;
                    }
                    else {
                        floatingObjectMenuBoundingBoxOnClose.Add(handler.menuID, boundingBox);
                    }
                }
            }
        }

        public void StoreInBetweenPositionOffsetOnClose(GameObject menu, Vector3 offset) {
            MenuHandler handler = menu.GetComponent<MenuHandler>();
            if (inBetweenPositionOffsetOnClose.ContainsKey(handler.menuID)) {
                inBetweenPositionOffsetOnClose[handler.menuID] = offset;
            }
            else {
                inBetweenPositionOffsetOnClose.Add(handler.menuID, offset);
            }
        }

        public Vector3 GetInBetweenPositionOffsetOppositeType(GameObject menu) {
            MenuHandler handler = menu.GetComponent<MenuHandler>();
            Vector3 res;
            if (handler.compact) {
                int menuID = SwitchToFloating(menu).GetComponent<MenuHandler>().menuID;
                if (inBetweenPositionOffsetOnClose.TryGetValue(menuID, out res)) {
                    return res;
                }
                else {
                    return new Vector3();
                }
            }
            else {
                int menuID = SwitchToCompact(menu).GetComponent<MenuHandler>().menuID;
                if (inBetweenPositionOffsetOnClose.TryGetValue(menuID, out res)) {
                    return res;
                }
                else {
                    return new Vector3();
                }
            }
            
        }

        public void StoreOrbitalPositionOffsetOnClose(GameObject menu, Vector3 offset) {
            MenuHandler handler = menu.GetComponent<MenuHandler>();
            if (orbitalPositionOffsetOnClose.ContainsKey(handler.menuID)) {
                orbitalPositionOffsetOnClose[handler.menuID] = offset;
            }
            else {
                orbitalPositionOffsetOnClose.Add(handler.menuID, offset);
            }
        }

        public Vector3 GetOrbitalPositionOffsetOppositeType(GameObject menu) {
            MenuHandler handler = menu.GetComponent<MenuHandler>();
            Vector3 res;
            if (handler.compact) {
                int menuID = SwitchToFloating(menu).GetComponent<MenuHandler>().menuID;
                if (orbitalPositionOffsetOnClose.TryGetValue(menuID, out res)) {
                    return res;
                }
                else {
                    return new Vector3();
                }
            }
            else {
                int menuID = SwitchToCompact(menu).GetComponent<MenuHandler>().menuID;
                if (orbitalPositionOffsetOnClose.TryGetValue(menuID, out res)) {
                    return res;
                }
                else {
                    return new Vector3();
                }
            }
        }
        public Vector3 GetOrbitalOffsetOppositeType(GameObject menu) {
            MenuHandler handler = menu.GetComponent<MenuHandler>();
            if (handler.menuVariantType == MenuHandler.MenuVariantType.MainMenu) {
                if (handler.compact) {
                    return mainMenu.floatingMenu.GetComponent<MenuHandler>().OrbitalOffset;
                }
                else {
                    return mainMenu.compactMenu.GetComponent<MenuHandler>().OrbitalOffset;
                }
            }
            else {
                if (handler.compact) {
                    return SwitchToFloating(GetObjectMenuWithID(handler.menuID)).GetComponent<MenuHandler>().OrbitalOffset;
                }
                else {
                    return SwitchToCompact(GetObjectMenuWithID(handler.menuID)).GetComponent<MenuHandler>().OrbitalOffset;
                }
            }
        }



        #endregion Public Methods

        #region Private Methods

        private void CheckMenuInitialization() {
            List<GameObject> menus = new List<GameObject>();
            if (mainMenu.compactMenu == null) {
                Debug.LogWarning("The compact main menu is not assigned in Menu Placement Service. Make sure you set it correctly unless you don't want to have it.");
            }
            else {
                if (!mainMenu.compactMenu.GetComponent<MenuHandler>().compact) {
                    Debug.LogError("The compact main menu is not set to 'compact'. Make sure you set it correctly in the insepctor.");
                }
                menus.Add(mainMenu.compactMenu);
            }

            if(mainMenu.floatingMenu == null) {
                Debug.LogWarning("The floating main menu is not assigned in Menu Placement Service. Make sure you set it correctly unless you don't want to have it.");
            }
            else {
                if (mainMenu.floatingMenu.GetComponent<MenuHandler>().compact) {
                    Debug.LogError("The floating main menu is set to 'compact'. Make sure you set it correctly in the insepctor");
                }
                menus.Add(mainMenu.floatingMenu);
            }

            if(mainMenu.floatingMenu != null && mainMenu.compactMenu != null) {
                if(mainMenu.floatingMenu.GetComponent<MenuHandler>().MinFloatingDistance != mainMenu.compactMenu.GetComponent<MenuHandler>().MinFloatingDistance) {
                    Debug.LogError("The 'Min Floating Distance' of main menus are not identical");
                }

                if(mainMenu.floatingMenu.GetComponent<MenuHandler>().MaxFloatingDistance != mainMenu.compactMenu.GetComponent<MenuHandler>().MaxFloatingDistance) {
                    Debug.LogError("The 'Max Floating Distance' of main menus are not identical");
                }

                if(mainMenu.floatingMenu.GetComponent<MenuHandler>().DefaultFloatingDistance != mainMenu.compactMenu.GetComponent<MenuHandler>().DefaultFloatingDistance) {
                    Debug.LogError("The 'Default Floating Distance' of main menus are not identical");
                }
            }

            foreach (MenuVariants m in objectMenus){
                if (m.compactMenu == null) {
                    Debug.LogWarning("One compact object menu is not assigned to Menu Placement Service. Make sure you set it correctly unless you don't want to have it.");
                }
                else {
                    if (!m.compactMenu.GetComponent<MenuHandler>().compact) {
                        Debug.LogError("The compact object menu '" + m.compactMenu + "' is not set to 'compact'. Make sure you set it correctly in the insepctor");
                    }
                    menus.Add(m.compactMenu);
                }

                if (m.floatingMenu == null) {
                    Debug.LogWarning("One floating object menu is not assigned to Menu Placement Service. Make sure you set it correctly unless you don't want to have it.");
                }
                else {
                    if (m.floatingMenu.GetComponent<MenuHandler>().compact) {
                        Debug.LogError("The floating object menu '" + m.floatingMenu + "' is set to 'compact'. Make sure you set it correctly in the inspector.");
                    }
                    menus.Add(m.floatingMenu);
                }

                if (m.floatingMenu != null && m.compactMenu != null) {
                    if (m.floatingMenu.GetComponent<MenuHandler>().MinFloatingDistance != m.compactMenu.GetComponent<MenuHandler>().MinFloatingDistance) {
                        Debug.LogError("The 'Min Floating Distance' of the object menus " + m + "are not identical");
                    }

                    if (m.floatingMenu.GetComponent<MenuHandler>().MaxFloatingDistance != m.compactMenu.GetComponent<MenuHandler>().MaxFloatingDistance) {
                        Debug.LogError("The 'Max Floating Distance' of the object menus " + m + "are not identical");
                    }

                    if (m.floatingMenu.GetComponent<MenuHandler>().DefaultFloatingDistance != m.compactMenu.GetComponent<MenuHandler>().DefaultFloatingDistance) {
                        Debug.LogError("The 'Default Floating Distance' of the object menus " + m + "are not identical");
                    }
                }

            }

            foreach (GameObject menu in menus) {
                if (!menu.GetComponent<MenuHandler>()) {
                    Debug.LogError("The menu object '" + menu + "' dosen't have a MenuHandler component. Please make sure every menu object you assigned to Menu Placement Service has a MenuHandler attached to it");
                    return;
                }
            }

            for (int i = 0; i < menus.Count; i++) {
                for (int j = i + 1; j < menus.Count; j++) {
                    if (menus[i].GetComponent<MenuHandler>().menuID == menus[j].GetComponent<MenuHandler>().menuID) {
                        Debug.LogError("The menu objects '" + menus[i] + "' and '" + menus[j] + "' have the same menuID. Make sure every menu object you assigned to Menu Placement Service has an identical menuID.");
                    }
                }
            }

        }
        private void CreateMenuController() {
            Instantiate(menuController);
        }

        /// <summary>
        /// Create a target which is lower than head for the InBetween solver on object menus
        /// </summary>
        private void CreateInBetweenTarget() {
            inBetweenTarget = new GameObject("InBetween Target");
            inBetweenTarget.transform.parent = CameraCache.Main.transform;
            inBetweenTarget.transform.position = new Vector3(CameraCache.Main.transform.position.x, CameraCache.Main.transform.position.y - 0.15f, CameraCache.Main.transform.position.z);
        }


        private GameObject GetObjectMenuWithID(int menuID) {
            foreach(MenuVariants v in objectMenus) {
                if(v.floatingMenu.GetComponent<MenuHandler>().menuID == menuID) {
                    return v.floatingMenu;
                }
                if(v.compactMenu.GetComponent<MenuHandler>().menuID == menuID) {
                    return v.compactMenu;
                }
            }
            Debug.LogError("The menu with menuID = " + menuID + " cannot be found");
            return null;
        }

        //Check if a menu is a clone of an origin through menuID.
        private bool isSameMenu(GameObject origin, GameObject clone) {
            if(origin.GetComponent<MenuHandler>().menuID == clone.GetComponent<MenuHandler>().menuID) {
                return true;
            }
            else {
                return false;
            }

        }

        private GameObject SwitchToCompact(GameObject menu) {
            if (menu.GetComponent<MenuHandler>().menuVariantType == MenuHandler.MenuVariantType.ObjectMenu) {
                foreach (MenuVariants v in objectMenus) {
                    if (isSameMenu(v.floatingMenu, menu)) {
                        return v.compactMenu;
                    }
                }
            }
            else {
                return mainMenu.compactMenu;
            }
            return null;
        }

        private GameObject SwitchToFloating(GameObject menu) {
            if (menu.GetComponent<MenuHandler>().menuVariantType == MenuHandler.MenuVariantType.ObjectMenu) {
                foreach (MenuVariants v in objectMenus) {
                    if (isSameMenu(v.compactMenu, menu)) {
                        return v.floatingMenu;
                    }
                }
            }
            else {
                return mainMenu.floatingMenu;
            }
            return null;
        }

        #endregion Private Methods

        private void CheckCapability() {
            if(CoreServices.InputSystem is IMixedRealityCapabilityCheck capabilityChecker) {
                ArticulatedHandSupported = capabilityChecker.CheckCapability(MixedRealityCapability.ArticulatedHand);
                MotionControllerSupported = capabilityChecker.CheckCapability(MixedRealityCapability.MotionController);
                GGVHandSupported = capabilityChecker.CheckCapability(MixedRealityCapability.GGVHand);
                if(ArticulatedHandSupported || MotionControllerSupported) {
                    HandTrackingEnabled = true;
                }
                else {
                    HandTrackingEnabled = false;
                }
                /*Dialog.Open(SuggestionPanel, DialogButtonType.OK, "Menu Needs Adjustment",
                                        "ArticulatedHandSupported,: "+ArticulatedHandSupported + "  MotionControllerSupported: " + MotionControllerSupported + "  GGVHandSupported: " + GGVHandSupported, true);*/
            }
        }

    }
}
