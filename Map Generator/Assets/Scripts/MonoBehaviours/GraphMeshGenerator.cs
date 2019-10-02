using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GraphMeshGenerator : MonoBehaviour
{
    private GraphGenerator _graphGenerator;
    private Graph graph {
        get {
            if(_graphGenerator == null) {
                _graphGenerator = GetComponent<GraphGenerator>();
            }
            return _graphGenerator.graph;
        }
    }
    public float wallThreshold;
    public bool autoUpdate;

    public void GenerateGraphMesh() { 
        for(int q = 0; q < graph.faces.Length; q++) {
            
            Transform child = transform.GetChild(q);
            Tile tile = child.gameObject.GetComponent<Tile>();

            Face face = graph.faces[q];

            if(face.noise > wallThreshold) { 
                tile.ChangeType(new GroundTile());
            } else { 
                tile.ChangeType(new WallTile());
            }
        }
    }
}

[CustomEditor (typeof (GraphMeshGenerator))]
public class GraphMeshGeneratorEditor : Editor { 
    public override void OnInspectorGUI() {
        GraphMeshGenerator graphMeshGenerator = (GraphMeshGenerator)target;

        if(DrawDefaultInspector() && graphMeshGenerator.autoUpdate) { 
            graphMeshGenerator.GenerateGraphMesh();
        }

        if(GUILayout.Button("Generate")) { 
            graphMeshGenerator.GenerateGraphMesh();
        }
    }
}