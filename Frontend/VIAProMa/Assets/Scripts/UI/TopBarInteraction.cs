
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TopBarInteraction : BaseFocusHandler, IMixedRealityInputHandler<MixedRealityPose>, IMixedRealityPointerHandler, IMixedRealitySourceStateHandler
{
    [SerializeField] private MixedRealityInputAction dragAction = MixedRealityInputAction.None;

    [SerializeField] private MixedRealityInputAction dragPositionAction = MixedRealityInputAction.None;

    [SerializeField] private Transform mainTransform;

    [SerializeField] private float LerpSpeed = 0.2f;

    private bool isDragging;
    private bool isDraggingEnabled = true;

    private Vector3 objectReferenceUp;
    private Vector3 objectReferenceForward;

    private Vector3 draggingPosition;


    private void Start()
    {
        if (mainTransform == null)
        {
            mainTransform = transform;
        }
    }

    private void OnDestroy()
    {
        if (isDragging)
        {
            StopDragging();
        }
    }



    void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData)
    {
        if (!isDraggingEnabled || !isDragging || eventData.MixedRealityInputAction != dragAction)
        {
            return;
        }
        eventData.Use();
        StopDragging();
    }

    void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (!isDraggingEnabled || isDragging || eventData.MixedRealityInputAction != dragAction)
        {
            return;
        }
        eventData.Use();


        FocusDetails focusDetails;
        Vector3 initialDraggingPosition = MixedRealityToolkit.InputSystem.FocusProvider.TryGetFocusDetails(eventData.Pointer, out focusDetails)
                ? focusDetails.Point
                : mainTransform.position;

        StartDragging(initialDraggingPosition);
    }

    void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData) { }



    void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

    public override void OnFocusExit(FocusEventData eventData)
    {
        if (isDragging)
        {
            StopDragging();
        }
    }

    public void SetDragging(bool isEnabled)
    {
        if (isDraggingEnabled == isEnabled)
        {
            return;
        }

        isDraggingEnabled = isEnabled;

        if (isDragging)
        {
            StopDragging();
        }
    }

    /// <summary>
    /// Starts dragging the object.
    /// </summary>
    private void StartDragging(Vector3 initialDraggingPosition)
    {
        if (!isDraggingEnabled || isDragging)
        {
            return;
        }

        eventData.Pointer.IsFocusLocked = true;
        isDragging = true;

        Vector3 inputPosition = currentPointer.Position;

        draggingPosition = initialDraggingPosition;
    }

    /// <summary>
    /// Stops dragging the object.
    /// </summary>
    private void StopDragging()
    {
        if (!isDragging)
        {
            return;
        }

        eventData.Pointer.IsFocusLocked = false;
        isDragging = false;

    }

    void IMixedRealityInputHandler<MixedRealityPose>.OnInputChanged(InputEventData<MixedRealityPose> eventData)
    {
        if (eventData.MixedRealityInputAction != dragPositionAction || !isDraggingEnabled || !isDragging)
        {
            return;
        }


        Vector3 pointerPosition = currentPointer.Position;

        Vector3 draggingPosition = pointerPosition;


        // Apply Final Position
        Vector3 newPosition = Vector3.Lerp(mainTransform.position, draggingPosition, LerpSpeed);
        mainTransform.position = newPosition;

 
    }
}
