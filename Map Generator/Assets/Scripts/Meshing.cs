using UnityEngine;
using TriangleNet.Meshing;
using System.Collections.Generic;
using System.Linq;
using System;

using TriangleNet.Geometry;

public class Meshing {

    static ConstraintOptions options = new ConstraintOptions() {
        ConformingDelaunay = false
    };

    public float tileHeight;

    public Meshing(float tileHeight) { 
        this.tileHeight = -tileHeight;
    }

    private TriangleNet.Mesh Create2DFaceMesh(PolyFace face) { 
        TriangleNet.Geometry.Polygon polygon = new TriangleNet.Geometry.Polygon();

        face.surroundingCorners
            .Where(corner => face.isFaceBorderCorner | corner != face.borderCorner)
            .Select(corner => corner.vertex - face.vertex)
            .OrderBy(vertex => {
                double angle = Math.Atan2(vertex.y, vertex.x);
                if(angle < 0 ) angle += Math.PI * 2;
                return angle;
            })
            .ToList()
            .ForEach(vertex => polygon.Add(vertex));

        return (TriangleNet.Mesh)polygon.Triangulate(options);
    }


    public UnityEngine.Mesh CreateFloorTileMesh(PolyFace face) { 
        Vector2 cancer = new Vector2(1, 1);

        var polyMesh = Create2DFaceMesh(face);
 
        Vector3[] vertices = new Vector3[polyMesh.vertices.Count];
        Vector3[] normals = new Vector3[polyMesh.vertices.Count];
        int[] triangles = new int[polyMesh.triangles.Count * 3];
        Vector2[] uv = new Vector2[polyMesh.vertices.Count];

        for(int index = 0; index < polyMesh.vertices.Count; index++) {
            var vertex = polyMesh.vertices[index];
            vertices[index] = vertex.ToVector3();
            normals[index] = new Vector3(0, 0, -1);
        }
        {
            int index = 0;
            foreach(var triangle in polyMesh.triangles) { 
                var data = triangle.vertices;
                triangles[index * 3 + 2] = data[0].id;
                triangles[index * 3 + 1] = data[1].id;
                triangles[index * 3] = data[2].id;
                index++;
            }
        }

        UnityEngine.Mesh mesh = new UnityEngine.Mesh();
       
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;

        return mesh;
    }

    public UnityEngine.Mesh CreateWallTileMesh(PolyFace face) { 
        // SORT CORNERS AND NEIGHBOURS SO THEY ARE IN ORDER

        var polyMesh = Create2DFaceMesh(face);

        var VC = polyMesh.vertices.Count;
        var TC = polyMesh.triangles.Count;

        Vector3[] vertices = new Vector3[VC * 4 + VC];
        Vector3[] normals = new Vector3[VC * 4 + VC];
        int[] triangles = new int[VC * 2 * 3 + TC * 3];

        int index;
        for(index = 0; index < VC; index++) { 
            Vertex v0 = polyMesh.vertices[index];
            Vertex v1 = polyMesh.vertices[(index + 1) % VC];
            Vector3 normal = (v1 - v0).Perpendicular().Normalize().ToVector3();

            vertices[index * 4] = v0.ToVector3();
            vertices[index * 4 + 1] = v1.ToVector3();
            vertices[index * 4 + 2] = v0.ToVector3(tileHeight);
            vertices[index * 4 + 3] = v1.ToVector3(tileHeight);

            normals[index * 4] = normal;
            normals[index * 4 + 1] = normal;
            normals[index * 4 + 2] = normal;
            normals[index * 4 + 3] = normal;

            triangles[index * 6] = index * 4 + 2;
            triangles[index * 6 + 1] = index * 4 + 1;
            triangles[index * 6 + 2] = index * 4;
            triangles[index * 6 + 3] = index * 4 + 1;
            triangles[index * 6 + 4] = index * 4 + 2;
            triangles[index * 6 + 5] = index * 4 + 3;

            vertices[VC * 4 + index] = v0.ToVector3(tileHeight); 
            normals[VC * 4 + index]  = new Vector3(0, 0, -1);
        }

        index = 0;
        foreach(var triangle in polyMesh.triangles) { 
            var data = triangle.vertices;
            triangles[VC * 2 * 3 + index * 3 + 0] = VC * 4 + data[2].id;
            triangles[VC * 2 * 3 + index * 3 + 1] = VC * 4 + data[1].id;
            triangles[VC * 2 * 3 + index * 3 + 2] = VC * 4 + data[0].id;
            index++;
        }

        UnityEngine.Mesh mesh = new UnityEngine.Mesh();
       
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;

        return mesh;
    }
}

