using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.UI.AppBar;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.Input;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuPlacement {
    /// <summary>
    /// This script extend the functionalities of the app bar in VIAProMa
    /// If more functionalities are required, it is recommanded to write an extra script that implements the IMPAppBarController interface or inherits this script.
    /// </summary>
    public class MPAppBarController : MonoBehaviour {

        //The menu object
        private GameObject targetObject;
        private MenuHandler handler;
        private MenuPlacementService placementService;
        private PlacementMessage message = new PlacementMessage();
        [SerializeField] private Sprite unlocked;
        [SerializeField] private Sprite locked;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private GameObject slider;
        public Vector3 StartPosition { get; private set; }
        public Quaternion StartRotation { get; private set; }
        public Vector3 StartScale { get; private set; }

        public Vector3 ConstantViewSizeStartScale { get; private set; }
        public float StartSliderValue { get; private set; }

        // Start is called before the first frame update
        void Start() {
            targetObject = gameObject.GetComponent<AppBarPlacer>().TargetBoundingBox.gameObject;

            targetObject.GetComponent<BoxCollider>().enabled = false;
            handler = targetObject.GetComponent<MenuHandler>();
            placementService = ServiceManager.GetService<MenuPlacementService>();
            if (handler.ConstantViewSizeEnabled) {
                slider.SetActive(true);
                slider.GetComponent<PinchSlider>().SliderValue = targetObject.GetComponent<ConstantViewSize>().TargetViewPercentV;
            }
            
        }

        private void Update() {
            if(placementService.PlacementMode != MenuPlacementService.MenuPlacementServiceMode.Adjustment) {
                targetObject.GetComponent<BoundingBoxStateController>().BoundingBoxActive = false;
                targetObject.GetComponent<BoxCollider>().enabled = false;
            }
        }


        /*public void AppBarInitialize(GameObject targetObject) {
            if (!targetObject.GetComponent<BoundingBoxStateController>()) {
                targetObject.AddComponent<BoundingBoxStateController>();
            }
            (targetObject.GetComponent<BoundingBox>() ?? targetObject.AddComponent<BoundingBox>()).BoundingBoxActivation = BoundingBox.BoundingBoxActivationType.ActivateManually;
            targetObject.GetComponent<BoundingBox>().CalculationMethod = BoundingBox.BoundsCalculationMethod.ColliderOnly;
            targetObject.GetComponent<BoundingBox>().Target = targetObject;
            if (targetObject.GetComponent<MenuHandler>().ConstantViewSizeEnabled) {
                targetObject.GetComponent<BoundingBox>().ShowScaleHandles = false;
            }
            if (!targetObject.GetComponent<MenuHandler>().AppBar) {
                targetObject.GetComponent<MenuHandler>().AppBar = Instantiate(ServiceManager.GetService<MenuPlacementService>().AppBar);
                targetObject.GetComponent<MenuHandler>().AppBar.GetComponent<AppBarPlacer>().TargetBoundingBox = targetObject.GetComponent<BoundingBox>();
            }
            else {
                targetObject.GetComponent<MenuHandler>().AppBar.SetActive(true);
            }
        }*/

        public void OnAppBarExpand() {
            ConstantViewSizeStartScale = targetObject.transform.localScale;
            slider.GetComponent<PinchSlider>().SliderValue = targetObject.GetComponent<ConstantViewSize>().TargetViewPercentV;
            StartSliderValue = slider.GetComponent<PinchSlider>().SliderValue;
            targetObject.GetComponent<BoxCollider>().enabled = true;
            placementService.EnterAdjustmentMode();
        }

        

        public void OnAppBarCollapse() {

            if (placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Manual) {
                targetObject.GetComponent<BoundingBox>().ShowScaleHandles = false;
                slider.SetActive(true);
            }

            targetObject.GetComponent<ObjectManipulator>().enabled = false;

            if (StartPosition != targetObject.transform.localPosition || StartRotation != targetObject.transform.localRotation || StartScale != targetObject.transform.localScale) {
                Tuple<Vector3, Quaternion, Vector3, float> lastOffsets = new Tuple<Vector3, Quaternion, Vector3, float>(StartPosition, StartRotation, StartScale, StartSliderValue);
                Tuple<Vector3, Quaternion, Vector3, float> newOffsets = new Tuple<Vector3, Quaternion, Vector3, float>(targetObject.transform.localPosition, targetObject.transform.localRotation, targetObject.transform.localScale, slider.GetComponent<PinchSlider>().SliderValue);
                handler.SaveOffsetBeforeManipulation(lastOffsets);
                handler.UpdateOffset(newOffsets, lastOffsets);            
            }
            targetObject.GetComponent<BoxCollider>().enabled = false;
            //Test
            targetObject.GetComponent<ObjectManipulator>().enabled = false;
            placementService.ExitAdjustmentMode();

        }

        public void Retrieve() {
            handler.Retrieve();
        }

        public void Close() {
            handler.Close();
            placementService.ExitAdjustmentMode();
            gameObject.GetComponent<AppBarStateController>().Collapse();
        }

        public void SwitchVariant() {
            if(placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Manual) {
                handler.ExitManualMode();
                if (handler.compact) {
                    message.switchType = PlacementMessage.SwitchType.CompactToFloating;
                }
                else {
                    message.switchType = PlacementMessage.SwitchType.FloatingToCompact;
                }
                placementService.UpdatePlacement(message, targetObject);
                message.switchType = PlacementMessage.SwitchType.NoSwitch;
            }
            else {
                Component suggestionPanel = Dialog.Open(placementService.SuggestionPanel, DialogButtonType.OK, "Not in manual mode", 
                    "You are currently not in manual mode, so you cannot switch to another variant. Please first switch to manual mode", true);
                suggestionPanel.gameObject.GetComponent<Follow>().MinDistance = 0.4f;
                suggestionPanel.gameObject.GetComponent<Follow>().MaxDistance = 0.4f;
                suggestionPanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.4f;
                suggestionPanel.gameObject.transform.forward = CameraCache.Main.transform.forward;
            }

        }

        public void OnSliderValueUpdate() {
            if(placementService.PlacementMode == MenuPlacementService.MenuPlacementServiceMode.Adjustment && slider.activeInHierarchy) {
                targetObject.GetComponent<ConstantViewSize>().TargetViewPercentV = slider.GetComponent<PinchSlider>().SliderValue;
                targetObject.transform.localScale = ConstantViewSizeStartScale * (slider.GetComponent<PinchSlider>().SliderValue / StartSliderValue);
            }
        }

        public void SwitchReferenceType() {
            if (placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Manual) {

                if (targetObject.transform.parent == null) {
                    targetObject.transform.parent = CameraCache.Main.transform;
                    icon.sprite = locked;
                }
                else {
                    targetObject.transform.parent = null;
                    icon.sprite = unlocked;
                }
            }
            else {
                Component suggestionPanel = Dialog.Open(placementService.SuggestionPanel, DialogButtonType.OK, "Not in manual mode",
                    "You are currently not in manual mode, so you cannot change the reference type. Please first switch to manual mode", true);
                suggestionPanel.gameObject.GetComponent<Follow>().MinDistance = 0.4f;
                suggestionPanel.gameObject.GetComponent<Follow>().MaxDistance = 0.4f;
                suggestionPanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.4f;
                suggestionPanel.gameObject.transform.forward = CameraCache.Main.transform.forward;
            }          
        }

        public void OnAdjustment() {
            if(placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Manual) {
                targetObject.GetComponent<BoundingBox>().ShowScaleHandles = true;
                slider.SetActive(false);
            }
            //For bounding box. If bounds control is applicable, the object menipulator can be removed.
            targetObject.GetComponent<ObjectManipulator>().enabled = true;            
            StartPosition = targetObject.transform.localPosition;
            StartRotation = targetObject.transform.localRotation;
            StartScale = targetObject.transform.localScale;
            slider.GetComponent<PinchSlider>().SliderValue = targetObject.GetComponent<ConstantViewSize>().TargetViewPercentV;
            //retrieved = false;
        }

        public void AdjustmentEnd() {
            
        }
    }
}

