using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Extensions;
using UnityEngine;
using UnityEditor;

public class GraphGenerator : MonoBehaviour {
    public int seed;
    public int width;
    public int height;
    public int faceSize;
    public int smoothingSteps;

    public Tile tilePrefab;

    [HideInInspector]
    public Graph graph;
    //https://forum.unity.com/threads/multiple-enum-select-from-inspector.184729/

    public bool autoUpdate;

    public void GenerateGraph() { 
        while(transform.childCount != 0) { 
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        Random.InitState(seed);

        PolyGraph polyGraph = new PolyGraph(width, height, faceSize, faceSize, smoothingSteps, new CentriodCorner());
        graph = new Graph(polyGraph);


        for(int index = 0; index < graph.faces.Length; index++) {
            Face face = graph.faces[index];
            Tile tile = (Tile)Object.Instantiate(
                tilePrefab, face.position, Quaternion.identity);
            tile.transform.SetParent(transform);
            tile.Initialize(face, new GroundTile());
            face.tile = tile;
        }

        GraphNoiseGenerator graphNoiseGenerator = GetComponent<GraphNoiseGenerator>();
        if(graphNoiseGenerator != null) graphNoiseGenerator.GenerateGraphNoise();
    }

    void OnValidate() { 
        if(width < faceSize * 2) {
            width = faceSize * 2 + 1;
        }
        if(height < faceSize * 2) { 
            height = faceSize * 2 + 1;
        }
    }

    void OnDrawGizmos() {
        
        if(graph != null) {
            Gizmos.color = Color.black;
            foreach(Edge edge in graph.edges) { 
                
                Gizmos.DrawLine(
                    edge.corners[0].position.ToVector3() + new Vector3(0, 0, -0.05f),
                    edge.corners[1].position.ToVector3() + new Vector3(0, 0, -0.05f)
                );
            }
        }
    }

}

[CustomEditor (typeof (GraphGenerator))]
public class GraphGeneratorEditor : Editor { 
    public override void OnInspectorGUI() {
        GraphGenerator graphGenerator = (GraphGenerator)target;

        if(DrawDefaultInspector() && graphGenerator.autoUpdate) { 
            graphGenerator.GenerateGraph();
        }

        if(GUILayout.Button("Generate")) { 
            graphGenerator.GenerateGraph();
        }
    }
}

