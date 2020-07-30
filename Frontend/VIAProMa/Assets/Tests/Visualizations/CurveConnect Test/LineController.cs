using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;

public class LineController : MonoBehaviour, IMixedRealityPointerHandler
{
    public static float stepSize = 1;
    public static int curveSegmentCount = 60;
    List<ConnectionCurve> curves;
    IMixedRealityPointer mainPointer = null;
    SimulatedHandData handData;
    public bool connecting = false;

    //Test
    public GameObject startTest;
    public GameObject goalTest;
    private GameObject currentConnectingStart;
    DateTime clickTimeStamp;

    // Start is called before the first frame update
    void Start()
    {
        curves = new List<ConnectionCurve>();

        //Test
        if (startTest != null && goalTest != null)
        {
            GameObject[] test = new GameObject[] {startTest,goalTest};
            //AddConnectionCurve(test);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(ConnectionCurve connectionCurve in curves)
        {
            Vector3[] curve = JoinedCurveGeneration.start(connectionCurve.start, connectionCurve.goal, stepSize, curveSegmentCount);
            connectionCurve.lineRenderer.positionCount = curve.Length;
            connectionCurve.lineRenderer.SetPositions(curve);
        }

        if (connecting)
        {
            GameObject target = null;
            if (mainPointer != null)
            {
                
                var cursor = (AnimatedCursor)mainPointer.BaseCursor;
                if (cursor.CursorState == CursorStateEnum.Select && (DateTime.Now - clickTimeStamp).TotalMilliseconds > 30)
                {
                    target = mainPointer.Result.CurrentPointerTarget.transform.root.gameObject;
                    if(target != null)
                        AddConnectionCurve(currentConnectingStart, target);
                    connecting = false;
                }
            }
            else
            {
                connecting = false;
            }
        }
    }

    void AddConnectionCurve(GameObject start, GameObject goal)
    {
        curves.Add(new ConnectionCurve(start, goal, this.gameObject));
    }

    void StartConnecting(GameObject start)
    {
        connecting = true;
        foreach (var source in MixedRealityToolkit.InputSystem.DetectedInputSources)
        {
            // Ignore anything that is not a hand because we want articulated hands
            if (source.SourceType == InputSourceType.Hand)
            {
                foreach (var p in source.Pointers)
                {
                    if (p is IMixedRealityNearPointer)
                    {
                        // Ignore near pointers, we only want the rays
                        continue;
                    }
                    if (p.Result != null)
                    {
                        mainPointer = p;
                        currentConnectingStart = start;
                        clickTimeStamp = DateTime.Now;
                    }

                }
            }
        }
    }


    void IMixedRealityPointerHandler.OnPointerDown(MixedRealityPointerEventData eventData) { }

    void IMixedRealityPointerHandler.OnPointerDragged(MixedRealityPointerEventData eventData) { }

    void IMixedRealityPointerHandler.OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        Debug.Log("Jo");
    }
    void IMixedRealityPointerHandler.OnPointerUp(MixedRealityPointerEventData eventData) { }
}

public class ConnectionCurve
{
    public GameObject start { get; }
    public GameObject goal { get; }
    public LineRenderer lineRenderer { get; }

    public ConnectionCurve(GameObject start, GameObject goal, GameObject LineController)
    {
        this.start = start;
        this.goal = goal;

        GameObject lineObject = new GameObject();
        lineObject.transform.parent = LineController.transform;
        lineRenderer = lineObject.AddComponent<LineRenderer>();

        Color c1 = Color.yellow;
        Color c2 = Color.red;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.1f;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

    }
}
