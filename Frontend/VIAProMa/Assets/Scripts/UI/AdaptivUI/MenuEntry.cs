using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine.Events;

/// <summary>
/// Contains everything needed to define a virtual tool
/// </summary>
[Serializable]
public struct MenuEntry
{
    public Sprite iconTool;
   
    public string textTrigger;
    public InputActionUnityEvent OnInputActionStartedTrigger;
    public InputActionUnityEvent OnInputActionEndedTrigger;

    public string textGrip;
    public InputActionUnityEvent OnInputActionStartedGrip;
    public InputActionUnityEvent OnInputActionEndedGrip;

    //Touchpad Parameters
    public string textTouchpadRight;
    public Sprite iconTouchpadRight;
    public InputActionUnityEvent OnInputActionEndedTouchpadRight;

    public string textTouchpadLeft;
    public Sprite iconTouchpadLeft;
    public InputActionUnityEvent OnInputActionEndedTouchpadLeft;

    public string textTouchpadUp;
    public Sprite iconTouchpadUp;
    public InputActionUnityEvent OnInputActionEndedTouchpadUp;

    public string textTouchpadDown;
    public Sprite iconTouchpadDown;
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
