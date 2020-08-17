using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Priority_Queue;
using System.Threading.Tasks;

public class LineController : MonoBehaviour
{
    public static float stepSize = 1;
    public static int curveSegmentCount = 60;
    public GameObject DeleteCube;
    GameObject instantiatedDeletCube;
    List<ConnectionCurve> curves;
    IMixedRealityPointer mainPointer = null;
    JoinedCurveGeneration curveGenerator;
    public enum State
    {
        defaultMode,
        connecting,
        deleting
    }

    public State currState = State.defaultMode;

    //Temp Curve
    private ConnectionCurve tempCurve;
    DateTime clickTimeStamp;

    //Colour settings for the curves
    Gradient defaultColour;
    Gradient deletColour;

    //Test
    public GameObject startTest;
    public GameObject goalTest;
    GameObject tempGoal = null;


    // Start is called before the first frame update
    void Start()
    {
        curves = new List<ConnectionCurve>();
        curveGenerator = GetComponent<JoinedCurveGeneration>();

        //Setting up the colours
        Color c1 = Color.red;
        Color c2 = Color.red;

        float alpha = 1.0f;
        defaultColour = new Gradient();
        defaultColour.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0), new GradientColorKey(new Color(0.2f, 0.8f, 0.02f, 1f), 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );


        deletColour = new Gradient();
        deletColour.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, 0), new GradientColorKey(Color.yellow, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );

        //Test
        if (startTest != null && goalTest != null)
        {
            AddConnectionCurve(startTest,goalTest);
        }

        Task test = JoinedCurveGeneration.UpdateAsync(curves, stepSize);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case State.defaultMode:
                if (instantiatedDeletCube != null)
                    Destroy(instantiatedDeletCube);
                break;

            case State.connecting:
                if (instantiatedDeletCube != null)
                    Destroy(instantiatedDeletCube);
                GameObject target = null;
                //For some ungodly reasons objects from the mrtk behave strange when they should be null. They can then still be dereferenced and != null still yields true, but there content is useless.
                //But ToString then returns null.
                if (mainPointer.ToString() != "null")
                {
                    if (mainPointer.Result != null && mainPointer.Result.CurrentPointerTarget != null)
                    {
                        tempCurve.goal = mainPointer.Result.CurrentPointerTarget.transform.root.gameObject;
                    }
                    else
                    {
                        tempCurve.goal = tempGoal;
                        tempCurve.goal.transform.position = mainPointer.Position;
                    }
                    var cursor = (AnimatedCursor)mainPointer.BaseCursor;
                    if (cursor.CursorState == CursorStateEnum.Select && (DateTime.Now - clickTimeStamp).TotalMilliseconds > 30)
                    {
                        if (mainPointer.Result.CurrentPointerTarget != null)
                            target = mainPointer.Result.CurrentPointerTarget.transform.root.gameObject;
                        if (target != null)
                            AddConnectionCurve(tempCurve.start, target);
                        StopConnecting();
                    }
                }
                else
                {
                    StopConnecting();
                }
                break;

            case State.deleting:
                if (mainPointer.ToString() != "null")
                {
                    //Update Delete Cube transform
                    var ray = mainPointer.Rays[0];
                    instantiatedDeletCube.transform.position = ray.Origin + ray.Direction.normalized * 0.5f;
                    instantiatedDeletCube.transform.rotation = Quaternion.LookRotation(ray.Direction.normalized, Vector3.up);

                    //Necassrary because you shouldn't delete an object from a list, while iterating over it
                    List<ConnectionCurve> curvesToDelete = new List<ConnectionCurve>();
                    //Check which curves colide with the delete cube
                    foreach (ConnectionCurve connectionCurve in curves)
                    {
                        Vector3[] curve = new Vector3[connectionCurve.lineRenderer.positionCount];
                        connectionCurve.lineRenderer.GetPositions(curve);
                        if (CurveGenerator.CurveCollsionCheck(curve, connectionCurve.start, connectionCurve.goal, 0b100000000, false, 0.05f))
                        {
                            connectionCurve.lineRenderer.colorGradient = deletColour;
                            if (((AnimatedCursor)mainPointer.BaseCursor).CursorState == CursorStateEnum.Select)
                            {
                                curvesToDelete.Add(connectionCurve);
                            }
                        }
                        else
                        {
                            connectionCurve.lineRenderer.colorGradient = defaultColour;
                        }
                    }

                    foreach (ConnectionCurve connectionCurve in curvesToDelete)
                    {
                        Destroy(connectionCurve.lineRenderer);
                        curves.Remove(connectionCurve);
                    }
                }
                else
                {
                    Destroy(instantiatedDeletCube);
                    currState = State.defaultMode;
                }

                break;
        }
    }

    void AddConnectionCurve(GameObject start, GameObject goal)
    {
        curves.Add(new ConnectionCurve(start, goal, gameObject));
    }

    void StartConnecting(GameObject start)
    {
        currState = State.connecting;
        RefreshPointer();
        GameObject currentConnectingStart = start;
        tempGoal = new GameObject("Temp Goal");
        tempGoal.transform.position = mainPointer.Position;
        tempCurve = new ConnectionCurve(start, tempGoal, gameObject, Color.yellow, Color.yellow);
        curves.Add(tempCurve);
        clickTimeStamp = DateTime.Now;
    }


    void StopConnecting()
    {
        curves.Remove(tempCurve);
        Destroy(tempGoal);
        Destroy(tempCurve.lineRenderer);
        tempCurve = null;
        currState = State.defaultMode;
    }

    void StartDisconnecting()
    {
        if (currState != State.deleting)
        {
            currState = State.deleting;
            RefreshPointer();
            instantiatedDeletCube = Instantiate(DeleteCube); 
        }
    }

    void DeleteCurves(GameObject startOrEndPoint)
    {
        List<ConnectionCurve> curvesToDelete = new List<ConnectionCurve>();
        foreach (ConnectionCurve connectionCurve in curves)
        {
            if (connectionCurve.start == startOrEndPoint || connectionCurve.goal == startOrEndPoint)
            {
                curvesToDelete.Add(connectionCurve);
            }
        }
        foreach (ConnectionCurve connectionCurve in curvesToDelete)
        {
            Destroy(connectionCurve.lineRenderer);
            curves.Remove(connectionCurve);
        }
        
    }

    public void RefreshPointer()
    {
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
                        return;
                    }

                }
            }
        }
    }

   
}

public class ConnectionCurve
{
    public GameObject start { get; }
    public GameObject goal { get; set; }
    public LineRenderer lineRenderer { get; }
    public CoroutineData coroutineData;
    public Coroutine coroutine;

    public ConnectionCurve(GameObject start, GameObject goal, GameObject LineController) : this(start, goal, LineController, Color.green, Color.green) { }
    

    public ConnectionCurve(GameObject start, GameObject goal, GameObject LineController, Color color1, Color color2)
    {
        this.start = start;
        this.goal = goal;

        GameObject lineObject = new GameObject();
        lineObject.transform.parent = LineController.transform;
        lineRenderer = lineObject.AddComponent<LineRenderer>();

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.025f;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

        coroutineData = new CoroutineData();
    }
}

public class ConnectionCurveWrapper
{
    public ConnectionCurve curve { get; set; }
}
