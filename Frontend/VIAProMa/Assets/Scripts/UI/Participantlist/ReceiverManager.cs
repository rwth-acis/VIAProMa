using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
//using Microsoft.MixedReality.Toolkit.Utilities.Editor.Search;
using Microsoft.MixedReality.Toolkit.Input;
using TMPro;
using Photon.Realtime;

public class ReceiverManager : MonoBehaviour, IMixedRealityPointerHandler
{
    public TextMeshPro TextObject;

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerDragged(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        string username = TextObject.text;
    }
}
