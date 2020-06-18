using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPAStar
{
    private GameObject startObject;
    private GameObject goalObject;
    private Maze searchMaze;

    public HPAStar(GameObject startObject, GameObject goalObject, float stepSize, int clusterSize)
    {
        this.startObject = startObject;
        this.goalObject = goalObject;
        searchMaze = new Maze(stepSize, clusterSize);
    }

    public List<Vector3> HPAStarSearch()
    {
        //



        return null;
    }
}
