using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using TriangleNet.Geometry;
using TriangleNet.Meshing;
using TriangleNet.Smoothing;
using TriangleNet.Topology;
using TriangleNet;

public class PolyGraph
{
    public Dictionary<int, PolyFace> faces;
    public Dictionary<int, PolyCorner> corners;
    public Dictionary<int, PolyEdge> edges; 
    public int height;
    public int width;
    private int edgeIdCounter;
    private int cornerIdCounter;

    public PolyGraph(
        int width, int height, int sparcity, int padding,
        int smoothingSteps, ICornerMode cornerMode ) 
    {

        this.width = width;
        this.height = height;

        edgeIdCounter = 0;
        cornerIdCounter = 0;

        faces = new Dictionary<int, PolyFace>();
        corners = new Dictionary<int, PolyCorner>();
        edges = new Dictionary<int, PolyEdge>();

        Mesh faceMesh;
        
        // Randomly generate a delaunay trianglulation to use as faces for the PolyGraph.
        {
            Polygon polygon = new Polygon();
            Vertex[] bounds = new Vertex[4] {
                new Vertex(0, 0),
                new Vertex(0, height),
                new Vertex(width, height),
                new Vertex(width, 0)
            };

            PoissonDiscSampler sampler = new PoissonDiscSampler(
                width - 2*padding, height -2* padding, sparcity);


            SimpleSmoother smoother = new SimpleSmoother();
            
            for(int i = 0; i < 4; i++) {
                polygon.Add(new Segment(
                    bounds[i],
                    bounds[(i + 1) % 4],
                    1
                ), 0);
            }

            foreach (UnityEngine.Vector2 sample in sampler.Samples()) {
                polygon.Add(new Vertex(sample.x + padding, sample.y + padding));
            }

            faceMesh = (Mesh)polygon.Triangulate(
                new ConstraintOptions() {  ConformingDelaunay = true });

            smoother.Smooth(faceMesh, smoothingSteps);
        }

        // Compose and add faces to face dictionary.


        foreach (Vertex vertex in faceMesh.Vertices) 
        {

            int xInt = Convert.ToInt32(vertex.x);
            int yInt = Convert.ToInt32(vertex.y);

            bool x0Border = xInt == 0;
            bool x1Border = xInt == width;
            bool y0Border = yInt == 0;
            bool y1Border = yInt == height;

            PolyFace face;

            if(x0Border | x1Border | y0Border | y1Border) { 
                Vertex borderVertex = new Vertex();
                borderVertex.x = vertex.x;
                borderVertex.y = vertex.y;

                if(x0Border) vertex.x += padding/2;
                if(x1Border) vertex.x -= padding/2;
                if(y0Border) vertex.y += padding/2;
                if(y1Border) vertex.y -= padding/2;

                PolyCorner borderCorner = new PolyCorner(cornerIdCounter++, borderVertex);
                face = new PolyFace(vertex);
                face.borderCorner = borderCorner;

                if((x0Border?1:0) + (x1Border?1:0) + (y0Border?1:0) + (y1Border?1:0) == 2) {
                    face.isFaceBorderCorner = true;
                }

                borderCorner.surroundingFaces.Add(face);
                face.surroundingCorners.Add(borderCorner);

                corners.Add(borderCorner.id, borderCorner);
            } else { 
                face = new PolyFace(vertex);
            }

            faces.Add(face.id, face);
        }

        foreach(Triangle triangle in faceMesh.triangles) 
        { 
            // Compose corner dicitonary from remaining faces.
            PolyCorner corner = new PolyCorner(cornerIdCounter++, triangle, cornerMode);
            
            foreach(Vertex faceVertex in triangle.vertices) 
            { 
                PolyFace face = faces[faceVertex.id];

                face.surroundingCorners.Add(corner);
                corner.surroundingFaces.Add(face);
            }
            corners.Add(corner.id, corner);
        }

        // Compose and add edges to edge dictionary.
        foreach(TriangleNet.Geometry.Edge meshEdge in faceMesh.Edges) 
        { 
            // Add pointers between corners faces and edges.
            PolyFace face0 = faces[meshEdge.P0];
            PolyFace face1 = faces[meshEdge.P1];

            face0.neighbouringFaces.Add(face1);
            face1.neighbouringFaces.Add(face0);

            PolyEdge edge = new PolyEdge(edgeIdCounter++, face0, face1);
            edges.Add(edge.id, edge);

            face0.surroundingEdges.Add(edge);
            face1.surroundingEdges.Add(edge);

            foreach(PolyCorner corner in face0.surroundingCorners)
            {
                if(face1.surroundingCorners.Contains(corner)) {
                    edge.surroundingCorners.Add(corner);
                    corner.surroundingEdges.Add(edge);
                }
            }

            if(edge.surroundingCorners.Count == 1) {

                PolyCorner faceCorner0 = face0.borderCorner;
                PolyCorner faceCorner1 = face1.borderCorner;

                Vertex f0 = faceCorner0.vertex;
                Vertex f1 = faceCorner1.vertex;

                Vertex v0 = edge.surroundingCorners[0].vertex;

                Vertex f = f1 - f0;
                Vertex v = v0 - f0;

                Vertex projvf = (((f * v) / Math.Pow(f.Magnitude(), 2)) * f);

                PolyCorner corner = new PolyCorner(cornerIdCounter++, projvf + f0);

                edge.surroundingCorners.Add(corner);
                corner.surroundingEdges.Add(edge);

                corner.surroundingFaces.Add(face1);
                corner.surroundingFaces.Add(face0);

                face0.surroundingCorners.Add(corner);
                face1.surroundingCorners.Add(corner);


                corners.Add(corner.id, corner);

                PolyEdge faceEdge0 = new PolyEdge(edgeIdCounter++, face0);
                PolyEdge faceEdge1 = new PolyEdge(edgeIdCounter++, face1);

                edges.Add(faceEdge0.id, faceEdge0);
                edges.Add(faceEdge1.id, faceEdge1);

                faceEdge0.surroundingCorners.Add(faceCorner0);
                faceEdge0.surroundingCorners.Add(corner);

                faceCorner0.surroundingEdges.Add(faceEdge0);

                faceEdge1.surroundingCorners.Add(faceCorner1);
                faceEdge1.surroundingCorners.Add(corner);

                faceCorner1.surroundingEdges.Add(faceEdge1);
            }
        }

        foreach(KeyValuePair<int, PolyEdge> kvp in edges) { 
            PolyEdge edge = kvp.Value;
            edge.vertex = 
                (edge.surroundingCorners[0].vertex 
                + edge.surroundingCorners[1].vertex) / 2; 
        }

        ConstraintOptions options = new ConstraintOptions() { 
            ConformingDelaunay = false
        };

        foreach(KeyValuePair<int, PolyFace> kvp in faces ) {
            PolyFace face = kvp.Value;
            face.GenerateMeshData(options);
        }
    }
}