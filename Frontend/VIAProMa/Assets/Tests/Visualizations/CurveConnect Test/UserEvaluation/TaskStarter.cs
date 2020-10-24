using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Delets all Connection Curves and then creates a Connection Curve for each pair in the startObjects and goalObjects list, once the StartTask method gets called. Only used for test purposes.
/// </summary>
public class TaskStarter : MonoBehaviour
{
    public List<GameObject> startObjects;
    public List<GameObject> goalObjects;

    /// <summary>
    /// Delets all Connection Curves and then creates a Connection Curve for each pair in the startObjects and goalObjects list.
    /// </summary>
    public void StartTask()
    {
        ConnectionCurveManager.Instance.DeleteAllCurves();
        for (int i = 0; i < startObjects.Count; i++)
        {
            ConnectionCurveManager.Instance.CreateConnectionCurveScene(startObjects[i],goalObjects[i]);
        }
    }
}
