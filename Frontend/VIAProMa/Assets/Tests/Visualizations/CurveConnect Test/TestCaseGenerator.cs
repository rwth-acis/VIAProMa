using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

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
}
