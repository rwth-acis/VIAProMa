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
        List<Vector3> lineVectorList = HPAStar.HPAStarSearch(startObject.transform.position, goalObject.transform.position, 1, 5);

        lineRenderer.positionCount = lineVectorList.Count;
        lineRenderer.SetPositions(lineVectorList.ToArray());

    }
    // Update is called once per frame
    void Update()
    {
        /*
        IntTriple startCell = VectorToCell(startObject.transform.position,stepSize);
        IntTriple goalCell = VectorToCell(goalObject.transform.position, stepSize);

        //List<Vector3> linePath = A_Star(startObject, goalObject);
        
        List<IntTriple> linePathCell = AStar.AStarSearch<IntTriple>(startCell, goalCell, GetNeighbors, (x,y) => x==y, HeuristicGenerator(goalObject.transform.position, stepSize), CostsBetweenGenerator(stepSize)).path;
        
        
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
}
