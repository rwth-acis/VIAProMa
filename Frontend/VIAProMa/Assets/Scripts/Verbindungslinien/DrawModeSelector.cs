using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;

public class DrawModeSelector : MonoBehaviour
{
    [HideInInspector] public BoxCollider boxColl;
    [HideInInspector] public LineDrawLogic linedrawscript;

    private void Start()
    {
        boxColl = gameObject.GetComponent<BoxCollider>();
        linedrawscript = GameObject.FindGameObjectWithTag("LineDraw").GetComponent<LineDrawLogic>();
    }
    void Update()
    {
        boxColl.enabled = linedrawscript.isLineModeActivated || linedrawscript.isDeleteLineModeActivated;
    }
}
