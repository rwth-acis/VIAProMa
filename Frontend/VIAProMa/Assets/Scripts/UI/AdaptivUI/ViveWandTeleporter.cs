using UnityEngine.UI;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using TMPro;

public class ViveWandTeleporter : MonoBehaviour, IMixedRealityInputHandler<float>
{
    //Events and action for the thrigger
    public MixedRealityInputAction gripPressAction;
    public float descriptionShowTime = 3;


    IMixedRealityInputSource ownSource;

    public string textGrip;
    public InputActionUnityEvent OnInputActionStartedGrip;
    public InputActionUnityEvent OnInputActionEndedGrip;

    public void SetupTool()
    {
        //Enable the button explain text
        GameObject menuButton = transform.Find("ButtonDescriptions").gameObject;
        menuButton.SetActive(true);

        SetText("GripText", "", textGrip);

        StopCoroutine("disableDescriptions");
        //Waits descriptionShowTime befor disabling the descriptions
        StartCoroutine("disableDescriptions");

    }

    private IEnumerator disableDescriptions()
    {
        yield return new WaitForSeconds(descriptionShowTime);
        GameObject menuButton = transform.Find("ButtonDescriptions").gameObject;
        menuButton.SetActive(false);
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

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityInputHandler<float>>(this);
        SetupTool();
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityInputHandler<float>>(this);
    }

    bool IsInputSourceThis(IMixedRealityInputSource inputSource)
    {
        return this == inputSource.Pointers[0]?.Controller?.Visualizer?.GameObjectProxy?.GetComponentInChildren<ViveWandVirtualTool>();
    }


    void IMixedRealityInputHandler<float>.OnInputChanged(InputEventData<float> eventData)
    {
        if (eventData.MixedRealityInputAction == gripPressAction)
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
