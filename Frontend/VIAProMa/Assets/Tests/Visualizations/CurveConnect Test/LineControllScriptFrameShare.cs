using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using static IntTriple;

public class LineControllScriptFrameShare : MonoBehaviour
{
    public float stepSize = 1;
    public GameObject startObject;
    public GameObject goalObject;
    public Color c1 = Color.yellow;
    public Color c2 = Color.red;
    public int maxProcessingTimePerFrame = 25;
    public bool testMode = true;

    //For distinguishing between random objects and start/goal
    List<GameObject> startGoalObjectsWithCollider;

    BoxCollider startBoundingBox;
    BoxCollider goalBoundingBox;

    GameObject boundContainerStart;
    GameObject boundContainerEnd;

    //TODO delet
    public GameObject[] testObject;

    enum PathAlgorithm
    {
        AStar,
        Greedy,
        HPA
    }

    // Start is called before the first frame update
    void Start()
    {
        //Test Stuff

        
        //

        //Main line renderer
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.widthMultiplier = 0.1f;

        

        float alpha = 1.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(c1, 0.0f), new GradientColorKey(c2, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );
        lineRenderer.colorGradient = gradient;

        
        //Line renderer for the test scenarious
        GameObject lineRendererAStarObject = new GameObject();
        LineRenderer lineRendererAStar = lineRendererAStarObject.AddComponent<LineRenderer>();
        lineRendererAStar.widthMultiplier = 0.1f;

        GameObject lineRendererGreedyObject = new GameObject();
        LineRenderer lineRendererGreedy = lineRendererGreedyObject.AddComponent<LineRenderer>();
        lineRendererGreedy.widthMultiplier = 0.1f;

            
        GameObject lineRendererHPAStarObject = new GameObject();
        LineRenderer lineRendererHPAStar = lineRendererHPAStarObject.AddComponent<LineRenderer>();
        lineRendererHPAStar.widthMultiplier = 0.1f;
            

        GameObject lineRendererGreedyRefObject = new GameObject();
        LineRenderer lineRendererGreedyRef = lineRendererGreedyRefObject.AddComponent<LineRenderer>();
        lineRendererGreedyRef.widthMultiplier = 0.1f;

        //Get the colliders from all child objects of start and goal
        List<Collider> startGoalCollider = new List<Collider>();
        startGoalCollider.AddRange(startObject.GetComponentsInChildren<Collider>());
        startGoalCollider.AddRange(goalObject.GetComponentsInChildren<Collider>());

        //Convert to game objects
        startGoalObjectsWithCollider = new List<GameObject>();
        foreach (Collider col in startGoalCollider)
        {
            startGoalObjectsWithCollider.Add(col.gameObject);
        }

        startBoundingBox = startObject.transform.Find("Bounding Box").gameObject.GetComponent<BoxCollider>();
        goalBoundingBox = goalObject.transform.Find("Bounding Box").gameObject.GetComponent<BoxCollider>();

        boundContainerStart = new GameObject();
        boundContainerStart.transform.parent = startObject.transform;
        boundContainerStart.transform.localPosition = Vector3.zero;
        boundContainerStart.layer = 6;
        boundContainerStart.name = "Hallu";

        boundContainerStart.AddComponent<BoxCollider>();
        BoxCollider boundingboxOnOtherLayer = boundContainerStart.GetComponent<BoxCollider>();
        boundingboxOnOtherLayer.name = "CurveConnectBoundingBox";
        boundingboxOnOtherLayer.size = startBoundingBox.size;
        boundingboxOnOtherLayer.center = startBoundingBox.center;


        boundContainerEnd = new GameObject();
        boundContainerEnd.transform.parent = goalObject.transform;
        boundContainerEnd.transform.localPosition = Vector3.zero;
        boundContainerEnd.layer = 6;
        boundContainerEnd.name = "Hallu";

        boundContainerEnd.AddComponent<BoxCollider>();
        BoxCollider boundingboxOnOtherLayerEnd = boundContainerEnd.GetComponent<BoxCollider>();
        boundingboxOnOtherLayerEnd.name = "Bruhh";
        boundingboxOnOtherLayerEnd.size = goalBoundingBox.size;
        boundingboxOnOtherLayerEnd.center = goalBoundingBox.center;

        if (testMode)
        {
        System.IO.StreamWriter aStar = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\AStar.csv");
        aStar.WriteLine("Obsacle Count; Start Goal distance; Optimal Path length; Time; Path length;Path colliions;Path curvature");
        System.IO.StreamWriter greedy = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\Greedy.csv");
        greedy.WriteLine("Obsacle Count; Start Goal distance; Optimal Path length; Time; Path length;Path colliions;Path curvature");
        System.IO.StreamWriter hpa = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\Hpa.csv");
        hpa.WriteLine("Obsacle Count; Start Goal distance; Optimal Path length; Time; Path length;Path colliions;Path curvature");
        System.IO.StreamWriter greedyRef = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\GreedyRef.csv");
        greedyRef.WriteLine("Obsacle Count; Start Goal distance; Optimal Path length; Time; Path length;Path colliions;Path curvature");
        System.IO.StreamWriter simple = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\Simple.csv");
        simple.WriteLine("Obsacle Count; Start Goal distance; Optimal Path length; Time; Path length;Path colliions;Path curvature");

        for (int testN = 0; testN < 5; testN++)
        {
            int obstacelCount = TestCaseGenerator.GenerateTestcase(startObject, goalObject);
            float startGoalDistance = Vector3.Distance(startObject.transform.position, goalObject.transform.position);

            //Test Cases
            Debug.Log("Start Goal distance:" + Vector3.Distance(startObject.transform.position, goalObject.transform.position));


            //Calculate the nearly optimal path:
            float stepSizeOpti = 1f;
            IntTriple startCellOpti = VectorToCell(startObject.transform.position, stepSizeOpti);
            IntTriple goalCellOpti = VectorToCell(goalObject.transform.position, stepSizeOpti);
            List<IntTriple> linePathCell = AStar.AStarSearch<IntTriple>(startCellOpti, goalCellOpti, GetNeighborsGenerator(stepSizeOpti), (x, y) => x == y, HeuristicGenerator(goalObject.transform.position, stepSizeOpti), CostsBetweenGenerator(0.5f)).path;
            Vector3[] lineVectorArray = new Vector3[linePathCell.Count + 2];
            lineVectorArray[0] = goalObject.transform.position;
            for (int i = 1; i < linePathCell.Count + 1; i++)
            {
                lineVectorArray[i] = CellToVector(linePathCell[i - 1], stepSizeOpti);
            }
            lineVectorArray[linePathCell.Count + 1] = startObject.transform.position;

            Debug.Log("OPtimal Path length:" + Curve.CurveLength(lineVectorArray));

            float optimal = Curve.CurveLength(lineVectorArray);


            DateTime startTime = DateTime.Now;

            //AStar
            Debug.Log("AStar:");

            IntTriple startCell = VectorToCell(startObject.transform.position, stepSize);
            IntTriple goalCell = VectorToCell(goalObject.transform.position, stepSize);

            //List<Vector3> linePath = A_Star(startObject, goalObject);
            AStar.AStarResult<IntTriple> result = AStar.AStarSearch<IntTriple>(startCell, goalCell, GetNeighborsGenerator(stepSize), (x, y) => x == y, HeuristicGenerator(goalObject.transform.position, stepSize), CostsBetweenGenerator(stepSize));
            linePathCell = result.path;


            lineVectorArray = new Vector3[linePathCell.Count + 2];
            lineVectorArray[0] = goalObject.transform.position;
            for (int i = 1; i < linePathCell.Count + 1; i++)
            {
                lineVectorArray[i] = CellToVector(linePathCell[i - 1], stepSize);
            }
            lineVectorArray[linePathCell.Count + 1] = startObject.transform.position;

            lineRendererAStar.positionCount = lineVectorArray.Length;
            lineRendererAStar.SetPositions(lineVectorArray);

            Debug.Log("Time: " + (DateTime.Now - startTime).TotalMilliseconds);
            Debug.Log("Path length: " + Curve.CurveLength(lineVectorArray));
            Debug.Log("Path colliions: " + Curve.CurveCollsionCount(lineVectorArray));
            Debug.Log("Path curvature: " + Curve.MaximalCurveAngel(lineVectorArray));

            aStar.WriteLine(obstacelCount + ";" + startGoalDistance + ";" + optimal + ";" + (DateTime.Now - startTime).TotalMilliseconds + ";" + Curve.CurveLength(lineVectorArray) + ";" + Curve.CurveCollsionCount(lineVectorArray) + ";" + Curve.MaximalCurveAngel(lineVectorArray));
                
            //Greedy
            Debug.Log("Greedy");
            startTime = DateTime.Now;

            result = Greedy.GreedySearch<IntTriple>(startCell, goalCell, GetNeighborsGenerator(stepSize), (x, y) => x == y, HeuristicGenerator(goalObject.transform.position, stepSize), CostsBetweenGenerator(stepSize));
            linePathCell = result.path;


            lineVectorArray = new Vector3[linePathCell.Count + 2];
            lineVectorArray[0] = startObject.transform.position;
            for (int i = 1; i < linePathCell.Count + 1; i++)
            {
                lineVectorArray[i] = CellToVector(linePathCell[i - 1], stepSize);
            }
            lineVectorArray[linePathCell.Count + 1] = goalObject.transform.position;


            lineRendererGreedy.positionCount = lineVectorArray.Length;
            lineRendererGreedy.SetPositions(lineVectorArray);
            Debug.Log("Time: " + (DateTime.Now - startTime).TotalMilliseconds);
            Debug.Log("Path Length:" + Curve.CurveLength(lineVectorArray));
            Debug.Log("Path colliions: " + Curve.CurveCollsionCount(lineVectorArray));
            Debug.Log("Path curvature: " + Curve.MaximalCurveAngel(lineVectorArray));

            greedy.WriteLine(obstacelCount + ";" + startGoalDistance + ";" + optimal + ";" + (DateTime.Now - startTime).TotalMilliseconds + ";" + Curve.CurveLength(lineVectorArray) + ";" + Curve.CurveCollsionCount(lineVectorArray) + ";" + Curve.MaximalCurveAngel(lineVectorArray));
            //HPAStar


            Debug.Log("HPA");
            startTime = DateTime.Now;
            List<Vector3> linePathVector3 = HPAStar.HPAStarSearch(startObject.transform.position, goalObject.transform.position, stepSize, 5);
            lineRendererHPAStar.positionCount = linePathVector3.Count;
            lineRendererHPAStar.SetPositions(linePathVector3.ToArray());
            Debug.Log((DateTime.Now - startTime).TotalMilliseconds);
            Debug.Log("Path Length:" + Curve.CurveLength(linePathVector3.ToArray()));
            Debug.Log("Path colliions: " + Curve.CurveCollsionCount(linePathVector3.ToArray()));
            Debug.Log("Path curvature: " + Curve.MaximalCurveAngel(linePathVector3.ToArray()));
            hpa.WriteLine(obstacelCount + ";" + startGoalDistance + ";" + optimal + ";" + (DateTime.Now - startTime).TotalMilliseconds + ";" + Curve.CurveLength(linePathVector3.ToArray()) + ";" + Curve.CurveCollsionCount(linePathVector3.ToArray()) + ";" + Curve.MaximalCurveAngel(linePathVector3.ToArray()));

            //Greedy refined
            Debug.Log("Greedy Refined");
            startTime = DateTime.Now;

            result = Greedy.GreedySearch<IntTriple>(startCell, goalCell, GetNeighborsGenerator(stepSize), (x, y) => x == y, HeuristicGenerator(goalObject.transform.position, stepSize), CostsBetweenGenerator(stepSize));
            linePathCell = result.path;


            lineVectorArray = new Vector3[linePathCell.Count + 2];
            lineVectorArray[0] = startObject.transform.position;
            for (int i = 1; i < linePathCell.Count + 1; i++)
            {
                lineVectorArray[i] = CellToVector(linePathCell[i - 1], stepSize);
            }
            lineVectorArray[linePathCell.Count + 1] = goalObject.transform.position;

            lineVectorArray = Greedy.postProcessing(lineVectorArray, 3);
            lineVectorArray = SimpleCurveGerneration.start(startObject.transform.position, goalObject.transform.position);

            lineRendererGreedyRef.positionCount = lineVectorArray.Length;
            lineRendererGreedyRef.SetPositions(lineVectorArray);
            Debug.Log("Time: " + (DateTime.Now - startTime).TotalMilliseconds);
            Debug.Log("Path Length:" + Curve.CurveLength(lineVectorArray));
            Debug.Log("Path colliions: " + Curve.CurveCollsionCount(lineVectorArray));
            Debug.Log("Path curvature: " + Curve.MaximalCurveAngel(lineVectorArray));
            greedyRef.WriteLine(obstacelCount + ";" + startGoalDistance + ";" + optimal + ";" + (DateTime.Now - startTime).TotalMilliseconds + ";" + Curve.CurveLength(lineVectorArray) + ";" + Curve.CurveCollsionCount(lineVectorArray) + ";" + Curve.MaximalCurveAngel(lineVectorArray));

            //Simple
            Debug.Log("Simple");
            startTime = DateTime.Now;
            lineVectorArray = SimpleCurveGerneration.startContinous(startObject.transform.position, goalObject.transform.position);
            Debug.Log("Time: " + (DateTime.Now - startTime).TotalMilliseconds);
            Debug.Log("Path Length:" + Curve.CurveLength(lineVectorArray));
            Debug.Log("Path colliions: " + Curve.CurveCollsionCount(lineVectorArray));
            Debug.Log("Path curvature: " + Curve.MaximalCurveAngel(lineVectorArray));
            simple.WriteLine(obstacelCount + ";" + startGoalDistance + ";" + optimal + ";" + (DateTime.Now - startTime).TotalMilliseconds + ";" + Curve.CurveLength(lineVectorArray) + ";" + Curve.CurveCollsionCount(lineVectorArray) + ";" + Curve.MaximalCurveAngel(lineVectorArray));
        }
        aStar.Close();
        greedy.Close();
        greedyRef.Close();
        simple.Close();
        hpa.Close();
        }
    }
    // Update is called once per frame
    void Update()
    {
        /*
        bool astar = true;
        if (astar)
        {
            IntTriple startCell = VectorToCell(startObject.transform.position, stepSize);
            IntTriple goalCell = VectorToCell(goalObject.transform.position, stepSize);

            //List<Vector3> linePath = A_Star(startObject, goalObject);

            List<IntTriple> linePathCell = AStar.AStarSearch<IntTriple>(startCell, goalCell, GetNeighbors, (x, y) => x == y, HeuristicGenerator(goalObject.transform.position, stepSize), CostsBetweenGenerator(stepSize)).path;


            Vector3[] lineVectorArray = new Vector3[linePathCell.Count + 2];
            lineVectorArray[0] = goalObject.transform.position;
            for (int i = 1; i < linePathCell.Count + 1; i++)
            {
                lineVectorArray[i] = CellToVector(linePathCell[i - 1], stepSize);
            }
            lineVectorArray[linePathCell.Count + 1] = startObject.transform.position;

            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = lineVectorArray.Length;
            lineRenderer.SetPositions(lineVectorArray);
        }
        else
        {

            IntTriple startCell = VectorToCell(startObject.transform.position, stepSize);
            IntTriple goalCell = VectorToCell(goalObject.transform.position, stepSize);

            

            List<IntTriple> linePathCell = Greedy.GreedySearch<IntTriple>(startCell, goalCell, GetNeighbors, (x, y) => x == y, HeuristicGenerator(goalObject.transform.position, stepSize));


            Vector3[] lineVectorArray = new Vector3[linePathCell.Count + 2];
            lineVectorArray[0] = startObject.transform.position;
            for (int i = 1; i < linePathCell.Count + 1; i++)
            {
                lineVectorArray[i] = CellToVector(linePathCell[i - 1], stepSize);
            }
            lineVectorArray[linePathCell.Count + 1] = goalObject.transform.position;

            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = lineVectorArray.Length;
            lineRenderer.SetPositions(lineVectorArray);

        }
        */


        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3[] curve = SimpleCurveGerneration.startContinous(startObject.transform.position,goalObject.transform.position, testObject);
        lineRenderer.positionCount = curve.Length;
        lineRenderer.SetPositions(curve);
        
        /*
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        Vector3[] controlPoints = new Vector3[testObject.Length];
        for(int i = 0; i < testObject.Length; i++)
        {
            controlPoints[i] = testObject[i].transform.position;
        }

        Vector3[] curve = BezierCurve.calculateCurve(controlPoints, 50);
        lineRenderer.positionCount = curve.Length;
        lineRenderer.SetPositions(curve);
        */
        
    }

    //Functions for the A* Search

    //gets the neighbors of a node by projecting a cube with the length stepSize  to every side and then checks for collision.
    Func<IntTriple, List<IntTriple>> GetNeighborsGenerator(float stepSize)
    {
        List<IntTriple> GetNeighbors(IntTriple node)
        {
            //TODO map to the plane between start and goal
            List<IntTriple> neighbors = new List<IntTriple>();



            for (int x = -1; x <= 1; x += 1)
            {
                for (int y = -1; y <= 1; y += 1)
                {
                    for (int z = -1; z <= 1; z += 1)
                    {
                        if ((x != 0 || y != 0 || z != 0) //dont return the node as its own neighbor
                            && (node.y + y >= 0)) //dont bent the path below the ground
                        {
                            IntTriple cell = new IntTriple(node.x + x, node.y + y, node.z + z);

                            if (!collisonWithObstacle(CellToVector(cell, stepSize), new Vector3(stepSize / 2, stepSize / 2, stepSize / 2)))
                            {
                                neighbors.Add(cell);
                            }
                        }
                    }
                }
            }
            return neighbors;
        }
        return GetNeighbors;
    }

    public  static Func<IntTriple, float> HeuristicGenerator(Vector3 goal, float stepSize)
    {
        float Heuristic(IntTriple cell)
        {
            return Vector3.Distance(CellToVector(cell,stepSize),goal);
        }
        return Heuristic;
    }

    public static Func<IntTriple, IntTriple, float> CostsBetweenGenerator(float stepSize)
    {
        float CostsBetween(IntTriple cell1, IntTriple cell2)
        {
            return Vector3.Distance(CellToVector(cell1, stepSize), CellToVector(cell2, stepSize));
        }
        return CostsBetween;
    }

    //Only things outside the boundingbox of start and goal are considered obstacles
    public static bool collisonWithObstacle(Vector3 center, Vector3 halfExtends)
    {
        Collider[] collidionsWithStartGoal = Physics.OverlapBox(center, halfExtends, default, 1 << 6);

        //Does the cell collide with the start or goal boundingbox (which is on layer 6)?
        if (collidionsWithStartGoal.Length > 0)
        {
            return false;
        }
        else
        {
            Collider[] colliderInRange = Physics.OverlapBox(center, halfExtends);
            //Collides the cell with nothing else? 
            if (colliderInRange.Length == 0)
            {
                return false;
            }
        }
        return true;
    }

    //TODO actual use orientation
    public static bool collisonWithObstacle(Vector3 center, Vector3 halfExtends, Quaternion orientaton)
    {
        Collider[] collidionsWithStartGoal = Physics.OverlapBox(center, halfExtends, default, 1 << 6);

        //Does the cell collide with the start or goal boundingbox (which is on layer 6)?
        if (collidionsWithStartGoal.Length > 0)
        {
            return false;
        }
        else
        {
            Collider[] colliderInRange = Physics.OverlapBox(center, halfExtends);
            //Collides the cell with nothing else? 
            if (colliderInRange.Length == 0)
            {
                return false;
            }
        }
        return true;
    }

    public static Collider[] GetCollidorsFromObstacles(Vector3 center, Vector3 halfExtends, Quaternion orientaton)
    {
        //Collider[] collidionsWithStartGoal = Physics.OverlapBox(center, halfExtends, orientaton, 1 << 6);

        //Does the cell collide with the start or goal boundingbox (which is on layer 6)?

        Collider[] potentialColliders = Physics.OverlapBox(center, halfExtends, orientaton);
        List<Collider> actuallColliders = new List<Collider>();

        foreach (Collider collider in potentialColliders)
        {
            if (Physics.OverlapBox(collider.bounds.center, collider.bounds.extents + new Vector3(1,1,1), default, 1 << 6).Length == 0)
            {
                actuallColliders.Add(collider);
            }
        }
        return actuallColliders.ToArray();
    }


}
