using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnFocus : BaseFocusHandler
{
    [SerializeField] Vector3 standardEulerRotation;
    [SerializeField] Vector3 focusedEulerRotation;

    public float damping = 10;

    Vector3 targetRotation;

    public override void OnFocusEnter(FocusEventData eventData)
    {
        base.OnFocusEnter(eventData);
        targetRotation = focusedEulerRotation;
    }

    public override void OnFocusExit(FocusEventData eventData)
    {
        base.OnFocusExit(eventData);
        targetRotation = standardEulerRotation;
    }

    private void Awake()
    {
        transform.localEulerAngles = standardEulerRotation;
        targetRotation = standardEulerRotation;
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * damping);
    }
}
