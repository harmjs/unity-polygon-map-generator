using UnityEngine;
using System.Collections.Generic;
public class Face {
    private int index;
    public Vector2 position;
    public int[] triangles;
    public Vector2[] vertices;
    public Vector2[] normals;
    public float noise;
    public Tile tile;
    public Corner[] corners;
    //public Dictionary<int, Corner> CornerMap;

    public Face[] faces;
    //public Dictionary<int, Face> FaceMap;

    public Edge[] edges;
    //public Dictionary<int, Edge> EdgeMap;

    public Face(int index, PolyFace polyFace) { 
        this.index = index;
        this.position = polyFace.vertex.ToVector2();
    }
}
