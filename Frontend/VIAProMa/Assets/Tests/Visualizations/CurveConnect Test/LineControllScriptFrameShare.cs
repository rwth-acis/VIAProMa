using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

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

        
        //BoxCollider boundingboxOnOtherLayer = (BoxCollider)new Collider();
        //boundingboxOnOtherLayer.center = ((BoxCollider)startBoundingBox).center;
        //boundingboxOnOtherLayer.size = ((BoxCollider)startBoundingBox).size;

        boundContainerStart = new GameObject();
        boundContainerStart.transform.parent = startObject.transform;
        boundContainerStart.transform.localPosition = Vector3.zero;
        boundContainerStart.layer = 6;
        boundContainerStart.name = "Hallu";

        boundContainerStart.AddComponent<BoxCollider>();
        BoxCollider boundingboxOnOtherLayer = boundContainerStart.GetComponent<BoxCollider>();
        Vector3 test = boundingboxOnOtherLayer.transform.localPosition;
        Vector3 test2 = boundContainerStart.transform.localPosition;
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
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3> linePath = A_Star(startObject, goalObject);
        framecount++;
        if (linePath != null)
        {
            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = linePath.Count + 2;

            linePath.Insert(0, goalObject.transform.position);
            linePath.Add(startObject.transform.position);
            lineRenderer.SetPositions(linePath.ToArray());
        }
    }

    //gets the neighbors of a node by projecting a cube with the length stepSize  to every side and then checks for collision.
    List<IntTriple> getNeighbors(IntTriple node)
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

                        if ( !collisonWithObstacle(tupleToVector(cell,stepSize), new Vector3(stepSize / 2, stepSize / 2, stepSize / 2)) )
                        {
                            neighbors.Add(cell);
                        }                        
                    }
                }
            }
        }
        return neighbors;
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
        totalPath.Add(tupleToVector(current, stepSize));

        IntTriple ancestor;
        while (cameFrom.TryGetValue(current, out ancestor))
        {
            totalPath.Add(tupleToVector(ancestor, stepSize));
            current = ancestor;
        }

        return totalPath;
    }

    public static Vector3 tupleToVector(IntTriple tuple, float stepSize)
    {
        return new Vector3(tuple.x, tuple.y, tuple.z)*stepSize;
    }


    void resetAStar()
    {
        Debug.Log(framecount);
        start = startObject.transform.position;
        goal = goalObject.transform.position;

        openSet = new SimplePriorityQueue<IntTriple>();
        cameFrom = new Dictionary<IntTriple, IntTriple>();
        gScore = new Dictionary<IntTriple, float>();
        Vector3 startPositionContinous = startObject.transform.position / stepSize;
        IntTriple startPositionDiscrete = new IntTriple((int)startPositionContinous.x, (int)startPositionContinous.y, (int)startPositionContinous.z);
        openSet.Enqueue(startPositionDiscrete, 0);
        gScore.Add(startPositionDiscrete, Vector3.Distance(start, goal));
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
        
        while (openSet.Count != 0 && DateTime.Now - startTime < TimeSpan.FromMilliseconds(maxProcessingTimePerFrame))
        {
            current = openSet.Dequeue();
            if (Vector3.Distance(tupleToVector(current, stepSize), goal) <= stepSize)
            {
                List<Vector3> optimalPath = reconstruct_path(cameFrom, current);
                resetAStar();
                return optimalPath;
            }

            List<IntTriple> neighbors = getNeighbors(current);

            //TODO Maby here multithreading?
            foreach (IntTriple neighbor in neighbors)
            {
                float h = Vector3.Distance(tupleToVector(neighbor, stepSize), goal);
                float tentative_gScore = gScore[current] + Vector3.Distance(tupleToVector(current, stepSize), tupleToVector(neighbor, stepSize));
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

    List<Vector3> HPA()
    {
        return null;
    }

    //Make cube clutsers. Divide each side on a cube in a normal cluster and use normal HPA entrance generation on it.
    //Then, use 3D A* to find path between the entrances.
    void abstractCluster()
    {
          
    }



}
