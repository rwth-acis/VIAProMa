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
    [SerializeField]
    Sprite icon;

    [SerializeField]
    public MixedRealityInputAction InputAction;

    public InputActionUnityEvent toolActionOnSelectStart;
    public InputActionUnityEvent toolActionOnSelectEnd;
}

public class VirtualToolEventData
{
    IMixedRealityInputSource invokingSource;
    GameObject target;
}

[System.Serializable]
public class VirtialToolEvent : UnityEvent<VirtualToolEventData> { }
