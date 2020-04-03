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

    [SerializeField] private BoundingBox boundingBox;
    [SerializeField] private Collider collider;
    [SerializeField] private GameObject adjustButtonCaption;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
        boundingBox = GetComponent<BoundingBox>();
        boundingBox.Active = false;
    }

    // Update is called once per frame
    public void Adjust()
    {
        if (boundingBox.Active)
        {
            adjustButtonCaption.GetComponent<TextMeshPro>().SetText("Adjust");
        }
        else
        {
            adjustButtonCaption.GetComponent<TextMeshPro>().SetText("Done");
        }
        collider.enabled = !collider.enabled;
        boundingBox.Active = !boundingBox.Active;
    }
}
