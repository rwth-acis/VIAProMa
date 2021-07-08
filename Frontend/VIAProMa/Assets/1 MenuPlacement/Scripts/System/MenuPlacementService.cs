using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.Utilities;
using i5.VIAProMa.UI.AppBar;
using Microsoft.MixedReality.Toolkit.Boundary;
using Microsoft.MixedReality.Toolkit.OpenVR.Headers;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
        [SerializeField] private MenuPlacementServiceMode placementMode = MenuPlacementServiceMode.Automatic;

        public MenuPlacementServiceMode PlacementMode
        {
            get => placementMode;
            set
            {
                placementMode = value;
            }
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
        private GameObject inBetweenTarget;

        public bool SuggestionPanelOn { get; set; }
        public MenuPlacementServiceMode PreviousMode { get; set; }



        public void Initialize(IServiceManager owner) {
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
            SuggestionPanelOn = false;
            CreateInBetweenTarget();
            CreateMenuController();
        }

        public void Cleanup() {
            placementMode = MenuPlacementServiceMode.Automatic;
        }
        

        #region Public Methods
        public GameObject GetInBetweenTarget() {
            return inBetweenTarget;
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
            if (handler.isMainMenu) {
                if (handler.isCompact) {
                    return ObjectPool<GameObject>.RequestResource(compactMainMenuPoolID, () => { return Instantiate(mainMenu.compactMenu); });
                }
                else {
                    return ObjectPool<GameObject>.RequestResource(floatingMainMenuPoolID, () => { return Instantiate(mainMenu.floatingMenu); });
                }
            }
            else {
                if (handler.isCompact) {
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
            if (handler.isMainMenu) {
                if (handler.isCompact) {
                    Debug.Log("Close Compact Main Menu");
                    ObjectPool<GameObject>.ReleaseResource(compactMainMenuPoolID, menu);
                }
                else {
                    ObjectPool<GameObject>.ReleaseResource(floatingMainMenuPoolID, menu);
                }    
            }
            else {
                if (handler.isCompact) {
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
                    Debug.Log("The menu is floating: " + !menu.GetComponent<MenuHandler>().isCompact);
                    if (!menu.GetComponent<MenuHandler>().isCompact) {
                        if (SwitchToCompact(menu) != menu) {
                            menu.GetComponent<MenuHandler>().Close();
                            SwitchToCompact(menu).GetComponent<MenuHandler>().Open(menu.GetComponent<MenuHandler>().TargetObject);
                        }
                    }
                    break;
                case PlacementMessage.SwitchType.CompactToFloating:
                    Debug.Log("The menu is compact: " + menu.GetComponent<MenuHandler>().isCompact);
                    if (menu.GetComponent<MenuHandler>().isCompact) {
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
            if (handler.isMainMenu) {
                if (handler.isCompact) {
                    return floatingMainMenuBoundingBoxOnClose;
                }
                else {                  
                    return compactMainMenuBoundingBoxOnClose;
                }
            }
            else {
                Bounds res;
                if (handler.isCompact) {
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
            if (handler.isMainMenu) {
                if (handler.isCompact) {
                    compactMainMenuBoundingBoxOnClose = boundingBox;
                }
                else {
                    floatingMainMenuBoundingBoxOnClose = boundingBox;
                }
            }
            else {
                if (handler.isCompact) {
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

        public Vector3 GetOrbitalOffsetOppositeType(GameObject menu) {
            MenuHandler handler = menu.GetComponent<MenuHandler>();
            if (handler.isMainMenu) {
                if (handler.isCompact) {
                    return mainMenu.floatingMenu.GetComponent<MenuHandler>().OrbitalOffset;
                }
                else {
                    return mainMenu.compactMenu.GetComponent<MenuHandler>().OrbitalOffset;
                }
            }
            else {
                if (handler.isCompact) {
                    return SwitchToFloating(GetObjectMenuWithID(handler.menuID)).GetComponent<MenuHandler>().OrbitalOffset;
                }
                else {
                    return SwitchToCompact(GetObjectMenuWithID(handler.menuID)).GetComponent<MenuHandler>().OrbitalOffset;
                }
            }
        }



        #endregion Public Methods

        #region Private Methods

        private void CreateMenuController() {
            Instantiate(menuController);
        }

        /// <summary>
        /// Create a target which is lower than head for the InBetween solver on object menus
        /// </summary>
        private void CreateInBetweenTarget() {
            inBetweenTarget = new GameObject("InBetween Target");
            inBetweenTarget.transform.parent = CameraCache.Main.transform;
            inBetweenTarget.transform.position = new Vector3(CameraCache.Main.transform.position.x, CameraCache.Main.transform.position.y - 0.2f, CameraCache.Main.transform.position.z);
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
            if (!menu.GetComponent<MenuHandler>().isMainMenu) {
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
            if (!menu.GetComponent<MenuHandler>().isMainMenu) {
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

    }
}
