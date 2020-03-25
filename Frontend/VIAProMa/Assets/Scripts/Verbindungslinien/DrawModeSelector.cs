using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

public class DrawModeSelector : MonoBehaviour
{
    [HideInInspector] public BoxCollider boxColl;
    ///[HideInInspector] public ConnectionLinesMenu linedrawscript;
    [HideInInspector] public WindowManager manager;

    private void Start()
    {
        boxColl = gameObject.GetComponent<BoxCollider>();
        manager = GameObject.FindWithTag("LineDraw").GetComponent<WindowManager>();
        ///linedrawscript = manager.GetComponent<ConnectionLinesMenu>();
    }
    void Update()
    {
        boxColl.enabled = manager.ConnectionLinesMenu.isLineModeActivated || manager.ConnectionLinesMenu.isDeleteLineModeActivated;
    }
}
