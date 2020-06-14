using System;

//Taken from https://stackoverflow.com/questions/12023164/do-we-have-some-sort-of-a-triple-collection-in-c-sharp
public class Triple<T1, T2, T3> : IEquatable<Object>
{
    public T1 Item1
    {
        get;
        set;
    }

    public T2 Item2
    {
        get;
        set;
    }

    public T3 Item3
    {
        get;
        set;
    }

    public Triple(T1 Item1, T2 Item2, T3 Item3)
    {
        this.Item1 = Item1;
        this.Item2 = Item2;
        this.Item3 = Item3;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || (obj as Triple<T1, T2, T3>) == null) //if the object is null or the cast fails
            return false;
        else
        {
            Triple<T1, T2, T3> triple = (Triple<T1, T2, T3>)obj;
            return Item1.Equals(triple.Item1) && Item2.Equals(triple.Item2) && Item3.Equals(triple.Item3);
        }
    }

    public override int GetHashCode()
    {
        return Item1.GetHashCode() ^ Item2.GetHashCode() ^ Item3.GetHashCode();
    }

    public static bool operator ==(Triple<T1, T2, T3> triple1, Triple<T1, T2, T3> triple2)
    {
        return triple1.Equals(triple2);
    }

    public static bool operator !=(Triple<T1, T2, T3> triple1, Triple<T1, T2, T3> triple2)
    {
        return !triple1.Equals(triple2);
    }
}