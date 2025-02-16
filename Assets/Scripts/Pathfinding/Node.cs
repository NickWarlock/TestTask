using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{
    public bool walkable;
    public Vector2 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost; // Distance from start node
    public int hCost; // Heuristic (distance from end node)
    public Node parent;

    public int FCost => gCost + hCost;

    public Node(bool walkable, Vector2 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
}