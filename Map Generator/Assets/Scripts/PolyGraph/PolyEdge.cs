using System.Collections.Generic;
using TriangleNet.Geometry;


public class PolyEdge
{
    //surrounding faces and surrounding corners are length 2 max, use array;
    public Vertex vertex;
    public int id;
    public List<PolyFace> surroundingFaces;
    public List<PolyCorner> surroundingCorners;

    // TODO: neighbouringEdges
    public PolyEdge(int id, PolyFace face0, PolyFace face1) 
    { 
        this.id = id;
        surroundingFaces = new List<PolyFace>() { face0, face1 };
        surroundingCorners = new List<PolyCorner>();
    }

    public PolyEdge(int id, PolyFace face0) 
    { 
        this.id = id;
        surroundingFaces = new List<PolyFace>() { face0 };
        surroundingCorners = new List<PolyCorner>();
    }
}
