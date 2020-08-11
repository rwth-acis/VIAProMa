using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

public class DrawModeSelector : MonoBehaviour
{
    private BoxCollider boxColl;

    private void Start()
    {
        boxColl = gameObject.GetComponent<BoxCollider>();
    }
    void Update()
    {
        boxColl.enabled = WindowManager.Instance.ConnectionLinesMenu.IsLineModeActivated || WindowManager.Instance.ConnectionLinesMenu.IsDeleteLineModeActivated;
    }
}
