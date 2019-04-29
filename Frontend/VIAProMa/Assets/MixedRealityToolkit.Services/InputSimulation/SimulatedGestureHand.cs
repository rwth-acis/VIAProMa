﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input
{
    [MixedRealityController(
        SupportedControllerType.GGVHand,
        new[] { Handedness.Left, Handedness.Right })]
    public class SimulatedGestureHand : SimulatedHand
    {
        public override HandSimulationMode SimulationMode => HandSimulationMode.Gestures;

        private MixedRealityInputAction holdAction = MixedRealityInputAction.None;
        private MixedRealityInputAction navigationAction = MixedRealityInputAction.None;
        private MixedRealityInputAction manipulationAction = MixedRealityInputAction.None;
        private bool useRailsNavigation = false;
        float holdStartDuration = 0.0f;
        float manipulationStartThreshold = 0.0f;

        private float SelectDownStartTime = 0.0f;
        private bool manipulationInProgress = false;
        private bool holdInProgress = false;
        private Vector3 currentRailsUsed = Vector3.one;
        private Vector3 currentPosition = Vector3.zero;
        private Vector3 lastPosition = Vector3.zero;
        private Vector3 cumulativeDelta = Vector3.zero;
        private MixedRealityPose currentGripPose = MixedRealityPose.ZeroIdentity;

        private Vector3 navigationDelta => new Vector3(
            Mathf.Clamp(cumulativeDelta.x, -1.0f, 1.0f) * currentRailsUsed.x,
            Mathf.Clamp(cumulativeDelta.y, -1.0f, 1.0f) * currentRailsUsed.y,
            Mathf.Clamp(cumulativeDelta.z, -1.0f, 1.0f) * currentRailsUsed.z);

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackingState"></param>
        /// <param name="controllerHandedness"></param>
        /// <param name="inputSource"></param>
        /// <param name="interactions"></param>
        public SimulatedGestureHand(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        void Awake()
        { 
            var gestureProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.GesturesProfile;
            if (gestureProfile != null)
            {
                for (int i = 0; i < gestureProfile.Gestures.Length; i++)
                {
                    var gesture = gestureProfile.Gestures[i];
                    switch (gesture.GestureType)
                    {
                        case GestureInputType.Hold:
                            holdAction = gesture.Action;
                            break;
                        case GestureInputType.Manipulation:
                            manipulationAction = gesture.Action;
                            break;
                        case GestureInputType.Navigation:
                            navigationAction = gesture.Action;
                            break;
                    }
                }

                useRailsNavigation = gestureProfile.UseRailsNavigation;
            }

            var inputSimProfile = MixedRealityToolkit.Instance?.GetService<InputSimulationService>()?.GetInputSimulationProfile();
            if (inputSimProfile != null)
            {
                holdStartDuration = inputSimProfile.HoldStartDuration;
                manipulationStartThreshold = inputSimProfile.ManipulationStartThreshold;
            }
        }

        /// <summary>
        /// The GGV default interactions.
        /// </summary>
        /// <remarks>A single interaction mapping works for both left and right controllers.</remarks>
        public override MixedRealityInteractionMapping[] DefaultInteractions => new[]
        {
            new MixedRealityInteractionMapping(0, "Select", AxisType.Digital, DeviceInputType.Select, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Grip Pose", AxisType.SixDof, DeviceInputType.SpatialGrip, MixedRealityInputAction.None),
        };

        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
        }

        protected override void UpdateInteractions(SimulatedHandData handData)
        {
            lastPosition = currentPosition;
            currentPosition = jointPositions[(int)TrackedHandJoint.IndexTip];
            currentGripPose.Position = currentPosition;
            if (lastPosition != currentPosition)
            {
                MixedRealityToolkit.InputSystem?.RaiseSourcePositionChanged(InputSource, this, currentPosition);
            }

            for (int i = 0; i < Interactions?.Length; i++)
            {

                switch (Interactions[i].InputType)
                {
                    case DeviceInputType.SpatialGrip:
                        Interactions[i].PoseData = currentGripPose;
                        if (Interactions[i].Changed)
                        {
                            MixedRealityToolkit.InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction, currentGripPose);
                        }
                        break;
                    case DeviceInputType.Select:
                        Interactions[i].BoolData = handData.IsPinching;

                        if (Interactions[i].Changed)
                        {
                            if (Interactions[i].BoolData)
                            {
                                MixedRealityToolkit.InputSystem?.RaiseOnInputDown(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);

                                SelectDownStartTime = Time.time;
                                cumulativeDelta = Vector3.zero;
                            }
                            else
                            {
                                MixedRealityToolkit.InputSystem?.RaiseOnInputUp(InputSource, ControllerHandedness, Interactions[i].MixedRealityInputAction);

                                // Stop gestures
                                CompleteHoldGesture();
                                CompleteManipulationNavigationGesture();
                            }
                        }
                        else if (Interactions[i].BoolData)
                        {
                            // For convenience of simulating in Unity Editor, make the ray use the index
                            // finger position instead of knuckle, since the index finger doesn't move when we press.
                            Vector3 newPosition = jointPositions[(int)TrackedHandJoint.IndexTip];
                            cumulativeDelta += newPosition - currentPosition;
                            currentPosition = newPosition;

                            if (!manipulationInProgress)
                            {
                                if (cumulativeDelta.magnitude > manipulationStartThreshold)
                                {
                                    CancelHoldGesture();
                                    StartManipulationNavigationGesture();
                                }
                                else if (!holdInProgress)
                                {
                                    float time = Time.time;
                                    if (time >= SelectDownStartTime + holdStartDuration)
                                    {
                                        StartHoldGesture();
                                    }
                                }
                            }
                            else
                            {
                                UpdateManipulationNavigationGesture();
                            }
                        }
                        break;
                }
            }
        }

        private void StartHoldGesture()
        {
            if (!holdInProgress)
            {
                MixedRealityToolkit.InputSystem?.RaiseGestureStarted(this, holdAction);
                holdInProgress = true;
            }
        }

        private void CompleteHoldGesture()
        {
            if (holdInProgress)
            {
                MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, holdAction);
                holdInProgress = false;
            }
        }

        private void CancelHoldGesture()
        {
            if (holdInProgress)
            {
                MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, holdAction);
                holdInProgress = false;
            }
        }

        private void StartManipulationNavigationGesture()
        {
            if (!manipulationInProgress)
            {
                MixedRealityToolkit.InputSystem?.RaiseGestureStarted(this, manipulationAction);
                MixedRealityToolkit.InputSystem?.RaiseGestureStarted(this, navigationAction);
                manipulationInProgress = true;

                currentRailsUsed = Vector3.one;
                UpdateNavigationRails();
            }
        }

        private void UpdateManipulationNavigationGesture()
        {
            MixedRealityToolkit.InputSystem?.RaiseGestureUpdated(this, manipulationAction, cumulativeDelta);

            UpdateNavigationRails();
            MixedRealityToolkit.InputSystem?.RaiseGestureUpdated(this, navigationAction, navigationDelta);
        }

        private void CompleteManipulationNavigationGesture()
        {
            if (manipulationInProgress)
            {
                MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, manipulationAction, cumulativeDelta);
                MixedRealityToolkit.InputSystem?.RaiseGestureCompleted(this, navigationAction, navigationDelta);
                manipulationInProgress = false;
            }
        }

        private void CancelManipulationNavigationGesture()
        {
            if (manipulationInProgress)
            {
                MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, manipulationAction);
                MixedRealityToolkit.InputSystem?.RaiseGestureCanceled(this, navigationAction);
                manipulationInProgress = false;
            }
        }

        // If rails are used, test the delta for largest component and limit navigation to that axis
        private void UpdateNavigationRails()
        {
            if (useRailsNavigation && currentRailsUsed == Vector3.one)
            {
                if (Mathf.Abs(cumulativeDelta.x) >= manipulationStartThreshold)
                {
                    currentRailsUsed = new Vector3(1, 0, 0);
                }
                else if (Mathf.Abs(cumulativeDelta.y) > manipulationStartThreshold)
                {
                    currentRailsUsed = new Vector3(0, 1, 0);
                }
                else if (Mathf.Abs(cumulativeDelta.z) > manipulationStartThreshold)
                {
                    currentRailsUsed = new Vector3(0, 0, 1);
                }
            }
        }
    }
}