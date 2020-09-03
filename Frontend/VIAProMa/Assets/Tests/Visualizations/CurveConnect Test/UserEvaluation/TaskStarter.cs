using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskStarter : MonoBehaviour
{
    public List<GameObject> startObject;
    public List<GameObject> goalObject;

    // Start is called before the first frame update
    public void StartTask()
    {
        LineController controller = GameObject.FindObjectOfType<LineController>();
        controller.DeleteAllCurves();
        for (int i = 0; i < startObject.Count; i++)
        {
            controller.CreateConnectionCurveScene(startObject[i],goalObject[i]);
        }
    }
}
