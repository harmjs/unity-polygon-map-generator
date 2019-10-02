using UnityEngine;
using UnityEditor;
public class GraphNoiseGenerator : MonoBehaviour {
    private GraphGenerator _graphGenerator;
    private Graph graph {
        get {
            if(_graphGenerator == null) {
                _graphGenerator = GetComponent<GraphGenerator>();
            }
            return _graphGenerator.graph;
        }
    }
    
    public float scale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public bool autoUpdate;

    public void GenerateGraphNoise() {
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float[] noiseMap = new float[graph.faces.Length];

        int q;
        for(q = 0; q < graph.faces.Length; q++) { 
            Face face = graph.faces[q];
            
            float amplitude = 1;
            float frequency = 1;
            float noiseHeight = 0;

            for(int o = 0; o < octaves; o++) { 
                float sampleX = face.position.x  / scale * frequency;
                float sampleY = face.position.y / scale * frequency;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseHeight += perlinValue * amplitude;
                amplitude *= persistance;
                frequency *= lacunarity;
            }

            if(noiseHeight > maxNoiseHeight) { 
                maxNoiseHeight = noiseHeight;
            }else if(noiseHeight < minNoiseHeight) {
                minNoiseHeight = noiseHeight;
            }
            noiseMap[q] = noiseHeight;
        }

        for(q = 0; q < graph.faces.Length; q++) {
            Transform child = transform.GetChild(q);
            Renderer renderer = child.gameObject.GetComponent<Renderer>();
            float noise = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[q]);

            graph.faces[q].noise = noise;
            renderer.material.color = Color.Lerp(Color.black, Color.white, noise);
        }

        GraphMeshGenerator graphMeshGenerator = GetComponent<GraphMeshGenerator>();
        if(graphMeshGenerator != null) graphMeshGenerator.GenerateGraphMesh();
    }
}

[CustomEditor (typeof (GraphNoiseGenerator))]
public class GraphNoiseGeneratorEditor : Editor { 
    public override void OnInspectorGUI() {
        GraphNoiseGenerator graphNoiseGenerator = (GraphNoiseGenerator)target;

        if(DrawDefaultInspector() && graphNoiseGenerator.autoUpdate) { 
            graphNoiseGenerator.GenerateGraphNoise();
        }

        if(GUILayout.Button("Generate")) { 
            graphNoiseGenerator.GenerateGraphNoise();
        }
    }
}

