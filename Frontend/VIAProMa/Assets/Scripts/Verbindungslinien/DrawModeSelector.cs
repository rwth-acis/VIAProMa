using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

public class DrawModeSelector : MonoBehaviour
{
    private BoxCollider boxColl;
    private WindowManager manager;

    private void Start()
    {
        boxColl = gameObject.GetComponent<BoxCollider>();
        manager = GameObject.FindWithTag("LineDraw").GetComponent<WindowManager>();
    }
    void Update()
    {
        boxColl.enabled = manager.ConnectionLinesMenu.IsLineModeActivated || manager.ConnectionLinesMenu.IsDeleteLineModeActivated;
    }
}
