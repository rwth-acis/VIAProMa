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
    [SerializeField] private TextMeshPro adjustButtonCaption;
    [SerializeField] private GameObject boundingBox;

    // Start is called before the first frame update
    void Start()
    {
        boundingBox.SetActive(false);
    }

    public void Adjust()
    {
        bool isActive = boundingBox.activeSelf;
        if (isActive)
        {
            adjustButtonCaption.SetText("Adjust");
            boundingBox.SetActive(false);
        }
        else
        {
            adjustButtonCaption.SetText("Done");
            boundingBox.SetActive(true);
        }
    }
}
