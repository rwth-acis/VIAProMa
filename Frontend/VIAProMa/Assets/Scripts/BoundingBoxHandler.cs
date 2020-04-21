using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using System;
using TMPro;

public class BoundingBoxHandler : MonoBehaviour
{
    [SerializeField] private GameObject adjustButtonCaption;
    [SerializeField] private GameObject boundingBoxObject;

    // Start is called before the first frame update
    void Start()
    {
        boundingBoxObject.SetActive(false);
    }

    public void Adjust()
    {

        if (boundingBoxObject.activeSelf)
        {
            adjustButtonCaption.GetComponent<TextMeshPro>().SetText("Adjust");
            boundingBoxObject.SetActive(false);
        }
        else
        {
            adjustButtonCaption.GetComponent<TextMeshPro>().SetText("Done");
            boundingBoxObject.SetActive(true);
        }
    }
}
