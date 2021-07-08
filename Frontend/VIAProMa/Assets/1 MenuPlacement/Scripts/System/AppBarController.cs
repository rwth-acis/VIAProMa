﻿using i5.Toolkit.Core.ServiceCore;
using i5.VIAProMa.UI.AppBar;
using i5.VIAProMa.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MenuPlacement {
    public class AppBarController : MonoBehaviour {

        //The menu object
        private GameObject targetObject;
        private MenuHandler handler;
        private MenuPlacementService placementService;
        private bool retrieved = false;
        private PlacementMessage message = new PlacementMessage();
        public Vector3 StartPosition { get; private set; }
        public Quaternion StartRotation { get; private set; }
        public Vector3 StartScale { get; private set; }

        // Start is called before the first frame update
        void Start() {
            targetObject = gameObject.GetComponent<AppBarPlacer>().TargetBoundingBox.gameObject;

            targetObject.GetComponent<BoxCollider>().enabled = false;
            handler = targetObject.GetComponent<MenuHandler>();
            placementService = ServiceManager.GetService<MenuPlacementService>();
        }

        private void Update() {
            if(placementService.PlacementMode != MenuPlacementService.MenuPlacementServiceMode.Adjustment && targetObject.GetComponent<BoundingBoxStateController>().BoundingBoxActive) {
                targetObject.GetComponent<BoundingBoxStateController>().BoundingBoxActive = false;
                targetObject.GetComponent<BoxCollider>().enabled = false;
            }
        }

        public void OnAppBarExpand() {
            //Debug.Log(retrieved);
            StartPosition = targetObject.transform.localPosition;
            StartRotation = targetObject.transform.localRotation;
            StartScale = targetObject.transform.localScale;
            targetObject.GetComponent<BoxCollider>().enabled = true;
            placementService.EnterAdjustmentMode();
        }

        

        public void OnAppBarCollapse() {
            if (StartPosition != targetObject.transform.localPosition || StartRotation != targetObject.transform.localRotation || StartScale != targetObject.transform.localScale) {
                Tuple<Vector3, Quaternion, Vector3> lastTransform = new Tuple<Vector3, Quaternion, Vector3>(StartPosition, StartRotation, StartScale);
                Tuple<Vector3, Quaternion, Vector3> newTransform = new Tuple<Vector3, Quaternion, Vector3>(targetObject.transform.localPosition, targetObject.transform.localRotation, targetObject.transform.localScale);
                if (!retrieved) {
                    handler.SaveOffsetBeforeManipulation(newTransform, lastTransform);
                }
                else {
                    retrieved = false;
                } 
                handler.UpdateOffset(newTransform, lastTransform);            
            }
            targetObject.GetComponent<BoxCollider>().enabled = false;
            placementService.ExitAdjustmentMode();

        }

        public void Retrieve() {
            retrieved = true;
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
                if (handler.isCompact) {
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
                suggestionPanel.gameObject.GetComponent<Follow>().MinDistance = 0.2f;
                suggestionPanel.gameObject.GetComponent<Follow>().MaxDistance = 0.2f;
                suggestionPanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.2f;
            }

        }

        public void SwitchReferenceType() {
            if(placementService.PreviousMode == MenuPlacementService.MenuPlacementServiceMode.Manual) {
                
                if (targetObject.transform.parent == null) {
                    targetObject.transform.parent = CameraCache.Main.transform;
                }
                else {
                    targetObject.transform.parent = null;
                }
            }
            else {
                Component suggestionPanel = Dialog.Open(placementService.SuggestionPanel, DialogButtonType.OK, "Not in manual mode", 
                    "You are currently not in manual mode, so you cannot change the reference type. Please first switch to manual mode", true);
                suggestionPanel.gameObject.GetComponent<Follow>().MinDistance = 0.2f;
                suggestionPanel.gameObject.GetComponent<Follow>().MaxDistance = 0.2f;
                suggestionPanel.gameObject.GetComponent<Follow>().DefaultDistance = 0.2f;
            }
        }

        public void OnAdjustment() {
            StartPosition = targetObject.transform.localPosition;
            StartRotation = targetObject.transform.localRotation;
            StartScale = targetObject.transform.localScale;
            retrieved = false;
        }
    }
}

