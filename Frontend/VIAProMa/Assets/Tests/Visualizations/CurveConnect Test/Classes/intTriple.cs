using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntTriple
{
    //public Triple<int, int, int> triple;
    public int x;
    public int y;
    public int z;

    public IntTriple(int int1, int int2, int int3)
    {
        x = int1;
        y = int2;
        z = int3;
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

    //All conversion between cells(IntTriples) and other search related types
    public static Vector3 CellToVector(IntTriple triple, float stepSize)
    {
        return new Vector3(triple.x, triple.y, triple.z) *stepSize + new Vector3(stepSize/2,stepSize/2,stepSize/2);
    }

    public static IntTriple VectorToCell(Vector3 vector, float stepSize)
    {
        Vector3 inverseVector = vector /stepSize;
        return new IntTriple((int)inverseVector.x, (int)inverseVector.y, (int)inverseVector.z);
    }

    public static IntTriple CellToCluster(IntTriple cell, int clusterSize)
    {
        return cell / clusterSize;
    }
}
