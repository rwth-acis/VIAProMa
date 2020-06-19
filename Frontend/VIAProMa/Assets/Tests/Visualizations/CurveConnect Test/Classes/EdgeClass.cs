using System;

public class Edge
{
    float costs;
    Object endNode;

    public Edge(Object node, float costs)
    {
        endNode = node;
        this.costs = costs;
    }
    
}
