using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A triple of type int. Contains functions to use it to store and manage cells for a 3D grid.
/// </summary>
public class IntTriple
{
    public int x;
    public int y;
    public int z;

    public IntTriple(int int1, int int2, int int3)
    {
        x = int1;
        y = int2;
        z = int3;
    }

    public IntTriple(IntTriple triple)
    {
        x = triple.x;
        y = triple.y;
        z = triple.z;
    }

    public static IntTriple operator +(IntTriple triple1, IntTriple triple2)
    {
        return new IntTriple(triple1.x + triple2.x, triple1.y + triple2.y, triple1.z + triple2.z);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || (obj as IntTriple) == null) //if the object is null or the cast fails
            return false;
        else
        {
            IntTriple triple = (IntTriple)obj;
            return x == triple.x && y == triple.y && z == triple.z;
        }
    }
    public static bool operator ==(IntTriple triple1, IntTriple triple2)
    {
        return triple1.Equals(triple2);
    }

    public static bool operator !=(IntTriple triple1, IntTriple triple2)
    {
        return !(triple1.Equals(triple2));
    }
    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
    }

    public static IntTriple operator *(IntTriple triple, int scalar)
    {
        return new IntTriple(triple.x * scalar, triple.y * scalar , triple.z * scalar);
    }

    public static IntTriple operator /(IntTriple triple, int scalar)
    {
        return new IntTriple(triple.x / scalar, triple.y / scalar, triple.z / scalar);
    }

    /// <summary>
    /// All conversion between cells(IntTriples) and other search related types
    /// </summary>
    public static Vector3 CellToVector(IntTriple triple, float stepSize)
    {
        return new Vector3(triple.x, triple.y, triple.z) *stepSize + new Vector3(stepSize/2,stepSize/2,stepSize/2);
    }

    /// <summary>
    /// Calculates the corresponding cell on the grid
    /// </summary>
    public static IntTriple VectorToCell(Vector3 vector, float stepSize)
    {
        Vector3 inverseVector = vector /stepSize;
        //Negativ values have to be floored instead of just casted because otherwise (-0.9,0,0) -> (0,0,0) and (0.9,0,0) -> (0,0,0) for example 
        return new IntTriple(   inverseVector.x >= 0 ? (int)inverseVector.x : Mathf.FloorToInt(inverseVector.x),
                                inverseVector.y >= 0 ? (int)inverseVector.y : Mathf.FloorToInt(inverseVector.y),
                                inverseVector.z >= 0 ? (int)inverseVector.z : Mathf.FloorToInt(inverseVector.z));
    }
}
