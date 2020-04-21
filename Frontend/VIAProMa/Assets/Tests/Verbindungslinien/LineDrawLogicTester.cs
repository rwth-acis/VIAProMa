using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawLogicTester : MonoBehaviour
{
    [SerializeField] private ConnectionLinesMenu linedrawlogicscript;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            linedrawlogicscript.EnterLineDrawMode();
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            linedrawlogicscript.LeaveLineDrawMode();
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            linedrawlogicscript.DeleteAllLines();
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            linedrawlogicscript.DeleteSpecificLine();
        }
    }
}
