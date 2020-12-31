using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using HoloToolkit.Unity;
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
}
