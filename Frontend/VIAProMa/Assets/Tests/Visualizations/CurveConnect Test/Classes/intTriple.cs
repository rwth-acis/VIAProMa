using System.Collections;
using System.Collections.Generic;

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
}
