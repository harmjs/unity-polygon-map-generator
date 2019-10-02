using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;

using UnityEngine;
public class FloorTileMeshType : ITileMeshType {
    public Mesh Create(Mesh mesh, Face face) { 
        mesh.Clear();

        Vector3[] vertices = face.vertices
            .Select(vertex => vertex.ToVector3())
            .ToArray();
        Vector3[] normals = face.vertices
            .Select(vertex => new Vector3(0, 0, -1))
            .ToArray();
        int[] triangles = face.triangles;

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;

        return mesh;
    }
}

public class WallTileMeshType : ITileMeshType { 
    public float wallHeight = -10;
    public Mesh Create(Mesh mesh, Face face) {
        mesh.Clear();
        int VC = face.vertices.Length;
        int TC = face.triangles.Length;

        Vector3[] vertices = new Vector3[VC * 4 + VC];
        Vector3[] normals = new Vector3[VC * 4 + VC];
        int[] triangles = new int[VC * 2 * 3 + TC * 3];

        for(int i = 0; i < VC; i++) { 
            Vector2 v0 = face.vertices[i];
            Vector2 v1 = face.vertices[(i + 1) % VC];

            vertices[i * 4] = v0.ToVector3();
            vertices[i * 4 + 1] = v1.ToVector3();
            vertices[i * 4 + 2] = v0.ToVector3(wallHeight);
            vertices[i * 4 + 3] = v1.ToVector3(wallHeight);

            Vector3 normal = face.normals[i].ToVector3();
            
            normals[i * 4] = normal;
            normals[i * 4 + 1] = normal;
            normals[i * 4 + 2] = normal;
            normals[i * 4 + 3] = normal;

            triangles[i * 6] = i * 4 + 2;
            triangles[i * 6 + 1] = i * 4 + 1;
            triangles[i * 6 + 2] = i * 4;
            triangles[i * 6 + 3] = i * 4 + 1;
            triangles[i * 6 + 4] = i * 4 + 2;
            triangles[i * 6 + 5] = i * 4 + 3;

            vertices[VC * 4 + i] = v0.ToVector3(wallHeight);
            normals[VC * 4 + i]  = new Vector3(0, 0, -1);
        }

        for(int t = 0; t < TC; t++) { 
            triangles[VC * 2 * 3 + t] = VC * 4 + face.triangles[t];
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;

        return mesh;
    }
}

public interface ITileMeshType { 
    Mesh Create(Mesh mesh, Face face);
}

public interface ITileType{
    ITileMeshType mesh { get; }
}

public class GroundTile : ITileType { 
    public ITileMeshType mesh { get; }

    public GroundTile() { 
        mesh = new FloorTileMeshType();
    }
}

public class WallTile : ITileType { 
    public ITileMeshType mesh { get; }

    public WallTile() { 
        mesh = new WallTileMeshType();
    }
}

public class Tile : MonoBehaviour
{
    private MeshFilter meshFilter;
    private Mesh mesh;
    public Face face;
    public ITileType type;

    public float noise;

    public void Initialize(Face face, ITileType type) { 
        this.face = face;
        this.type = type;

        mesh = type.mesh.Create(new Mesh(), face);

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    public void ChangeType(ITileType nextType) {
        if(type.mesh.GetType() == nextType.mesh.GetType()) {
            mesh = nextType.mesh.Create(mesh, face);
        }
        type = nextType;
    }
}