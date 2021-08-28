﻿using i5.Toolkit.Core.ServiceCore;
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
    /// This script extends the functionalities of the App Bar from VIAProMa
    /// If more functionalities are required, it is recommanded to write another script inherits this script or an additional script added on the App Bar object.
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

        public float StartSliderValue { get; private set; }

        // Start is called before the first frame update
        void Start() {
            targetObject = gameObject.GetComponent<AppBarPlacer>().TargetBoundingBox.gameObject;

            targetObject.GetComponent<BoxCollider>().enabled = false;
            handler = targetObject.GetComponent<MenuHandler>();
            placementService = ServiceManager.GetService<MenuPlacementService>();
            //Just for BoundingBox. When BoundsControl is applicable, the object menipulator can be removed.
            if (!targetObject.GetComponent<ObjectManipulator>()) {
                targetObject.AddComponent<ObjectManipulator>();
            }
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

        public void OnAppBarExpand() {
            StartPosition = targetObject.transform.localPosition;
            StartRotation = targetObject.transform.localRotation;
            StartScale = targetObject.transform.localScale;
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
            StartPosition = targetObject.transform.localPosition;
            StartRotation = targetObject.transform.localRotation;
            StartScale = targetObject.transform.localScale;
            slider.GetComponent<PinchSlider>().SliderValue = targetObject.GetComponent<ConstantViewSize>().TargetViewPercentV;
            StartSliderValue = slider.GetComponent<PinchSlider>().SliderValue;
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
                targetObject.transform.localScale = StartScale * (slider.GetComponent<PinchSlider>().SliderValue / StartSliderValue);
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
            targetObject.GetComponent<ObjectManipulator>().enabled = true;            
            StartPosition = targetObject.transform.localPosition;
            StartRotation = targetObject.transform.localRotation;
            StartScale = targetObject.transform.localScale;
            slider.GetComponent<PinchSlider>().SliderValue = targetObject.GetComponent<ConstantViewSize>().TargetViewPercentV;
            StartSliderValue = slider.GetComponent<PinchSlider>().SliderValue;
        }

        public void AdjustmentEnd() {
            
        }
    }
}

