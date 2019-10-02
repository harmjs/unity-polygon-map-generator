
using TriangleNet.Geometry;
using TriangleNet.Topology;
using System;
using UnityEngine;

public interface ICornerMode { 
    Vertex FindVertex(Triangle triangle);
}

public class CentriodCorner : ICornerMode {
    
    public Vertex FindVertex(Triangle triangle) { 
        double totalX = 0;
        double totalY = 0;

        foreach (Vertex vertex in triangle.vertices)
        {
            totalX += vertex.x;
            totalY += vertex.y;
        }

        return new Vertex(totalX/3, totalY/3);
    } 
}   

public class IncenterCorner : ICornerMode {
    public Vertex FindVertex(Triangle triangle) {
        Vertex A = triangle.vertices[0];
        Vertex B = triangle.vertices[1];
        Vertex C = triangle.vertices[2];

        double a = Math.Sqrt(Math.Pow(B.x - C.x, 2) + Math.Pow(B.y - C.y, 2));
        double b = Math.Sqrt(Math.Pow(C.x - A.x, 2) + Math.Pow(C.y - A.y, 2));
        double c = Math.Sqrt(Math.Pow(B.x - A.x, 2) + Math.Pow(B.y - A.y, 2));

        double x = (a * A.x + b * B.x + c * C.x) / (a + b + c);
        double y = (a * A.y + b * B.y + c * C.y) / (a + b + c);

        return new Vertex(x, y);
    }
}