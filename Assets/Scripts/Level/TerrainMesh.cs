﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TerrainMesh : MonoBehaviour {

    private IDensity BuildDensity() {

        // base layer
        var plane = new Plane(0);

        // perlin layers
        const float baseFreq = 0.025f;
        var perlin0 = new Perlin3(0, baseFreq * 3.99f, 0.25f);
        var perlin1 = new Perlin3(1, baseFreq * 2.03f, 0.50f);
        var perlin2 = new Perlin3(2, baseFreq * 1.01f, 1.00f);
        var perlin3 = new Perlin3(3, baseFreq * 0.49f, 2.00f);

        // floating islands
        var island1 = new FloatingIsland(new Vector3(10, 4, 10), 6);
        var island2 = new FloatingIsland(new Vector3(8, 2, -1), 4);
        var island3 = new FloatingIsland(new Vector3(0, 3, 11), 3);

        // rings
        var ring1 = new Ring(new Vector3(5, -4, 5), 8);
        var ring2 = new Ring(new Vector3(0, -2, 2), 4);

        // stitch together
        return new Density(p => {

            // start with plane
            float density = plane.Evaluate(p);

            // add perlin noise
            density += perlin0.Evaluate(p);
            density += perlin1.Evaluate(p);
            density += perlin2.Evaluate(p);
            density += perlin3.Evaluate(p);

            // combine with islands and add a bit of noise
            // add structural variation to big island
            float islands = -1000.0f;
            islands = Mathf.Max(islands, island1.Evaluate(p) + perlin1.Evaluate(p));
            islands = Mathf.Max(islands, island2.Evaluate(p));
            islands = Mathf.Max(islands, island3.Evaluate(p));
            islands += perlin0.Evaluate(p);
            density = Mathf.Max(density, islands);

            // combine with rings and a bit of noise
            float rings = -1000.0f;
            rings = Mathf.Max(rings, ring1.Evaluate(p));
            rings = Mathf.Max(rings, ring2.Evaluate(Quaternion.AngleAxis(30, Vector3.up) * p));
            rings += perlin0.Evaluate(p);
            density = Mathf.Max(density, rings);

            return density;
        });
    }

    void Start() {

        // create mesh collider for ray casts
        var collider = gameObject.AddComponent<MeshCollider>();
        collider.isTrigger = true;

        // create density function
        const float isoLevel = 0.0f;
        density = BuildDensity();

        // build mesh
        var vertices = BuildIsoSurface(density, isoLevel, transform.position);
        UpdateMesh(vertices);
    }

    public void IncreaseDensity(Vector3 position) {
        // TODO: implement
    }

    public void DecreaseDensity(Vector3 position) {
        // TODO: implement
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
            Func<Vector3, Vector2> mapping = (p => new Vector2(p.x, p.z));
            if (nx >= nz && nx > ny) {
                mapping = (p => new Vector2(p.y, p.z));
            } else if (nz >= nx && nz > ny) {
                mapping = (p => new Vector2(p.x, p.y));
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

        // update mesh
        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // update collider
        var collider = GetComponent<MeshCollider>();
        if (collider != null) {
            collider.sharedMesh = null;
            collider.sharedMesh = mesh;
        }
    }

    private static int PositionToIndex(float x) {
        // HACK: This is the hard coded inverse transformation
        // of density cache indizes. This breaks when hard coded
        // sampling is modified.
        return Mathf.FloorToInt((5 + x) * 4);
    }

    private static List<Vector3> BuildIsoSurface(IDensity density, float isoLevel, Vector3 offset) {

        // measure execution time
        var process = Process.GetCurrentProcess();
        var time1 = process.TotalProcessorTime;

        // sample density function
        MarchingCubes evaluator = new MarchingCubes();
        SamplingRange xyzRange = new SamplingRange(-5, 5, 0.25f);
        var surface = evaluator.BuildSurface(density, isoLevel, xyzRange, xyzRange, xyzRange, offset);
        var vertices = new List<Vector3>(surface);

        // log execution time
        var time2 = process.TotalProcessorTime;
        UnityEngine.Debug.Log("generation: " + (time2 - time1).TotalMilliseconds + " ms");

        // limit vertices (Unity constraint)
        const int maxVertices = 64998;
        UnityEngine.Debug.Log("vertices: " + vertices.Count);
        if (vertices.Count > maxVertices) {
            return vertices.GetRange(0, maxVertices);
        }

        return vertices;
    }

    private IDensity density;
}
