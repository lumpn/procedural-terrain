using System.Collections.Generic;
using UnityEngine;

public static class DensityBuilder {
    public static IDensity Add(this IDensity a, IDensity b) {
        return new Density(p => a.Evaluate(p) + b.Evaluate(p));
    }
}

public class TerrainMesh : MonoBehaviour {

    private IDensity BuildDensity() {

        // perlin layers
        const float baseFreq = 0.05f;
        var perlin1 = new Perlin3(1, baseFreq * 1.01f, 1.0f);
        var perlin2 = new Perlin3(2, baseFreq * 0.49f, 2.0f);
        var perlin3 = new Perlin3(3, baseFreq * 0.27f, 4.0f);

        // base layer
        var plane = new Plane(0);

        // stitch together
        return plane.Add(perlin1).Add(perlin2).Add(perlin3);
    }

    void Start() {

        mesh = GetComponent<MeshFilter>().mesh;

        // create density function
        const float isoLevel = 0.0f;
        IDensity density = BuildDensity();

        // create surface evaluator
        MarchingCubes evaluator = new MarchingCubes();

        // sample density function
        SamplingRange xzRange = new SamplingRange(0, 10, 0.25f);
        SamplingRange yRange = new SamplingRange(-10, 10, 0.25f);
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
