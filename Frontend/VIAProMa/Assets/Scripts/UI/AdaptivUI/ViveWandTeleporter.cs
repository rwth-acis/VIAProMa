using UnityEngine.UI;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using TMPro;

public class ViveWandTeleporter : ViveWand, IMixedRealityInputHandler<float>
{
    //Events and action for the thrigger
    public MixedRealityInputAction gripPressAction;

    public string textGrip;
    public InputActionUnityEvent OnInputActionStartedGrip;
    public InputActionUnityEvent OnInputActionEndedGrip;

    /// <summary>
    /// Activates the description texts for descriptionShowTime seconds.
    /// </summary>
    public void SetupTool()
    {
        //Enable the button explain text
        GameObject menuButton = transform.Find("ButtonDescriptions").gameObject;
        menuButton.SetActive(true);

        SetText("GripText", "", textGrip);

        StopCoroutine("DisableDescriptions");
        //Waits descriptionShowTime befor disabling the descriptions
        StartCoroutine("DisableDescriptions");

    }

    /// <summary>
    /// Registers the handlers in the input system. Otherwise, they will recive events only when a pointer has this object in focus.
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine("SetOwnSource");
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<float>>(this);
        SetupTool();
    }

    /// <summary>
    /// Deregisters all handlers, otherwise it will recive events even after deactivcation.
    /// </summary>
    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<float>>(this);
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
                OnInputActionStartedGrip.Invoke(eventData);
            }
            else
            {
                OnInputActionEndedGrip.Invoke(eventData);
            }
        }
    }
}
