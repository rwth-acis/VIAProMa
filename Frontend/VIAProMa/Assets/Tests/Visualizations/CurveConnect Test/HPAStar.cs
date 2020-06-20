using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPAStar
{
    

    public static List<Vector3> HPAStarSearch(Vector3 start, Vector3 goal, float stepSize, int clusterSize)
    {
        Maze searchMaze = new Maze(stepSize, clusterSize);
        searchMaze.InsertStartOrGoalNode(start, true);
        searchMaze.InsertStartOrGoalNode(goal, false);

        return null;
    }
}
