using System.Collections.Generic;
using UnityEngine;
using System;
using static IntTriple;

public class TestCaseGenerator
{
    public static List<GameObject> GenerateTestcase(GameObject start, GameObject goal, float distance, int obstacleCount, List<GameObject> previousObstacles)
    {
        var rand = new System.Random();

        foreach (GameObject obstacle in previousObstacles)
        {
            GameObject.DestroyImmediate(obstacle);
        }

        previousObstacles = new List<GameObject>();
        start.transform.position = PlaceRandom(Vector3.zero, rand.Next(0,50),rand);
        goal.transform.position = PlaceRandom(start.transform.position,distance, rand);

        Vector3 startGoalCenter = start.transform.position + (goal.transform.position - start.transform.position) / 2;
        float startGoalDistance = Vector3.Distance(start.transform.position,goal.transform.position);

        for (int i = 0; i < obstacleCount; i++)
        {
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.transform.position = PlaceRandom(startGoalCenter, rand.Next(0, (int)(startGoalDistance*0.7f + (float)rand.NextDouble()) + 2), rand);
            obstacle.transform.rotation = Quaternion.Euler(rand.Next(rand.Next(0,359)), rand.Next(rand.Next(0, 359)), rand.Next(rand.Next(0, 359)));
            obstacle.transform.localScale = new Vector3((float)rand.NextDouble() * 2, (float)rand.NextDouble() * 2, (float)rand.NextDouble() * 2);
            previousObstacles.Add(obstacle);
        }
        return previousObstacles;
    }

    static Vector3 PlaceRandom(Vector3 start, float distance, System.Random rand)
    {
        Vector3 direction = new Vector3(1, 0, 0);
        direction = Quaternion.Euler(0, rand.Next(0, 359), 0) * direction;
        direction.y = (float)rand.NextDouble() * 0.5f;
        direction.Normalize();
        return start + direction * distance;
    }

    static void RunTests(float stepSize, GameObject startObject, GameObject goalObject, GameObject boundContainerStart, GameObject boundContainerEnd)
    {
        //Line renderer for the test scenarious
        GameObject lineRendererAStarObject = new GameObject();
        LineRenderer lineRendererAStar = lineRendererAStarObject.AddComponent<LineRenderer>();
        lineRendererAStar.name = "AStar";
        lineRendererAStar.widthMultiplier = 0.1f;

        GameObject lineRendererGreedyObject = new GameObject();
        LineRenderer lineRendererGreedy = lineRendererGreedyObject.AddComponent<LineRenderer>();
        lineRendererGreedy.name = "Greedy";
        lineRendererGreedy.widthMultiplier = 0.1f;


        GameObject lineRendererHPAStarObject = new GameObject();
        LineRenderer lineRendererHPAStar = lineRendererHPAStarObject.AddComponent<LineRenderer>();
        lineRendererHPAStar.name = "HPA";
        lineRendererHPAStar.widthMultiplier = 0.1f;


        GameObject lineRendererGreedyRefObject = new GameObject();
        LineRenderer lineRendererGreedyRef = lineRendererGreedyRefObject.AddComponent<LineRenderer>();
        lineRendererGreedyRef.name = "GreedyRef";
        lineRendererGreedyRef.widthMultiplier = 0.1f;

        GameObject lineRendererSimpleObject = new GameObject();
        LineRenderer lineRendererSimple = lineRendererSimpleObject.AddComponent<LineRenderer>();
        lineRendererSimple.name = "Simple";
        lineRendererSimple.widthMultiplier = 0.1f;

      
        System.IO.StreamWriter time = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\Time.csv");
        time.WriteLine("Distance;A*;Greedy;GreedyRef;Simple");
        System.IO.StreamWriter distanceDiff = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\DistanceDiff.csv");
        System.IO.StreamWriter curvature = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\Curvature.csv");
        System.IO.StreamWriter collisions = new System.IO.StreamWriter(@"C:\Users\Sebastian\Documents\RWTH\ViaPromaTestLogs\Collisions.csv");

        List<GameObject> previousObstacles = new List<GameObject>();


        for (int distance = 1; distance <= 30; distance++)
        {
            float averageTimeAStar = 0;
            float averageTimeGreedy = 0;
            float averageTimeGreedyRef = 0;
            float averageTimeSimple = 0;

            float averageDistanceDiffAStar = 0;
            float averageDistanceDiffGreedy = 0;
            float averageDistanceDiffGreedyRef = 0;
            float averageDistanceDiffSimple = 0;

            float averageCurvatureAStar = 0;
            float averageCurvatureGreedy = 0;
            float averageCurvatureGreedyRef = 0;
            float averageCurvatureSimple = 0;

            float averageCollisionsAStar = 0;
            float averageCollisionsGreedy = 0;
            float averageCollisionsGreedyRef = 0;
            float averageCollisionsSimple = 0;

            for (int obstacleCount = 0; obstacleCount <= distance / 2; obstacleCount++)
            {
                previousObstacles = TestCaseGenerator.GenerateTestcase(startObject, goalObject, distance, obstacleCount, previousObstacles);
                int obstacelCount = previousObstacles.Count;
                float startGoalDistance = Vector3.Distance(startObject.transform.position, goalObject.transform.position);

                //Test Cases
                //Debug.Log("Start Goal distance:" + Vector3.Distance(startObject.transform.position, goalObject.transform.position));


                //Calculate the nearly optimal path:
                float stepSizeOpti = 0.5f;
                IntTriple startCellOpti = VectorToCell(startObject.transform.position, stepSizeOpti);
                IntTriple goalCellOpti = VectorToCell(goalObject.transform.position, stepSizeOpti);
                List<IntTriple> linePathCell = AStar.AStarGridSearch(startCellOpti, goalCellOpti, stepSizeOpti, goalObject.transform.position).path;
                Vector3[] lineVectorArray = new Vector3[linePathCell.Count + 2];
                lineVectorArray[0] = goalObject.transform.position;
                for (int i = 1; i < linePathCell.Count + 1; i++)
                {
                lineVectorArray[i] = CellToVector(linePathCell[i - 1], stepSizeOpti);
                }
                lineVectorArray[linePathCell.Count + 1] = startObject.transform.position;

                //Debug.Log("OPtimal Path length:" + Curve.CurveLength(lineVectorArray));

                float optimal = CurveGenerator.CurveLength(lineVectorArray);


                DateTime startTime = DateTime.Now;

                //AStar
                //Debug.Log("AStar:");

                IntTriple startCell = VectorToCell(startObject.transform.position, stepSize);
                IntTriple goalCell = VectorToCell(goalObject.transform.position, stepSize);

                //List<Vector3> linePath = A_Star(startObject, goalObject);
                AStar.SearchResult<IntTriple> result = AStar.AStarGridSearch(startCell,goalCell,stepSize,goalObject.transform.position);
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

                //aStar.WriteLine(obstacelCount + ";" + startGoalDistance + ";" + optimal + ";" + (DateTime.Now - startTime).TotalMilliseconds + ";" + Curve.CurveLength(lineVectorArray) + ";" + Curve.CurveCollsionCount(lineVectorArray) + ";" + Curve.MaximalCurveAngel(lineVectorArray));
                averageTimeAStar += (float)(DateTime.Now - startTime).TotalMilliseconds;
                averageDistanceDiffAStar += CurveGenerator.CurveLength(lineVectorArray) - optimal;
                averageCollisionsAStar += CurveGenerator.CurveCollsionCount(lineVectorArray, boundContainerStart, boundContainerEnd);
                averageCurvatureAStar += CurveGenerator.MaximalCurveAngel(lineVectorArray);

                //Greedy
                startTime = DateTime.Now;

                result = Greedy.GreedyGridSearch(startCell, goalCell, stepSize, goalObject.transform.position);
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
                averageTimeGreedy += (float)(DateTime.Now - startTime).TotalMilliseconds;
                averageDistanceDiffGreedy += CurveGenerator.CurveLength(lineVectorArray) - optimal;
                averageCollisionsGreedy += CurveGenerator.CurveCollsionCount(lineVectorArray, boundContainerStart, boundContainerEnd);
                averageCurvatureGreedy += CurveGenerator.MaximalCurveAngel(lineVectorArray);

                //HPAStar

                /*
                //Debug.Log("HPA");
                startTime = DateTime.Now;
                List<Vector3> linePathVector3 = HPAStar.HPAStarSearch(startObject.transform.position, goalObject.transform.position, stepSize, 5);
                lineRendererHPAStar.positionCount = linePathVector3.Count;
                lineRendererHPAStar.SetPositions(linePathVector3.ToArray());
                //Debug.Log((DateTime.Now - startTime).TotalMilliseconds);
                //Debug.Log("Path Length:" + Curve.CurveLength(linePathVector3.ToArray()));
                //Debug.Log("Path colliions: " + Curve.CurveCollsionCount(linePathVector3.ToArray()));
                //Debug.Log("Path curvature: " + Curve.MaximalCurveAngel(linePathVector3.ToArray()));
                hpa.WriteLine(obstacelCount + ";" + startGoalDistance + ";" + optimal + ";" + (DateTime.Now - startTime).TotalMilliseconds + ";" + Curve.CurveLength(linePathVector3.ToArray()) + ";" + Curve.CurveCollsionCount(linePathVector3.ToArray()) + ";" + Curve.MaximalCurveAngel(linePathVector3.ToArray()));
                */

                //Greedy refined
                //Debug.Log("Greedy Refined");
                startTime = DateTime.Now;

                result = Greedy.GreedyGridSearch(startCell, goalCell, stepSize, goalObject.transform.position);
                linePathCell = result.path;


                lineVectorArray = new Vector3[linePathCell.Count + 2];
                lineVectorArray[0] = startObject.transform.position;
                for (int i = 1; i < linePathCell.Count + 1; i++)
                {
                lineVectorArray[i] = CellToVector(linePathCell[i - 1], stepSize);
                }
                lineVectorArray[linePathCell.Count + 1] = goalObject.transform.position;
                lineVectorArray = Greedy.postProcessing(lineVectorArray, 3);

                lineRendererGreedyRef.positionCount = lineVectorArray.Length;
                lineRendererGreedyRef.SetPositions(lineVectorArray);
                averageTimeGreedyRef += (float)(DateTime.Now - startTime).TotalMilliseconds;
                averageDistanceDiffGreedyRef += CurveGenerator.CurveLength(lineVectorArray) - optimal;
                averageCollisionsGreedyRef += CurveGenerator.CurveCollsionCount(lineVectorArray, boundContainerStart, boundContainerEnd);
                averageCurvatureGreedyRef += CurveGenerator.MaximalCurveAngel(lineVectorArray);


                //Simple
                //Debug.Log("Simple");
                startTime = DateTime.Now;
                lineVectorArray = SimpleCurveGerneration.StartGeneration(startObject.transform.position, goalObject.transform.position, boundContainerStart, boundContainerEnd, 15);
                lineRendererSimple.positionCount = lineVectorArray.Length;
                lineRendererSimple.SetPositions(lineVectorArray);
                averageTimeSimple += (float)(DateTime.Now - startTime).TotalMilliseconds;
                averageDistanceDiffSimple += CurveGenerator.CurveLength(lineVectorArray) - optimal;
                averageCollisionsSimple += CurveGenerator.CurveCollsionCount(lineVectorArray, boundContainerStart, boundContainerEnd);
                averageCurvatureSimple += CurveGenerator.MaximalCurveAngel(lineVectorArray);
            }
            time.WriteLine(distance + ";" + averageTimeAStar / ((distance / 2) + 1) + ";" + averageTimeGreedy / ((distance / 2) + 1) + ";" + averageTimeGreedyRef + ";" + averageTimeSimple);
            distanceDiff.WriteLine(distance + ";" + averageDistanceDiffAStar / ((distance / 2) + 1) + ";" + averageDistanceDiffGreedy / ((distance / 2) + 1) + ";" + averageDistanceDiffGreedyRef + ";" + averageDistanceDiffSimple);
            curvature.WriteLine(distance + ";" + averageCurvatureAStar / ((distance / 2) + 1) + ";" + averageCurvatureGreedy / ((distance / 2) + 1) + ";" + averageCurvatureGreedyRef + ";" + averageCurvatureSimple);
            collisions.WriteLine(distance + ";" + averageCollisionsAStar / ((distance / 2) + 1) + ";" + averageCollisionsGreedy / ((distance / 2) + 1) + ";" + averageCollisionsGreedyRef + ";" + averageCollisionsSimple);
        }
        time.Close();
        distanceDiff.Close();
        curvature.Close();
        collisions.Close();

        //TestCaseGenerator.GenerateTestcase(startObject,goalObject,10,15, previousObstacles);
        
    }
}
