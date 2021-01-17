using UnityEngine.UI;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class VirtualTool : MonoBehaviour, IMixedRealityInputActionHandler
{
    public IMixedRealityInputSource inputSource;


    [SerializeField]
    [Tooltip("Input Action to handle")]
    public MixedRealityInputAction inputAction = MixedRealityInputAction.None;

    [SerializeField]
    [Tooltip("Whether input events should be marked as used after handling so other handlers in the same game object ignore them")]
    private bool markEventsAsUsed = false;

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

    public void SetupTool(InputActionUnityEvent OnInputActionStarted, InputActionUnityEvent OnInputActionEnded, InputActionUnityEvent OnToolCreated,
        InputActionUnityEvent OnToolDestroyed, MixedRealityInputAction inputAction, IMixedRealityInputSource inputSource, Sprite icon)
    {
        if (this.OnToolDestroyed != null)
        {
            this.OnToolDestroyed.Invoke(null);
        }

        this.OnInputActionStarted = OnInputActionStarted;
        this.OnInputActionEnded = OnInputActionEnded;
        this.OnToolCreated = OnToolCreated;
        this.OnToolDestroyed = OnToolDestroyed;
        this.inputAction = inputAction;
        this.inputSource = inputSource;
        GetComponentInChildren<Image>().sprite = icon;

        OnToolCreated.Invoke(null);
    }

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputActionHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
    }

    void IMixedRealityInputActionHandler.OnActionStarted(BaseInputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == inputAction && eventData.InputSource == inputSource && !eventData.used)
        {
            OnInputActionStarted.Invoke(eventData);
            if (markEventsAsUsed)
            {
                eventData.Use();
            }
        }
    }
    void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == inputAction && eventData.InputSource == inputSource && !eventData.used)
        {
            OnInputActionEnded.Invoke(eventData);
            if (markEventsAsUsed)
            {
                eventData.Use();
            }
        }
    }
}
