using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Priority_Queue;

public class LineController : MonoBehaviour
{
    public static float stepSize = 1;
    public static int curveSegmentCount = 60;
    public GameObject DeleteCube;
    GameObject instantiatedDeletCube;
    List<ConnectionCurve> curves;
    IMixedRealityPointer mainPointer = null;
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


    // Start is called before the first frame update
    void Start()
    {
        curves = new List<ConnectionCurve>();

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
    }

    // Update is called once per frame
    void Update()
    {
        //Update Curves
        foreach (ConnectionCurve connectionCurve in curves)
        {
            AStarParameter result = JoinedCurveGeneration.start(connectionCurve.start, connectionCurve.goal, stepSize, curveSegmentCount);
            if (result.curve != null)
            {
                Vector3[] curve = result.curve;
                connectionCurve.lineRenderer.positionCount = curve.Length;
                connectionCurve.lineRenderer.SetPositions(curve);
            }
            else
            {
                if (connectionCurve.currentCoroutine == null)
                {
                    
                    StartCoroutine("AStarSearchCoroutine", result);
                    connectionCurve.currentCoroutine = result;
                }
                if(!connectionCurve.currentCoroutine.isRunning)
                {
                    Vector3[] curve = CurveGenerator.IntTripleArrayToCurve(connectionCurve.currentCoroutine.output.path, connectionCurve.start.transform.position, connectionCurve.goal.transform.position, stepSize);
                    connectionCurve.lineRenderer.positionCount = curve.Length;
                    connectionCurve.lineRenderer.SetPositions(curve);
                    connectionCurve.currentCoroutine = null;
                }
            }
        }

        //Update the tempcurve if existing
        if (tempCurve != null)
        {
            //Vector3[] curve = JoinedCurveGeneration.start(tempCurve.start, tempCurve.goal, stepSize, curveSegmentCount);
            //tempCurve.lineRenderer.positionCount = curve.Length;
            //tempCurve.lineRenderer.SetPositions(curve);
        }
        

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
                    tempCurve.goal.transform.position = mainPointer.Position;
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
                        if (CurveGenerator.CurveCollsionCheck(curve, connectionCurve.start, connectionCurve.goal, 0b100000000, false))
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
        curves.Add(new ConnectionCurve(start, goal, this.gameObject));
    }

    void StartConnecting(GameObject start)
    {
        currState = State.connecting;
        RefreshPointer();
        GameObject currentConnectingStart = start;
        GameObject tempGoal = new GameObject("Temp Goal");
        GameObject tempBox = new GameObject("Bounding Box");
        tempBox.transform.parent = tempGoal.transform;
        BoxCollider collider = tempBox.AddComponent<BoxCollider>();
        collider.name = "Bounding Box";
        collider.enabled = false;
        tempGoal.transform.position = mainPointer.Position;
        tempCurve = new ConnectionCurve(start, tempGoal, this.gameObject);
        clickTimeStamp = DateTime.Now;
    }


    void StopConnecting()
    {
        Destroy(tempCurve.goal);
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

    public IEnumerator AStarSearchCoroutine(AStarParameter parameter)
    {
        SimplePriorityQueue<IntTriple> openSet = new SimplePriorityQueue<IntTriple>();
        Dictionary<IntTriple, IntTriple> cameFrom = new Dictionary<IntTriple, IntTriple>();
        Dictionary<IntTriple, float> gScore = new Dictionary<IntTriple, float>();
        openSet.Enqueue(parameter.start, 0);
        gScore.Add(parameter.start, parameter.Heuristic(parameter.start));
        IntTriple current;
        parameter.isRunning = true;
        DateTime timeAtBeginOfFrame = DateTime.Now;
        int frameCount = 0;

        while (openSet.Count != 0)
        {
            if ((DateTime.Now - timeAtBeginOfFrame).TotalMilliseconds > 5)
            {
                Debug.Log("Next Frame");
                frameCount++;
                if (frameCount > 5)
                {
                    Debug.Log("Fuck this");
                    yield break;
                    
                }
                yield return null;
                timeAtBeginOfFrame = DateTime.Now;
            }

            current = openSet.Dequeue();
            if (parameter.GoalTest(current, parameter.goal))
            {
                List<IntTriple> optimalPath = null;
                if (parameter.calculatePath)
                    optimalPath = AStar.reconstruct_path<IntTriple>(cameFrom, current);

                parameter.output = new AStar.SearchResult<IntTriple>(optimalPath, gScore[current]);
                parameter.isRunning = false;
                yield break;
            }

            List<IntTriple> neighbors = parameter.GetNeighbors(current);

            //IntTripleODO Maby here multithreading?
            foreach (IntTriple neighbor in neighbors)
            {
                float h = parameter.Heuristic(neighbor);
                float tentative_gScore = gScore[current] + parameter.CostsBetween(current, neighbor);
                float neighboreGScore;
                if (gScore.TryGetValue(neighbor, out neighboreGScore))
                {
                    if (tentative_gScore < neighboreGScore)
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentative_gScore;
                        openSet.EnqueueWithoutDuplicates(neighbor, neighboreGScore + h);
                    }
                }
                //if neighbore dosn't have a gScore then it's infinit and therefore bigger than tentative_gScore
                else
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentative_gScore;
                    openSet.EnqueueWithoutDuplicates(neighbor, tentative_gScore + h);
                }
            }
        }
        if (openSet.Count == 0)
        {
            //open set is empty and goal is never reached => no possible path
            parameter.output = new AStar.SearchResult<IntTriple>(new List<IntTriple>(), float.PositiveInfinity);
            parameter.isRunning = false;
            yield break;
        }
        parameter.output = new AStar.SearchResult<IntTriple>(null, float.PositiveInfinity);
        parameter.isRunning = false;
    }
}

public class ConnectionCurve
{
    public GameObject start { get; }
    public GameObject goal { get; }
    public LineRenderer lineRenderer { get; }
    public AStarParameter currentCoroutine;

    public ConnectionCurve(GameObject start, GameObject goal, GameObject LineController)
    {
        this.start = start;
        this.goal = goal;

        GameObject lineObject = new GameObject();
        lineObject.transform.parent = LineController.transform;
        lineRenderer = lineObject.AddComponent<LineRenderer>();

        Color c1 = Color.green;
        Color c2 = Color.green;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.025f;

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;
    }
}
