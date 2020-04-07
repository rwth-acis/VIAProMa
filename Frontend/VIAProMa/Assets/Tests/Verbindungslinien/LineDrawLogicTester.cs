using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawLogicTester : MonoBehaviour
{
    [SerializeField] private LineDrawLogic linedrawlogicscript;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            linedrawlogicscript.SwitchLineDrawMode();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            linedrawlogicscript.DeleteAllLines();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            linedrawlogicscript.DeleteSpecificLine();
        }
    }
}
