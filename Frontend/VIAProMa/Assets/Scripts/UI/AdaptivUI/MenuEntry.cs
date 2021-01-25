using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.Events;

//public class MenuEntry : MonoBehaviour
//{
//    [SerializeField]
//    Sprite icon;

//    [SerializeField]
//    Func<GameObject, IMixedRealityInputSource, int> toolAction;
//}

[Serializable]
public struct MenuEntry
{
    public Sprite iconTool;
    public Sprite iconTouchpadRight;
    public Sprite iconTouchpadUp;
    public Sprite iconTouchpadLeft;
    public Sprite iconTouchpadDown;

    public InputActionUnityEvent OnInputActionStartedTrigger;
    public InputActionUnityEvent OnInputActionEndedTrigger;

    public InputActionUnityEvent OnInputActionEndedTouchpadRight;
    public InputActionUnityEvent OnInputActionEndedTouchpadLeft;
    public InputActionUnityEvent OnInputActionEndedTouchpadUp;
    public InputActionUnityEvent OnInputActionEndedTouchpadDown;

    //Tool specific events
    public InputActionUnityEvent OnToolCreated;
    public InputActionUnityEvent OnToolDestroyed;
    public VirtualToolFocusEvent OnHoverOverTargetStart;
    public VirtualToolFocusEvent OnHoverOverTargetActive;
    public VirtualToolFocusEvent OnHoverOverTargetStop;
}

[Serializable]
public class VirtualToolFocusEvent : UnityEvent<FocusEventData> { }
