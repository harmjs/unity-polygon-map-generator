using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Extensions;

public class Graph
    {
        public int width;
        public int height;
        public Corner[] corners;
        public Face[] faces;
        public Edge[] edges;

    public Graph(PolyGraph polyGraph) 
    {
        Initialize(polyGraph);
    }
    
    private void Initialize(PolyGraph polyGraph)  {
        width = polyGraph.width; 
        height = polyGraph.height;

        faces = polyGraph.faces
            .Select((kvp, i) => new Face(i, kvp.Value))
            .ToArray();
        corners = polyGraph.corners
            .Select((kvp, i) => new Corner(i, kvp.Value))
            .ToArray();
        edges = polyGraph.edges 
            .Select((kvp, i) => new Edge(i, kvp.Value))
            .ToArray();
            
        int index;

        for(index = 0; index < faces.Length; index++) { 
            Face face = faces[index];
            PolyFace polyFace = polyGraph.faces[index];

            face.corners = polyFace.surroundingCorners
                .Select(polyCorner => corners[polyCorner.id])
                .OrderBy(corner => face.position.GetAngle(corner.position))
                .ToArray();
            face.faces = polyFace.neighbouringFaces
                .Select(otherPolyFace => faces[otherPolyFace.id])
                .OrderBy(otherFace => face.position.GetAngle(otherFace.position))
                .ToArray();
            face.edges = polyFace.surroundingEdges
                .Select(polyEdge => edges[polyEdge.id])
                .OrderBy(edge => face.position.GetAngle(edge.position))
                .ToArray();

            face.triangles = polyFace.triangles;
            face.vertices = polyFace.vertices
                .Select(vertex => vertex.ToVector2())
                .ToArray();
            face.normals = polyFace.normals
                .Select((vertex => vertex.ToVector2()))
                .ToArray();
        }


        for(index = 0; index < corners.Length; index++) { 
            Corner corner = corners[index];
            PolyCorner polyCorner = polyGraph.corners[index];

            corner.faces = polyCorner.surroundingFaces
                .Select(polyFace => faces[polyFace.id])
                .OrderBy(face => corner.position.GetAngle(face.position))
                .ToArray();
            corner.edges = polyCorner.surroundingEdges
                .Select(polyEdge => edges[polyEdge.id])
                .OrderBy(edge => corner.position.GetAngle(edge.position))
                .ToArray();
        }

        for(index = 0; index < edges.Length; index++) { 
            Edge edge = edges[index];
            PolyEdge polyEdge = polyGraph.edges[index];

            edge.faces = polyEdge.surroundingFaces
                .Select(polyFace => faces[polyFace.id])
                .OrderBy(face => edge.position.GetAngle(face.position))
                .ToArray();

            edge.corners = polyEdge.surroundingCorners
                .Select(polyCorner => corners[polyCorner.id])
                .OrderBy(corner => edge.position.GetAngle(corner.position))
                .ToArray();
        }
    }
}
