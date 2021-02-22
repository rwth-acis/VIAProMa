using UnityEngine.UI;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Timers;
using System.Collections;
using TMPro;

public class ViveWandVirtualTool : ViveWand, IMixedRealityInputActionHandler, IMixedRealityInputHandler<Vector2>, IMixedRealityInputHandler<float>
{
    //Events and action for the thrigger
    public MixedRealityInputAction triggerInputAction;
    public MixedRealityInputAction touchpadTouchActionAction;
    public MixedRealityInputAction touchpadPressAction;
    public MixedRealityInputAction gripPressAction;

    [SerializeField]
    MenuEntry defaultEntry;

    //The last recorded position on the touchpad
    Vector2 thumbPosition;

    public MenuEntry currentEntry { get; private set; }

    //Hover Actions
    GameObject oldFocusTarget;

    /// <summary>
    /// Set the new event handler, icons and desriciption texts for the new entry. Also activates the description texts for descriptionShowTime seconds.
    /// </summary>
    /// <param name="newEntry"></param> The new entry
    public void SetupTool(MenuEntry newEntry)
    {
        if (currentEntry.OnToolDestroyed != null)
        {
            currentEntry.OnToolDestroyed.Invoke(null);
        }

        //set the new icons
        SetIcon("ToolIconCanvas", newEntry.iconTool, defaultEntry.iconTool);
        SetIcon("TouchpadRightIcon", newEntry.iconTouchpadRight, defaultEntry.iconTouchpadRight);
        SetIcon("TouchpadUpIcon", newEntry.iconTouchpadUp, defaultEntry.iconTouchpadUp);
        SetIcon("TouchpadLeftIcon", newEntry.iconTouchpadLeft, defaultEntry.iconTouchpadLeft);
        SetIcon("TouchpadDownIcon", newEntry.iconTouchpadDown, defaultEntry.iconTouchpadDown);

        if (newEntry.OnToolCreated != null)
        {
            newEntry.OnToolCreated.Invoke(null);
        }

        //Enable the button explain text
        GameObject menuButton = transform.Find("ButtonDescriptions").gameObject;
        menuButton.SetActive(true);

        SetText("TouchpadRightText", newEntry.textTouchpadRight, defaultEntry.textTouchpadRight);
        SetText("TouchpadDownText", newEntry.textTouchpadDown, defaultEntry.textTouchpadDown);
        SetText("TouchpadLeftText", newEntry.textTouchpadLeft, defaultEntry.textTouchpadLeft);
        SetText("TouchpadUpText", newEntry.textTouchpadUp, defaultEntry.textTouchpadUp);
        SetText("TriggerText", newEntry.textTrigger, defaultEntry.textTrigger);
        SetText("GripText", newEntry.textGrip, defaultEntry.textGrip);

        StopCoroutine("DisableDescriptions");
        //Waits descriptionShowTime befor disabling the descriptions
        StartCoroutine("DisableDescriptions");

        currentEntry = newEntry;
    }

    /// <summary>
    /// Sets the icon on the coresponding canvas on the tool.
    /// </summary>
    /// <param name="canvasName"></param> The name of the canvas, on which the icon shoould be set
    /// <param name="icon"></param> The new Icon. If null, the defaut icon is used
    /// <param name="defaultIcon"></param> Only used, if icon is null
    private void SetIcon(string canvasName, Sprite icon, Sprite defaultIcon)
    {
        Transform iconCanvas = transform.Find(canvasName);
        iconCanvas.gameObject.SetActive(true);
        if (icon != null)
        {
            iconCanvas.GetComponentInChildren<Image>().sprite = icon;
        }
        else if (defaultIcon != null)
        {
            iconCanvas.GetComponentInChildren<Image>().sprite = defaultIcon;
        }
        else
        {
            iconCanvas.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        //Update the hover events
        if (ownSource == null)
        {
            //It can take a few frames until this method returns an InputSource, because they first have to register themself in the input system
            //ownSource = GetOwnInputSource();
        }
        else
        {

            FocusEventData data = new FocusEventData(UnityEngine.EventSystems.EventSystem.current);
            GameObject target = ownSource.Pointers[0].Result?.CurrentPointerTarget;
            data.Initialize(ownSource.Pointers[0], oldFocusTarget, target);

            if (target != oldFocusTarget)
            {
                if (oldFocusTarget != null)
                {
                    currentEntry.OnHoverOverTargetStop.Invoke(data);
                }

                if (target != null)
                {
                    currentEntry.OnHoverOverTargetStart.Invoke(data);
                    currentEntry.OnHoverOverTargetActive.Invoke(data);
                }
            }
            else if (target != null)
            {
                currentEntry.OnHoverOverTargetActive.Invoke(data);
            }
            oldFocusTarget = target;
        }
    }

    /// <summary>
    /// Registers the handlers in the input system. Otherwise, they will recive events only when a pointer has this object in focus.
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine("SetOwnSource");
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputActionHandler>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<Vector2>>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<float>>(this);
        SetupTool(defaultEntry);
    }

    /// <summary>
    /// Deregisters all handlers, otherwise it will recive events even after deactivcation.
    /// </summary>
    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<Vector2>>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<float>>(this);
    }

    /// <summary>
    /// Triggerd when an input action starts
    /// </summary>
    void IMixedRealityInputActionHandler.OnActionStarted(BaseInputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == triggerInputAction && IsInputSourceThis(eventData.InputSource) && !eventData.used)
        {
            //On Trigger event
            currentEntry.OnInputActionStartedTrigger?.Invoke(eventData);
        }
    }
    /// <summary>
    /// Triggerd when an input action ends. Used for the trigger and the touchpad.
    /// </summary>
    /// <param name="eventData"></param>
    void IMixedRealityInputActionHandler.OnActionEnded(BaseInputEventData eventData)
    {
        if (IsInputSourceThis(eventData.InputSource))
        {
            if (eventData.MixedRealityInputAction == triggerInputAction)
            {
                    currentEntry.OnInputActionEndedTrigger?.Invoke(eventData);
            }
            else if (eventData.MixedRealityInputAction == touchpadPressAction)
            {
                //Touchad
                float angle = Vector2.SignedAngle(Vector2.right, thumbPosition);
                if (angle > -45 && angle <= 45)
                {
                    //Right press
                    if (currentEntry.OnInputActionEndedTouchpadRight.GetPersistentEventCount() > 0)
                    {
                        currentEntry.OnInputActionEndedTouchpadRight?.Invoke(eventData);
                    }
                    else
                    {
                        defaultEntry.OnInputActionEndedTouchpadRight.Invoke(eventData);
                    }
                }
                else if (angle > 45 && angle <= 135)
                {
                    //Up press
                    if (currentEntry.OnInputActionEndedTouchpadUp.GetPersistentEventCount() > 0)
                    {
                        currentEntry.OnInputActionEndedTouchpadUp.Invoke(eventData);
                    }
                    else
                    {
                        defaultEntry.OnInputActionEndedTouchpadUp.Invoke(eventData);
                    }
                }
                else if ((angle > 135 && angle <= 180) || (angle >= -180 && angle <= -135))
                {
                    //Left press
                    if (currentEntry.OnInputActionEndedTouchpadLeft.GetPersistentEventCount() > 0)
                    {
                        currentEntry.OnInputActionEndedTouchpadLeft?.Invoke(eventData);
                    }
                    else
                    {
                        defaultEntry.OnInputActionEndedTouchpadLeft.Invoke(eventData);
                    }
                }
                else
                {
                    //Down press
                    if (currentEntry.OnInputActionEndedTouchpadDown.GetPersistentEventCount() > 0)
                    {
                        currentEntry.OnInputActionEndedTouchpadDown?.Invoke(eventData);
                    }
                    else
                    {
                        defaultEntry.OnInputActionEndedTouchpadDown.Invoke(eventData);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Save the last known position of the thumb on the trackpad to use it when the trackpad is pressed.
    /// </summary>
    void IMixedRealityInputHandler<Vector2>.OnInputChanged(InputEventData<Vector2> eventData)
    {
        if (eventData.MixedRealityInputAction == touchpadTouchActionAction)
        {
            thumbPosition = eventData.InputData;
        }
    }

    /// <summary>
    /// Triggerd when an input action of type float changes its value. Used for the grip button.
    /// </summary>
    /// <param name="eventData"></param>
    void IMixedRealityInputHandler<float>.OnInputChanged(InputEventData<float> eventData)
    {
        if (IsInputSourceThis(eventData.InputSource) && eventData.MixedRealityInputAction == gripPressAction)
        {
            if (eventData.InputData > 0.5)
            {
                if (currentEntry.OnInputActionStartedGrip.GetPersistentEventCount() > 0)
                {
                    currentEntry.OnInputActionStartedGrip.Invoke(eventData);
                }
                else
                {
                    defaultEntry.OnInputActionStartedGrip.Invoke(eventData);
                }
            }
            else
            {
                if (currentEntry.OnInputActionEndedGrip.GetPersistentEventCount() > 0)
                {
                    currentEntry.OnInputActionEndedGrip.Invoke(eventData);
                }
                else
                {
                    defaultEntry.OnInputActionEndedGrip.Invoke(eventData);
                }
            }
        }
    }
}
