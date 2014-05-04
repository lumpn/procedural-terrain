using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

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
        return new Density(p => {
            return plane.Evaluate(p)
                 + perlin1.Evaluate(p)
                 + perlin2.Evaluate(p)
                 + perlin3.Evaluate(p);
        });
    }

    void Start() {

        // measure execution time
        var process = Process.GetCurrentProcess();
        var time1 = process.TotalProcessorTime;

        // create density function
        const float isoLevel = 0.0f;
        IDensity density = BuildDensity();

        // create surface evaluator
        MarchingCubes evaluator = new MarchingCubes();

        // sample density function
        SamplingRange xyzRange = new SamplingRange(-5, 5, 0.25f);
        var surface = evaluator.BuildSurface(density, isoLevel, xyzRange, xyzRange, xyzRange, transform.position);

        // limit vertices
        const int maxVertices = 64998;
        var vertices = new List<Vector3>(surface);
        UnityEngine.Debug.Log("vertices: " + vertices.Count);
        if (vertices.Count > maxVertices) {
            vertices = vertices.GetRange(0, maxVertices);
        }

        // log execution time
        var time2 = process.TotalProcessorTime;
        UnityEngine.Debug.Log("generation: " + (time2 - time1).TotalMilliseconds + " ms");

        // build mesh
        UpdateMesh(vertices);
    }

    private void UpdateMesh(List<Vector3> vertices) {

        // build UVs and triangles from vertices
        var uvs = new List<Vector2>();
        var triangles = new List<int>();
        for (int i = 0; i < vertices.Count; i += 3) {

            // get vertices
            var a = vertices[i + 0];
            var b = vertices[i + 1];
            var c = vertices[i + 2];

            // calculate face normal
            var normal = Vector3.Cross(b - a, c - a);
            var nx = Mathf.Abs(normal.x);
            var ny = Mathf.Abs(normal.y);
            var nz = Mathf.Abs(normal.z);

            // select triplanar mapping by largest normal component
            Func<Vector3, Vector2> mapping = SelectXZ;
            if (nx >= nz && nx > ny) {
                mapping = SelectYZ;
            } else if (nz >= nx && nz > ny) {
                mapping = SelectXY;
            }

            // build UVs
            uvs.Add(mapping(a));
            uvs.Add(mapping(b));
            uvs.Add(mapping(c));

            //build triangle
            triangles.Add(i + 0);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
        }

        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    private static Vector2 SelectXY(Vector3 v) {
        return new Vector2(v.x, v.y);
    }

    private static Vector2 SelectXZ(Vector3 v) {
        return new Vector2(v.x, v.z);
    }

    private static Vector2 SelectYZ(Vector3 v) {
        return new Vector2(v.y, v.z);
    }
}
