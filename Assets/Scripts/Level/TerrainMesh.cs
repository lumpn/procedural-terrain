using System.Collections.Generic;
using UnityEngine;

public class TerrainMesh : MonoBehaviour {

    private IDensity CreateDensity() {
        return new PerlinPlane(1 / 20.0f, 5.0f);
    }

    private const int minXZ = 0;
    private const int maxXZ = 100;

    private const float epsilon = 0.001f;

    // Use this for initialization
    void Start() {

        mesh = GetComponent<MeshFilter>().mesh;

        // create density function
        IDensity density = CreateDensity();
        float isoLevel = 0.0f;

        // create surface evaluator
        MarchingCubes evaluator = new MarchingCubes();

        // sample density function
        SamplingRange xzRange = new SamplingRange(0, 20, 0.5f);
        SamplingRange yRange = new SamplingRange(-10, 10, 0.5f);
        var surface = evaluator.BuildSurface(density, isoLevel, xzRange, yRange, xzRange);

        // limit vertices
        const int maxVertices = 64998;
        var vertices = new List<Vector3>(surface);
        Debug.Log("vertices: " + vertices.Count);
        if (vertices.Count > maxVertices) {
            vertices = vertices.GetRange(0, maxVertices);
        }

        // build mesh
        UpdateMesh(vertices);
    }

    private void UpdateMesh(List<Vector3> vertices) {

        var uvs = new List<Vector2>();
        var triangles = new List<int>();

        // TODO step face-wise (i+=3)
        int idx = 0;
        foreach (var vertex in vertices) {
            // TODO uvs by triplanar texturing!
            uvs.Add(new Vector2(vertex.x, vertex.z));
            triangles.Add(idx++);
        }

        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    private Mesh mesh;
}
