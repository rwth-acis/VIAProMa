using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnFocus : BaseFocusHandler
{
    [SerializeField] private Vector3 standardEulerRotation;
    [SerializeField] private Vector3 focusedEulerRotation;

    public float damping = 10;

    private Vector3 targetRotation;

    private void Awake()
    {
        transform.localEulerAngles = standardEulerRotation;
        targetRotation = standardEulerRotation;
    }

    private void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRotation), Time.deltaTime * damping);
    }

    public override void OnFocusEnter(FocusEventData eventData)
    {
        base.OnFocusEnter(eventData);
        ToFocusedRotation();
    }

    public override void OnFocusExit(FocusEventData eventData)
    {
        base.OnFocusExit(eventData);
        ToStandardRotation();
    }

    public void ToStandardRotation()
    {
        targetRotation = standardEulerRotation;
    }

    public void ToFocusedRotation()
    {
        targetRotation = focusedEulerRotation;
    }
}
