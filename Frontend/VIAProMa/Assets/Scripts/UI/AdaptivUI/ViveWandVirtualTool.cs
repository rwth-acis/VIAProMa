using UnityEngine.UI;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using System.Timers;
using System.Collections;
using TMPro;

public class ViveWandVirtualTool : MonoBehaviour, IMixedRealityInputActionHandler, IMixedRealityInputHandler<Vector2>
{
    //Events and action for the thrigger
    public MixedRealityInputAction triggerInputAction;
    public MixedRealityInputAction touchpadTouchActionAction;
    public MixedRealityInputAction touchpadPressAction;

    Vector2 thumbPosition;

    IMixedRealityInputSource ownSource;

    public MenuEntry currentEntry { get; private set; }

    [SerializeField]
    MenuEntry defaultEntry;

    private bool hoverActive = false;

    //Hover Actions
    GameObject oldFocusTarget;

    //Has to be global to prevent the garbage collection from collecting it before elapsed
    Timer timer;

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

        StopCoroutine("disableText");
        StartCoroutine("disableText");

        currentEntry = newEntry;
    }

    private IEnumerator disableText()
    {
        yield return new WaitForSeconds(3);
        GameObject menuButton = transform.Find("ButtonDescriptions").gameObject;
        menuButton.SetActive(false);
    }

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

    private void SetText(string gameobjectName, string text, string defaulText)
    {
        GameObject textGameobject = transform.Find("ButtonDescriptions/" + gameobjectName).gameObject;
        TMP_Text textMesh = textGameobject.GetComponentInChildren<TMP_Text>();
        textGameobject.SetActive(true);
        if (text != "")
        {
            textMesh.text = text;
        }
        else if (defaulText != "")
        {
            textMesh.text = defaulText;
        }
        else
        {
            textGameobject.SetActive(false);
        }
    }

    private void Update()
    {
        //Update the hover events
        if (ownSource == null)
        {
            //It can take a few frames until this method returns an InputSource, because they first have to register themself in the input system
            ownSource = GetOwnInputSource();
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
                    //Debug.Log("Hover Stop");
                }

                if (target != null)
                {
                    currentEntry.OnHoverOverTargetStart.Invoke(data);
                    //Debug.Log("Hover Start");
                    currentEntry.OnHoverOverTargetActive.Invoke(data);
                    //Debug.Log("Hover");
                }
            }
            else if (target != null)
            {
                currentEntry.OnHoverOverTargetActive.Invoke(data);
                //Debug.Log("Hover");
            }
            oldFocusTarget = target;
        }
    }

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputActionHandler>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<Vector2>>(this);
        SetupTool(defaultEntry);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputActionHandler>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<Vector2>>(this);
    }

    //Trigger on start
    void IMixedRealityInputActionHandler.OnActionStarted(BaseInputEventData eventData)
    {
        if (eventData.MixedRealityInputAction == triggerInputAction && IsInputSourceThis(eventData.InputSource) && !eventData.used)
        {
            currentEntry.OnInputActionStartedTrigger?.Invoke(eventData);
        }
    }
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
                float angle = Vector2.SignedAngle(Vector2.right, thumbPosition);
                if (angle > -45 && angle <= 45)
                {
                    //Right press
                    if (currentEntry.iconTouchpadRight != null)
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
                    if (currentEntry.iconTouchpadUp != null)
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
                    if (currentEntry.iconTouchpadLeft != null)
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
                    if (currentEntry.iconTouchpadDown != null)
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

    //Save the last known position of the thumb on the trackpad to use it when the trackpad is pressed
    void IMixedRealityInputHandler<Vector2>.OnInputChanged(InputEventData<Vector2> eventData)
    {
        if (eventData.MixedRealityInputAction == touchpadTouchActionAction)
        {
            thumbPosition = eventData.InputData;
        }
    }

    bool IsInputSourceThis(IMixedRealityInputSource inputSource)
    {
        return this == inputSource.Pointers[0].Controller.Visualizer.GameObjectProxy.GetComponentInChildren<ViveWandVirtualTool>();
    }

    IMixedRealityInputSource GetOwnInputSource()
    {
        foreach (var source in CoreServices.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            foreach (var pointer in source.Pointers)
            {
                if (pointer.Controller?.Visualizer?.GameObjectProxy == gameObject)
                {
                    return source;
                }
            }
        }
        Debug.LogError("Can't find the input source this tool belongs too");
        return null;
    }
}
