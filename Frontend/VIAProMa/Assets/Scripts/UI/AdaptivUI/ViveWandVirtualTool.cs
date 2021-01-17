using UnityEngine.UI;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public class ViveWandVirtualTool : MonoBehaviour, IMixedRealityInputActionHandler, IMixedRealityInputHandler<Vector2>
{
    [SerializeField]
    [Tooltip("Input Action to handle")]
    public MixedRealityInputAction triggerAction;

    [SerializeField]
    [Tooltip("Two Axis Input Action to handle")]
    public MixedRealityInputAction twoAxisInputAction;

    [SerializeField]
    [Tooltip("Two Axis Input Action to handle")]
    public MixedRealityInputAction TouchpadPressAction;

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

    Vector2 thumbPosition;

    public void SetupTool(InputActionUnityEvent OnInputActionStarted, InputActionUnityEvent OnInputActionEnded, InputActionUnityEvent OnToolCreated,
        InputActionUnityEvent OnToolDestroyed, MixedRealityInputAction inputAction, Sprite icon)
    {
        if (this.OnToolDestroyed != null)
        {
            this.OnToolDestroyed.Invoke(null);
        }

        this.OnInputActionStarted = OnInputActionStarted;
        this.OnInputActionEnded = OnInputActionEnded;
        this.OnToolCreated = OnToolCreated;
        this.OnToolDestroyed = OnToolDestroyed;
        this.triggerAction = inputAction;
        GetComponentInChildren<Image>().sprite = icon;

        OnToolCreated.Invoke(null);
    }

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputActionHandler>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<Vector2>>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<Vector2>>(this);
    }

    void IMixedRealityInputActionHandler.OnActionStarted(BaseInputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == triggerAction && IsInputSourceThis(eventData.InputSource) && !eventData.used)
        {
            OnInputActionStarted.Invoke(eventData);
        }
    }
    void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
    {
        if (IsInputSourceThis(eventData.InputSource))
        {
            if (eventData.MixedRealityInputAction == triggerAction)
            {
                OnInputActionEnded.Invoke(eventData);
            }
            else if (eventData.MixedRealityInputAction == TouchpadPressAction)
            {
                Debug.Log(thumbPosition);
            }
        }
    }

    void IMixedRealityInputHandler<Vector2>.OnInputChanged(InputEventData<Vector2> eventData)
    {
        if (eventData.MixedRealityInputAction == twoAxisInputAction)
        {
            thumbPosition = eventData.InputData;
        }
    }

    bool IsInputSourceThis(IMixedRealityInputSource inputSource)
    {
        return this == inputSource.Pointers[0].Controller.Visualizer.GameObjectProxy.GetComponentInChildren<ViveWandVirtualTool>();
    }
}
