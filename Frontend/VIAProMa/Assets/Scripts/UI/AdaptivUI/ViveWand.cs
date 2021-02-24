using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit;

public abstract class ViveWand : MonoBehaviour
{
    public float descriptionShowTime = 3;
    protected IMixedRealityInputSource ownSource;

    /// <summary>
    /// Disabel the description texts after descriptionShowTime seconds
    /// </summary>
    /// <returns></returns>
    protected IEnumerator DisableDescriptions()
    {
        yield return new WaitForSeconds(descriptionShowTime);
        GameObject menuButton = transform.Find("ButtonDescriptions").gameObject;
        menuButton.SetActive(false);
    }

    protected IEnumerator SetOwnSource()
    {
        while (ownSource == null)
        {
            ownSource = GetOwnInputSource();
            yield return null;
        }
    }

    /// <summary>
    /// Sets the description texts for the TMP that is attached to an GameObject with name gameobjectName.
    /// </summary>
    /// <param name="gameobjectName"></param> The name of the object, the TMP is attached to
    /// <param name="text"></param> The text to be set
    /// <param name="defaulText"></param> The default trext, that is used, if text is "".
    protected void SetText(string gameobjectName, string text, string defaulText)
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

    /// <summary>
    /// Is the provided input source the same as the object this belongs to?
    /// </summary>
    /// <param name="inputSource"></param>
    /// <returns></returns>
    protected bool IsInputSourceThis(IMixedRealityInputSource inputSource)
    {
        //return this == inputSource.Pointers[0]?.Controller?.Visualizer?.GameObjectProxy?.GetComponentInChildren<ViveWand>();
        return inputSource == ownSource;
    }

    /// <summary>
    /// Get the input source, this object belongs to. Can return null, when the input source isn't registerd yet.
    /// </summary>
    /// <returns></returns>
    protected IMixedRealityInputSource GetOwnInputSource()
    {
        foreach (var source in CoreServices.InputSystem.DetectedInputSources)
        {
            foreach (var pointer in source.Pointers)
            {
                if (pointer.Controller?.Visualizer?.GameObjectProxy == gameObject)
                {
                    return source;
                }
            }
        }
        return null;
    }
}
