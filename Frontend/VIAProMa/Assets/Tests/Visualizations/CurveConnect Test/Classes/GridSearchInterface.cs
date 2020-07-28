using System.Collections.Generic;
using UnityEngine;
using static IntTriple;
using System;

public abstract class GridSearch : CurveGenerator
{
    public struct GridSearchResult
    {
        public List<Vector3> path { get; set; }
        public float costs { get; set; }

        public GridSearchResult(List<Vector3> path, float length)
        {
            this.path = path;
            this.costs = length;
        }
    }

    public struct SearchResult<T>
    {
        public List<T> path { get; set; }
        public float costs { get; set; }

        public SearchResult(List<T> path, float length)
        {
            this.path = path;
            this.costs = length;
        }
    }

    //gets the neighbors of a node by projecting a cube with the length stepSize  to every side and then checks for collision.
    public static Func<IntTriple, List<IntTriple>> GetNeighborsGeneratorGrid(float stepSize)
    {
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
                            IntTriple cell = new IntTriple(node.x + x, node.y + y, node.z + z);

                            if (!collisonWithObstacle(CellToVector(cell, stepSize), new Vector3(stepSize / 2, stepSize / 2, stepSize / 2)))
                            {
                                neighbors.Add(cell);
                            }
                        }
                    }
                }
            }
            return neighbors;
        }
        return GetNeighbors;
    }

    public static Func<IntTriple, float> HeuristicGeneratorGrid(Vector3 goal, float stepSize)
    {
        float Heuristic(IntTriple cell)
        {
            return Vector3.Distance(CellToVector(cell, stepSize), goal);
        }
        return Heuristic;
    }

    public static Func<IntTriple, IntTriple, float> CostsBetweenGeneratorGrid(float stepSize)
    {
        float CostsBetween(IntTriple cell1, IntTriple cell2)
        {
            return Vector3.Distance(CellToVector(cell1, stepSize), CellToVector(cell2, stepSize));
        }
        return CostsBetween;
    }
}
