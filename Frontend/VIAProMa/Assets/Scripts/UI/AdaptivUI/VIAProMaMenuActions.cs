using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using HoloToolkit.Unity;
using Photon.Pun;
public class VIAProMaMenuActions : MonoBehaviour
{
    public void TestAction(VirtualToolEventData data)
    {
        Debug.Log("--------------Jo----------------");
    }

    public void DoubleSize(BaseInputEventData eventData)
    {
        GameObject target = eventData.InputSource.Pointers[0]?.Result?.CurrentPointerTarget;
        if (target != null)
        {
            target.transform.root.localScale *= 2;
        }
    }

    public void HalveeSize(BaseInputEventData eventData)
    {
        GameObject target = eventData.InputSource.Pointers[0]?.Result?.CurrentPointerTarget;
        if (target != null)
        {
            target.transform.root.localScale /= 2;
        }
    }

    public void Remove(BaseInputEventData eventData)
    {
        GameObject target = GetVisualisationFromInputSource(eventData.InputSource);
        if (target == null)
        {
            return;
        }
        if (target.GetComponentInChildren<PhotonView>() != null)
        {
            PhotonNetwork.Destroy(target);
        }
        else
        {
            Destroy(target);
        }
    }

    GameObject GetVisualisationFromInputSource(IMixedRealityInputSource source)
    {
        foreach (var pointer in source.Pointers)
        {
            GameObject target = pointer.Result?.CurrentPointerTarget;
            if (target != null)
            {
                while (target != null && !IsVisualisation(target))
                {
                    target = target.transform.parent?.gameObject;
                }
                if (target != null && IsVisualisation(target))
                {
                    return target;
                }
            }
        }
        return null;
    }

    bool IsVisualisation(GameObject gameObject)
    {
        return gameObject.GetComponent<Visualization>() != null;
    }
}
