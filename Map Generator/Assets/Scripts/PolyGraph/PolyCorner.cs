using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet.Topology;
public class PolyCorner
{ 
    public int id;
    public Vertex vertex;
    public HashSet<PolyFace> surroundingFaces;
    public HashSet<PolyEdge> surroundingEdges;
    
    // TODO: neighbouringCorners
    public PolyCorner(int id, Triangle triangle, ICornerMode cornerMode) 
    { 
        this.id = id;
        vertex = cornerMode.FindVertex(triangle);
        surroundingFaces = new HashSet<PolyFace>();
        surroundingEdges = new HashSet<PolyEdge>();
    }

    public PolyCorner(int id, Vertex vertex) { 
        this.id = id;
        this.vertex = vertex;

        surroundingFaces = new HashSet<PolyFace>();
        surroundingEdges = new HashSet<PolyEdge>();
    }
}