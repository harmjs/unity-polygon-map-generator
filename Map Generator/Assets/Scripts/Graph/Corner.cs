using UnityEngine;
using System.Collections.Generic;
public class Corner {
    public int index;
    public Vector2 position;
    public Corner[] corners;
    //public Dictionary<int, Corner> CornerMap;

    public Face[] faces;
    //public Dictionary<int, Face> FaceMap;

    public Edge[] edges;
    //public Dictionary<int, Edge> EdgeMap;

    public Corner(int index, PolyCorner polyCorner) {
        this.index = index;
        position = polyCorner.vertex.ToVector2();
    }
}

