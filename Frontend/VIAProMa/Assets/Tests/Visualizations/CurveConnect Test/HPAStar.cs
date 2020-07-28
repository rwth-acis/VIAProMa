using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IntTriple;

public class HPAStar : GridSearch
{
    

    public static List<Vector3> HPAStarSearch(Vector3 start, Vector3 goal, float stepSize, int clusterSize)
    {
        Maze searchMaze = new Maze(stepSize, clusterSize);
        Entrance startEntrance = searchMaze.InsertStartOrGoalNode(start, true);
        Entrance goalEntrance = searchMaze.InsertStartOrGoalNode(goal, false);
        List<Entrance> path = AStar.AStarSearch<Entrance>(startEntrance,goalEntrance, GetNeighboresGenerator(searchMaze, stepSize, clusterSize), (x,y) => x==y, HeuristicGenerator(goal), CostsBetween).path;

        List<Vector3> positions = new List<Vector3>();

        foreach (Entrance entrance in path)
        {
            positions.Add(entrance.position);
        }

        return positions;
    }

    static Func<Entrance, List<Entrance>> GetNeighboresGenerator(Maze searchMaze, float stepSize, int clusterSize)
    {
        List<Entrance> GetNeighbores(Entrance entrance)
        {
            //To make sure that "entrance" has all edges. If the cluster already exists, nothing will happen
            for (IntTriple direction = new IntTriple(-1, 0, 0); direction != new IntTriple(0, 0, 0); direction = TripleIterator(direction))
            {
                searchMaze.addCluster(CellToCluster(VectorToCell(entrance.position + new Vector3(direction.x,direction.y,direction.z)*stepSize/2,stepSize),clusterSize));
            }
            return new List<Entrance>(entrance.edges.Keys);
        }
        return GetNeighbores;
    }

    static Func<Entrance, float> HeuristicGenerator(Vector3 goal)
    {
        float Heuristic(Entrance entrance)
        {
            return Vector3.Distance(entrance.position,goal);
        }
        return Heuristic;
    }

    static float CostsBetween(Entrance firstEntrance, Entrance secondEntrance)
    {
        return firstEntrance.edges[secondEntrance];
    }

    public static List<Vector3> RefinePath(List<Vector3> path, float stepSize, int clusterSize)
    {
        List<Vector3> refinedPath = new List<Vector3>();
        for (int i = 0; i < path.Count - 2; i++)
        {
            //The shifting is necasary to ensure that the cells are in the same cluster
            Vector3 test = path[i] + (path[i + 1] - path[i]).normalized * stepSize / 2;
            Vector3 test2 = path[i+1] + (path[i] - path[i + 1]).normalized * stepSize / 2;
            IntTriple firstCell = VectorToCell(path[i] + (path[i + 1] - path[i]).normalized * stepSize/2, stepSize);
            IntTriple secondCell = VectorToCell(path[i+1] + (path[i] - path[i+1]).normalized * stepSize / 2, stepSize);
            IntTriple clusterNumber = CellToCluster(firstCell,clusterSize);
            IntTriple clusterNumber2 = CellToCluster(secondCell, clusterSize);

            List<IntTriple> triplePath = AStar.AStarSearch<IntTriple>(firstCell, secondCell,
                            Maze.GetNeighborsFunctionGenerator(clusterNumber, clusterSize, stepSize), (item1, item2) => item1 == item2, HeuristicGeneratorGrid(path[i+1], stepSize),
                            CostsBetweenGeneratorGrid(stepSize)).path;
            foreach (IntTriple triple in triplePath)
            {
                refinedPath.Add(CellToVector(triple,stepSize));
            }
        }
        return refinedPath;
    }
}
