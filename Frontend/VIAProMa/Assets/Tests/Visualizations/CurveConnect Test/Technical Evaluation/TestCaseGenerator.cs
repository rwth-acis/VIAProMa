using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using static IntTriple;

public class TestCaseGenerator : MonoBehaviour
{
    public GameObject startObject;
    public GameObject goalObject;

    void Start()
    {
        RunTests();
    }

    public static async Task<List<GameObject>> GenerateTestcase(GameObject start, GameObject goal, float distance, int obstacleCount, List<GameObject> previousObstacles)
    {
        try
        {
            var rand = new System.Random();

            foreach (GameObject obstacle in previousObstacles)
            {
                GameObject.Destroy(obstacle);
            }
            await Task.Yield();

            previousObstacles = new List<GameObject>();
            start.transform.position = PlaceRandom(Vector3.zero, rand.Next(0, 50), rand);
            goal.transform.position = PlaceRandom(start.transform.position, distance, rand);

            Vector3 startGoalCenter = start.transform.position + (goal.transform.position - start.transform.position) / 2;
            float startGoalDistance = Vector3.Distance(start.transform.position, goal.transform.position);

            for (int i = 0; i < obstacleCount; i++)
            {
                GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obstacle.transform.position = PlaceRandom(startGoalCenter, rand.Next(0, (int)(startGoalDistance * 0.7f + (float)rand.NextDouble()) + 2), rand);
                obstacle.transform.rotation = Quaternion.Euler(rand.Next(rand.Next(0, 359)), rand.Next(rand.Next(0, 359)), rand.Next(rand.Next(0, 359)));
                obstacle.transform.localScale = new Vector3((float)rand.NextDouble() * 2, (float)rand.NextDouble() * 2, (float)rand.NextDouble() * 2);
                previousObstacles.Add(obstacle);
            }
            return previousObstacles;
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    static Vector3 PlaceRandom(Vector3 start, float distance, System.Random rand)
    {
        Vector3 direction = new Vector3(1, 0, 0);
        direction = Quaternion.Euler(0, rand.Next(0, 359), 0) * direction;
        direction.y = (float)rand.NextDouble() * 0.5f;
        direction.Normalize();
        return start + direction * distance;
    }

    async void RunTests()
    {
        try
        {
            LineController lineController = GameObject.FindObjectOfType<LineController>();
            //System.IO.StreamWriter time = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\Time.csv");
            //time.WriteLine("Distance;A*;Greedy;GreedyRef;Simple");
            System.IO.StreamWriter distanceDiff = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\EndEval\DistanceDiff.csv");
            //System.IO.StreamWriter curvature = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\Curvature.csv");
            System.IO.StreamWriter collisions = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\EndEval\Collisions.csv");

            List<GameObject> previousObstacles = new List<GameObject>();


            for (int distance = 1; distance <= 30; distance++)
            {
                float averageDistance = 0;
                bool hasCollision = false;

                for (int obstacleCount = 0; obstacleCount <= distance / 2; obstacleCount++)
                {
                    previousObstacles = await GenerateTestcase(startObject, goalObject, distance, obstacleCount, previousObstacles);
                    //previousObstacles = GenerateTestcase(startObject, goalObject, distance, obstacleCount, previousObstacles);
                    //int obstacelCount = previousObstacles.Count;
                    float startGoalDistance = Vector3.Distance(startObject.transform.position, goalObject.transform.position);

                    //Test Cases
                    //Debug.Log("Start Goal distance:" + Vector3.Distance(startObject.transform.position, goalObject.transform.position));


                    //Calculate the nearly optimal path:
                    float stepSizeOpti = 0.5f;
                    IntTriple startCellOpti = VectorToCell(startObject.transform.position, stepSizeOpti);
                    IntTriple goalCellOpti = VectorToCell(goalObject.transform.position, stepSizeOpti);
                    List<IntTriple> linePathCell = AStar.AStarGridSearch(startCellOpti, goalCellOpti, stepSizeOpti, goalObject.transform.position, startObject, goalObject).path;
                    Vector3[] lineVectorArray = new Vector3[linePathCell.Count + 2];
                    lineVectorArray[0] = goalObject.transform.position;
                    for (int i = 1; i < linePathCell.Count + 1; i++)
                    {
                        lineVectorArray[i] = CellToVector(linePathCell[i - 1], stepSizeOpti);
                    }
                    lineVectorArray[linePathCell.Count + 1] = startObject.transform.position;

                    //Debug.Log("OPtimal Path length:" + Curve.CurveLength(lineVectorArray));

                    float optimal = CurveGenerator.CurveLength(lineVectorArray);

                    ConnectionCurve connectionCurve = lineController.CreateConnectionCurveOwn(startObject, goalObject);
                    await WaitForCurve(connectionCurve);
                    Vector3[] curve = new Vector3[connectionCurve.lineRenderer.positionCount];
                    connectionCurve.lineRenderer.GetPositions(curve);

                    averageDistance += CurveGenerator.CurveLength(curve);// - optimal;
                    hasCollision = hasCollision || CurveGenerator.CurveCollsionCheck(curve, startObject, goalObject);
                    await Task.Yield();
                    //Photon.Pun.PhotonNetwork.Destroy(connectionCurve.GetComponent<Photon.Pun.PhotonView>());
                    Destroy(connectionCurve.gameObject);
                }
                //time.WriteLine(distance + ";" + averageTimeAStar / ((distance / 2) + 1) + ";" + averageTimeGreedy / ((distance / 2) + 1) + ";" + averageTimeGreedyRef + ";" + averageTimeSimple);
                distanceDiff.WriteLine(distance + ";" + averageDistance / ((distance / 2) + 1));
                //curvature.WriteLine(distance + ";" + averageCurvatureAStar / ((distance / 2) + 1) + ";" + averageCurvatureGreedy / ((distance / 2) + 1) + ";" + averageCurvatureGreedyRef + ";" + averageCurvatureSimple);
                collisions.WriteLine(distance + ";" + hasCollision);
            }
            //time.Close();
            distanceDiff.Close();
            //curvature.Close();
            collisions.Close();
        }
        catch (Exception e)
        {
            throw e;
        }      
    }

    async Task WaitForCurve(ConnectionCurve curve)
    {
        try
        {
            for (int i = 0; i < 10 && curve.lineRenderer.positionCount == 2; i++)
            {
                await Task.Yield();
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}
