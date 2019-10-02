using System.Collections.Generic;
using TriangleNet.Geometry;
using TriangleNet;
using TriangleNet.Meshing;
using System;
using System.Linq;


public class PolyFace { 
    public Vertex vertex;

    public PolyCorner borderCorner; //can always add this
    public bool isFaceBorderCorner = false;
    public HashSet<PolyFace> neighbouringFaces;

    public HashSet<PolyCorner> surroundingCorners;

    public HashSet<PolyEdge> surroundingEdges;

    public Vertex[] vertices;
    public Vertex[] normals;
    public int[] triangles;
    public int id {
        get { return vertex.id; }
    }
    public Mesh mesh;

    public PolyFace(Vertex vertex) 
    { 
        this.vertex = vertex;
        neighbouringFaces = new HashSet<PolyFace>();
        surroundingCorners = new HashSet<PolyCorner>(){};
        surroundingEdges = new HashSet<PolyEdge>();
    }

    public bool onBorder() {
        return this.borderCorner != null;
    }

    public void GenerateMeshData(ConstraintOptions options) { 
        var polygon = new TriangleNet.Geometry.Polygon();
        vertices = surroundingCorners
            .Where(corner => isFaceBorderCorner | corner != borderCorner)
            .Select(corner => corner.vertex - vertex)
            .OrderBy(difference => difference.GetAngle())
            .ToArray();
        
        normals = new Vertex[vertices.Length];

        for(int q = 0; q < vertices.Length; q++) { 
            Vertex v0 = vertices[q];
            Vertex v1 = vertices[(q + 1) % vertices.Length];
            normals[q] = (v1 - v0).Perpendicular().Normalize();
        }

        foreach(Vertex vertex in vertices) {
            polygon.Add(vertex);
        }
    
        Mesh mesh = (Mesh)polygon.Triangulate(options);

        triangles = new int[mesh.triangles.Count * 3]; 
        int index = 0;

        // if conforming delauny is off Id should eqaul index
        // if not I will have to develop a work around
        foreach(var triangle in mesh.triangles) { 
            var vertices = triangle.vertices;
            triangles[index * 3] = vertices[2].id;
            triangles[index * 3 + 1] = vertices[1].id;
            triangles[index * 3 + 2] = vertices[0].id;
            index++;
        }
    }
}

