﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
#if UNITY_WSA
#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA.Input;
#else
using UnityEngine.VR.WSA.Input;
#endif
#endif

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Waits for a controller to be instantiated, then attaches itself to a specified element
    /// </summary>
    public class AttachToController : ControllerFinder
    {
        public bool SetChildrenInactiveWhenDetached = true;

        [SerializeField]
        protected Vector3 PositionOffset = Vector3.zero;

        [SerializeField]
        protected Vector3 RotationOffset = Vector3.zero;

        [SerializeField]
        protected Vector3 ScaleOffset = Vector3.one;

        [SerializeField]
        protected bool SetScaleOnAttach = false;

        public bool IsAttached { get { return transform.parent == null; } }

        protected virtual void OnAttachToController() { }
        protected virtual void OnDetachFromController() { }

        protected override void OnEnable()
        {
            SetChildrenActive(false);

            base.OnEnable();
        }

        protected override void OnControllerFound()
        {
            // Parent ourselves under the element and set our offsets
            transform.parent = ElementTransform;
            transform.localPosition = PositionOffset;
            transform.localEulerAngles = RotationOffset;

            if (SetScaleOnAttach)
            {
                transform.localScale = ScaleOffset;
            }

            SetChildrenActive(true);

            // Announce that we're attached
            OnAttachToController();
        }

        protected override void OnControllerLost()
        {
            OnDetachFromController();

            SetChildrenActive(false);

            transform.parent = null;
        }

        private void SetChildrenActive(bool isActive)
        {
            if (SetChildrenInactiveWhenDetached)
            {
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(isActive);
                }
            }
        }

        protected void Reset()
        {
            // We want the default value of Handedness of Controller finders to be Unknown so it doesn't attach to random object.
            // But we also want the Editor to start with a useful default, so we set a Left handedness on inspector reset.
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
            Handedness = InteractionSourceHandedness.Left;
#endif
        }
    }
}