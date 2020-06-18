using System;

public class Edge
{
    float costs;
    Node endNode;

    public Edge(Node node, float costs)
    {
        endNode = node;
        this.costs = costs;
    }
    
}
