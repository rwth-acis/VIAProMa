using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawLogicTester : MonoBehaviour
{
    private ConnectionLinesMenu linedrawlogicscript;

    private void Start()
    {
        WindowManager.Instance.ConnectionLinesMenu.Open(new Vector3(0,0,1.5f), this.transform.eulerAngles);
    }

    // Update is called once per frame
    void Update()
    {
        if(linedrawlogicscript == null)
        {
            linedrawlogicscript = WindowManager.Instance.GetComponentInChildren<ConnectionLinesMenu>();
        }

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
