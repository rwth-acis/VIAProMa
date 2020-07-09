using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCaseGenerator
{
    public static int GenerateTestcase(GameObject start, GameObject goal)
    {
        var rand = new System.Random();
        int obstacleCount = rand.Next(3,15);


        start.transform.position = new Vector3(rand.Next(-50,0),rand.Next(0,10), rand.Next(-50,0));

        Vector3 direction = new Vector3(rand.Next(3, 30), 0, 0);
        direction = Quaternion.Euler(0, rand.Next(0, 359), 0) * direction;
        goal.transform.position = start.transform.position + Quaternion.Euler(0, rand.Next(0, 359), 0) * direction;

        //goal.transform.position = new Vector3(rand.Next(1, 50), rand.Next(0, 10), rand.Next(1, 50));

        for (int i = 0; i <= obstacleCount; i++)
        {
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obstacle.transform.position = start.transform.position + (goal.transform.position - start.transform.position) * rand.Next(-30, 130) / 100f + new Vector3(rand.Next(-10,10),rand.Next(0,10),rand.Next(-10,10));
            obstacle.transform.rotation = Quaternion.Euler(rand.Next(rand.Next(0,359)), rand.Next(rand.Next(0, 359)), rand.Next(rand.Next(0, 359)));
            obstacle.transform.localScale = new Vector3(rand.Next(1,500)/100f, rand.Next(1, 500) / 100f, rand.Next(1, 500) / 100f);
        }
        return obstacleCount;
    }
}
