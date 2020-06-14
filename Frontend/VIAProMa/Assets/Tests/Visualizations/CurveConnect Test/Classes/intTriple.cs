using System.Collections;
using System.Collections.Generic;

public class IntTriple
{
    public Triple<int, int, int> triple;

    public IntTriple(int int1, int int2, int int3)
    {
        triple = new Triple<int, int, int>(int1, int2, int3);
    }

    public static IntTriple operator +(IntTriple triple1, IntTriple triple2)
    {
        return new IntTriple(triple1.triple.Item1 + triple2.triple.Item1, triple1.triple.Item2 + triple2.triple.Item2, triple1.triple.Item3 + triple2.triple.Item3);
    }

    public override bool Equals(object obj)
    {
        return triple.Equals(obj);
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
        return triple.GetHashCode();
    }

    public static IntTriple operator *(IntTriple triple, int scalar)
    {
        return new IntTriple(triple.triple.Item1 * scalar, triple.triple.Item2 * scalar , triple.triple.Item3 * scalar);
    }
}
