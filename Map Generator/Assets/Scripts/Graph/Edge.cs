using UnityEngine;
public class Edge {
    public Vector2 position;
    public int index;
    public Face[] faces;
    public Corner[] corners;

    public Edge(int index, PolyEdge edge) { 
        this.index = index;
        position = edge.vertex.ToVector2();
    }
}
