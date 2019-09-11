// Refer to lines 22, 131, 168 and the Update() Method.
// Many thanks to @newske who posted a partial solution on stackoverflow:
// https://stackoverflow.com/questions/56846672/how-can-i-configure-mrtk-to-work-with-touch-input-in-editor-and-on-mobile-device?noredirect=1#comment100693647_56846672


// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Input.UnityInput
{
    [MixedRealityController(
        SupportedControllerType.TouchScreen,
        new[] { Handedness.Any })]
    public class UnityTouchController : BaseController
    {
        public UnityTouchController(TrackingState trackingState, Handedness controllerHandedness, IMixedRealityInputSource inputSource = null, MixedRealityInteractionMapping[] interactions = null)
                : base(trackingState, controllerHandedness, inputSource, interactions)
        {
        }

        // @JS: Changed because the original 30f will mean pointerclicked never gets triggered.
        private const float K_CONTACT_EPSILON = 0.003f;

        /// <summary>
        /// Time in seconds to determine if the contact registers as a tap or a hold
        /// </summary>
        public float MaxTapContactTime { get; set; } = 0.5f;

        /// <summary>
        /// The threshold a finger must move before starting a manipulation gesture.
        /// </summary>
        public float ManipulationThreshold { get; set; } = 5f;

        /// <summary>
        /// Current Touch Data for the Controller.
        /// </summary>
        public Touch TouchData { get; internal set; }

        /// <summary>
        /// Current Screen point ray for the Touch.
        /// </summary>
        public Ray ScreenPointRay { get; internal set; }

        /// <summary>
        /// The current lifetime of the Touch.
        /// </summary>
        public float Lifetime { get; private set; } = 0.0f;

        /// <inheritdoc />
        public override MixedRealityInteractionMapping[] DefaultInteractions { get; } =
        {
            new MixedRealityInteractionMapping(0, "Touch Pointer Delta", AxisType.DualAxis, DeviceInputType.PointerPosition, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(1, "Touch Pointer Position", AxisType.SixDof, DeviceInputType.SpatialPointer, MixedRealityInputAction.None),
            new MixedRealityInteractionMapping(2, "Touch Press", AxisType.Digital, DeviceInputType.PointerClick, MixedRealityInputAction.None),
        };

        private bool isTouched;
        private MixedRealityInputAction holdingAction;
        private bool isHolding;
        private MixedRealityInputAction manipulationAction;
        private bool isManipulating;
        private MixedRealityPose lastPose = MixedRealityPose.ZeroIdentity;

        /// <inheritdoc />
        public override void SetupDefaultInteractions(Handedness controllerHandedness)
        {
            AssignControllerMappings(DefaultInteractions);
            if (InputSystem?.InputSystemProfile.GesturesProfile != null)
            {
                for (int i = 0; i < InputSystem.InputSystemProfile.GesturesProfile.Gestures.Length; i++)
                {
                    var gesture = InputSystem.InputSystemProfile.GesturesProfile.Gestures[i];

                    switch (gesture.GestureType)
                    {
                        case GestureInputType.Hold:
                            holdingAction = gesture.Action;
                            break;
                        case GestureInputType.Manipulation:
                            manipulationAction = gesture.Action;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Start the touch.
        /// </summary>
        public void StartTouch()
        {
            InputSystem?.RaisePointerDown(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction);
            isTouched = true;
            InputSystem?.RaiseGestureStarted(this, holdingAction);
            isHolding = true;
        }

        // @JS: Not yet sure why we need startPending, was added by newske
        private int startPending = 0;

        /// <summary>
        /// Update the touch data.
        /// </summary>
        public void Update()
        {
            if (!isTouched) { return; }

            if (startPending == 2)
            {
                StartTouch();
            }

            startPending++;

            Lifetime += Time.deltaTime;

            if (TouchData.phase == TouchPhase.Moved)
            {
                Interactions[0].Vector2Data = TouchData.deltaPosition;

                if (Interactions[0].Changed)
                {
                    InputSystem?.RaisePositionInputChanged(InputSource, ControllerHandedness, Interactions[0].MixedRealityInputAction, TouchData.deltaPosition);
                }

                lastPose.Position = InputSource.Pointers[0].BaseCursor.Position;
                lastPose.Rotation = InputSource.Pointers[0].BaseCursor.Rotation;
                InputSystem?.RaiseSourcePoseChanged(InputSource, this, lastPose);

                // @JS - To use with manipulationhandlers we must call RaisePointerDragged.
                InputSystem?.RaisePointerDragged(InputSource.Pointers[0], Interactions[1].MixedRealityInputAction);

                Interactions[1].PoseData = lastPose;

                if (Interactions[1].Changed)
                {
                    InputSystem?.RaisePoseInputChanged(InputSource, ControllerHandedness, Interactions[1].MixedRealityInputAction, lastPose);
                }

                if (!isManipulating)
                {
                    if (Mathf.Abs(TouchData.deltaPosition.x) > ManipulationThreshold ||
                        Mathf.Abs(TouchData.deltaPosition.y) > ManipulationThreshold)
                    {
                        InputSystem?.RaiseGestureCanceled(this, holdingAction);
                        isHolding = false;

                        InputSystem?.RaiseGestureStarted(this, manipulationAction);
                        isManipulating = true;
                    }
                }
                else
                {
                    InputSystem?.RaiseGestureUpdated(this, manipulationAction, TouchData.deltaPosition);
                }
            }
        }

        /// <summary>
        /// End the touch.
        /// </summary>
        public void EndTouch()
        {
            startPending = 0;
            if (TouchData.phase == TouchPhase.Ended)
            {
                if (Lifetime < K_CONTACT_EPSILON)
                {
                    if (isHolding)
                    {
                        InputSystem?.RaiseGestureCanceled(this, holdingAction);
                        isHolding = false;
                    }

                    if (isManipulating)
                    {
                        InputSystem?.RaiseGestureCanceled(this, manipulationAction);
                        isManipulating = false;
                    }
                } else if (Lifetime < MaxTapContactTime)
                {
                    if (isHolding)
                    {
                        InputSystem?.RaiseGestureCanceled(this, holdingAction);
                        isHolding = false;
                    }

                    if (isManipulating)
                    {
                        InputSystem?.RaiseGestureCanceled(this, manipulationAction);
                        isManipulating = false;
                    }

                    InputSystem?.RaisePointerClicked(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction, TouchData.tapCount);
                }

                if (isHolding)
                {
                    InputSystem?.RaiseGestureCompleted(this, holdingAction);
                    isHolding = false;
                }

                if (isManipulating)
                {
                    InputSystem?.RaiseGestureCompleted(this, manipulationAction, TouchData.deltaPosition);
                    isManipulating = false;
                }
            }

            if (isHolding)
            {
                InputSystem?.RaiseGestureCompleted(this, holdingAction);
                isHolding = false;
            }

            Debug.Assert(!isHolding);

            if (isManipulating)
            {
                InputSystem?.RaiseGestureCompleted(this, manipulationAction, TouchData.deltaPosition);
                isManipulating = false;
            }

            Debug.Assert(!isManipulating);

            InputSystem?.RaisePointerUp(InputSource.Pointers[0], Interactions[2].MixedRealityInputAction);

            Lifetime = 0.0f;
            isTouched = false;
            Interactions[1].PoseData = MixedRealityPose.ZeroIdentity;
            Interactions[0].Vector2Data = Vector2.zero;
        }
    }
}