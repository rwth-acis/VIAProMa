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


    //Content for A*
    Vector3 start;
    Vector3 goal;
    //Priority queu that contains the nodes that may need to be expanded
    SimplePriorityQueue<IntTriple> openSet;
    //For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start
    Dictionary<IntTriple, IntTriple> cameFrom;
    //For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
    Dictionary<IntTriple, float> gScore;
    IntTriple current;
    int framecount = 0;
    IntTriple goalCell;

    //For distinguishing between random objects and start/goal
    List<GameObject> startGoalObjectsWithCollider;

    BoxCollider startBoundingBox;
    BoxCollider goalBoundingBox;

    GameObject boundContainerStart;
    GameObject boundContainerEnd;


    // Start is called before the first frame update
    void Start()
    {
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
        resetAStar();

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


        //Test Stuff
        
        //Maze test = new Maze(stepSize,5);
        //test.addCluster(new IntTriple(0, 0, 0));
        //test.addCluster(new IntTriple(0, 0, 1));
        
    }

    // Update is called once per frame
    void Update()
    {
        IntTriple startCell = VectorToCell(startObject.transform.position,stepSize);
        IntTriple goalCell = VectorToCell(goalObject.transform.position, stepSize);

        //List<Vector3> linePath = A_Star(startObject, goalObject);
        
        List<IntTriple> linePathCell = AStar.AStarSearchTest<IntTriple>(startCell, goalCell, stepSize, GetNeighbors, (x,y) => x==y, HeuristicGenerator(goalObject.transform.position), CostsBetween).path;
        
        
        Vector3[] lineVectorArray = new Vector3[linePathCell.Count+2];
        lineVectorArray[0] = goalObject.transform.position;
        for (int i = 1; i < linePathCell.Count+1; i++)
        {
            lineVectorArray[i] = CellToVector(linePathCell[i - 1], stepSize);
        }
        lineVectorArray[linePathCell.Count + 1] = startObject.transform.position;

        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = lineVectorArray.Length;
        lineRenderer.SetPositions(lineVectorArray);
        

        /*
        //List<Vector3> linePath = A_Star(startObject, goalObject);
        framecount++;
        if (linePath != null)
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = linePath.Count + 2;

            linePath.Insert(0, goalObject.transform.position);
            linePath.Add(startObject.transform.position);
            lineRenderer.SetPositions(linePath.ToArray());
        }
        */
    }

    //Functions for the A* Search

    //gets the neighbors of a node by projecting a cube with the length stepSize  to every side and then checks for collision.
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
                        IntTriple  cell = new IntTriple(node.x + x, node.y + y, node.z + z);

                        if (!collisonWithObstacle(CellToVector(cell,stepSize), new Vector3(stepSize / 2, stepSize / 2, stepSize / 2)) )
                        {
                            neighbors.Add(cell);
                        }                        
                    }
                }
            }
        }
        return neighbors;
    }

    Func<IntTriple, float> HeuristicGenerator(Vector3 goal)
    {
        float Heuristic(IntTriple cell)
        {
            return Vector3.Distance(CellToVector(cell,stepSize),goal);
        }
        return Heuristic;
    }

    float CostsBetween(IntTriple cell1, IntTriple cell2)
    {
        return Vector3.Distance(CellToVector(cell1,stepSize),CellToVector(cell2,stepSize));
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
 
    //TODO Improvable by given every node a pointer to it's ancestor which is much quicker then looking it up in a dictonary
    List<Vector3> reconstruct_path(Dictionary<IntTriple, IntTriple> cameFrom, IntTriple current)
    {
        List<Vector3> totalPath = new List<Vector3>();
        totalPath.Add(CellToVector(current, stepSize));

        IntTriple ancestor;
        while (cameFrom.TryGetValue(current, out ancestor))
        {
            totalPath.Add(CellToVector(ancestor, stepSize));
            current = ancestor;
        }

        return totalPath;
    }

    void resetAStar()
    {
        Debug.Log(framecount);
        start = startObject.transform.position;
        goal = goalObject.transform.position;

        openSet = new SimplePriorityQueue<IntTriple>();
        cameFrom = new Dictionary<IntTriple, IntTriple>();
        gScore = new Dictionary<IntTriple, float>();
        IntTriple startPositionCell = VectorToCell(startObject.transform.position, stepSize);
        goalCell = VectorToCell(goal, stepSize);
        openSet.Enqueue(startPositionCell, 0);
        gScore.Add(startPositionCell, Vector3.Distance(start, goal));
        framecount = 0;
    }
    //A* with the heuristic: "euclidian distance between
    //start and goal" which is admissable and consistent.
    List<Vector3> A_Star(GameObject startObject, GameObject goalObject)
    {
        //When finding a path takes longer than 30 frames, give up
        if (framecount >= 30)
        {
            resetAStar();
        }

        DateTime startTime = DateTime.Now;

        //while (openSet.Count != 0 && DateTime.Now - startTime < TimeSpan.FromMilliseconds(maxProcessingTimePerFrame))
        while (openSet.Count != 0)
        {
            current = openSet.Dequeue();
            if (current == goalCell)
            {
                List<Vector3> optimalPath = reconstruct_path(cameFrom, current);
                resetAStar();
                return optimalPath;
            }

            List<IntTriple> neighbors = GetNeighbors(current);

            //TODO Maby here multithreading?
            foreach (IntTriple neighbor in neighbors)
            {
                float h = Vector3.Distance(CellToVector(neighbor, stepSize), goal);
                float tentative_gScore = gScore[current] + Vector3.Distance(CellToVector(current, stepSize), CellToVector(neighbor, stepSize));
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
            resetAStar();
            return new List<Vector3>();
        }

        return null;
    }

}
