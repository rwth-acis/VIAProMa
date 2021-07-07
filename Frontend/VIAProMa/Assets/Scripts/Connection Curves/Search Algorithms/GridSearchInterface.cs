using System.Collections.Generic;
using UnityEngine;
using static IntTriple;
using System;

/// <summary>
/// Provides methods for classes that search pathes in the continuous world by using a 3D grid.
/// </summary>
public abstract class GridSearch : CurveGenerator
{
    /// <summary>
    /// The result of a grid search operation. Contains the path and the path length.
    /// </summary>
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

    /// <summary>
    /// The result of a graph search operation. Contains the path and the path length.
    /// </summary>
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

    /// <summary>
    /// Generates a function that only takes a cell and then gets the neighbors of it by projecting a cube with the length stepSize to every side and then checks for collisions.
    /// </summary>
    public static Func<IntTriple, List<IntTriple>> GetNeighborsGeneratorGrid(float stepSize, GameObject startObject, GameObject goalObject)
    {
        List<IntTriple> GetNeighbors(IntTriple node)
        {
            List<IntTriple> neighbors = new List<IntTriple>();

            for (int x = -1; x <= 1; x += 1)
            {
                for (int y = -1; y <= 1; y += 1)
                {
                    for (int z = -1; z <= 1; z += 1)
                    {
                        if ((x != 0 || y != 0 || z != 0)) //dont return the node as its own neighbor
                            //&& (node.y + y >= 0)) //dont bent the path below the ground
                        {
                            IntTriple cell = new IntTriple(node.x + x, node.y + y, node.z + z);

                            if (!collisonWithObstacle(CellToVector(cell, stepSize), new Vector3(stepSize / 2, stepSize / 2, stepSize / 2), startObject, goalObject))
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

    /// <summary>
    /// Generates a heuristic function for the search on a grid with the cell size stepSize.
    /// </summary>
    public static Func<IntTriple, float> HeuristicGeneratorGrid(Vector3 goal, float stepSize)
    {
        float Heuristic(IntTriple cell)
        {
            return Vector3.Distance(CellToVector(cell, stepSize), goal);
        }
        return Heuristic;
    }

    /// <summary>
    /// Generates a function that calculates the cost between two adjacent cells.
    /// </summary>
    public static Func<IntTriple, IntTriple, float> CostsBetweenGeneratorGrid(float stepSize)
    {
        float CostsBetween(IntTriple cell1, IntTriple cell2)
        {
            return Vector3.Distance(CellToVector(cell1, stepSize), CellToVector(cell2, stepSize));
        }
        return CostsBetween;
    }
}
