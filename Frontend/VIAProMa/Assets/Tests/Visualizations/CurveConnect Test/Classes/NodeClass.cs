using System;

public class Node
{
    public Triple<int, int, int> position
    {
        get;
        set;
    }
    public Node cameFrom
    {
        get;
        set;
    }
}