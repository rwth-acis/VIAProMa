using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class VirtualTool : MonoBehaviour, IMixedRealityInputActionHandler
{
    public IMixedRealityInputSource inputSource;


    [SerializeField]
    [Tooltip("Input Action to handle")]
    public MixedRealityInputAction InputAction = MixedRealityInputAction.None;

    [SerializeField]
    [Tooltip("Whether input events should be marked as used after handling so other handlers in the same game object ignore them")]
    private bool MarkEventsAsUsed = false;

    /// <summary>
    /// Unity event raised on action start, e.g. button pressed or gesture started. 
    /// Includes the input event that triggered the action.
    /// </summary>
    public InputActionUnityEvent OnInputActionStarted;

    /// <summary>
    /// Unity event raised on action end, e.g. button released or gesture completed.
    /// Includes the input event that triggered the action.
    /// </summary>
    public InputActionUnityEvent OnInputActionEnded;


    public InputActionUnityEvent OnToolCreated;

    public InputActionUnityEvent OnToolDestroyed;

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputActionHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
    }

    private void Start()
    {
        if (OnToolCreated != null)
        {
            OnToolCreated.Invoke(null);
        }
    }
    private void OnDestroy()
    {
        if (OnToolDestroyed != null)
        {
            OnToolDestroyed.Invoke(null);
        }
    }

    void IMixedRealityInputActionHandler.OnActionStarted(BaseInputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == InputAction && eventData.InputSource == inputSource && !eventData.used)
        {
            OnInputActionStarted.Invoke(eventData);
            if (MarkEventsAsUsed)
            {
                eventData.Use();
            }
        }
    }
    void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == InputAction && eventData.InputSource == inputSource && !eventData.used)
        {
            OnInputActionEnded.Invoke(eventData);
            if (MarkEventsAsUsed)
            {
                eventData.Use();
            }
        }
    }
}
