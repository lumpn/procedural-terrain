using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TerrainPatch {

    public TerrainPatch(GameObject meshObject) {
        this.meshObject = meshObject;
    }

    public IEnumerator Evaluate(MarchingCubes evaluator, IDensity density, float isoLevel, Vector3 offset, float size, int resolution) {

        // evaluate density
        var range = new SamplingRange(-size / 2, size / 2, size / resolution);
        var surface = evaluator.BuildSurface(density, isoLevel, range, range, range, offset);

        // build vertices, UVs, and triangles from vertices
        var vertices = new List<Vector3>();
        var uvs = new List<Vector2>();
        var triangles = new List<int>();

        int i = 0;
        int idx = 0;
        Vector3[] face = new Vector3[3];
        foreach (var vertex in surface) {
            //yield return 0;

            // update face vertices
            face[idx++] = vertex;
            if (idx < 3) continue;
            idx = 0;

            // calculate face normal
            var normal = Vector3.Cross(face[1] - face[0], face[2] - face[0]);
            var nx = Mathf.Abs(normal.x);
            var ny = Mathf.Abs(normal.y);
            var nz = Mathf.Abs(normal.z);

            // select triplanar mapping by largest normal component
            Func<Vector3, Vector2> mapping = GetXZ;
            if (nx >= nz && nx > ny) {
                mapping = GetYZ;
            } else if (nz >= nx && nz > ny) {
                mapping = GetXY;
            }

            // build vertices
            vertices.Add(face[0]);
            vertices.Add(face[1]);
            vertices.Add(face[2]);

            // build UVs
            uvs.Add(mapping(face[0]));
            uvs.Add(mapping(face[1]));
            uvs.Add(mapping(face[2]));

            //build triangle
            triangles.Add(i++);
            triangles.Add(i++);
            triangles.Add(i++);
        }

        // update mesh
        //yield return 0;
        var mesh = meshObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // update collider
        yield return 0;
        var collider = meshObject.GetComponent<MeshCollider>();
        if (collider != null) {
            collider.sharedMesh = null;
            collider.sharedMesh = mesh;
        }

        // re-position
        yield return 0;
        meshObject.transform.position = offset;
    }

    private static Vector2 GetXY(Vector3 v) {
        return new Vector2(v.x, v.y);
    }

    private static Vector2 GetXZ(Vector3 v) {
        return new Vector2(v.x, v.z);
    }

    private static Vector2 GetYZ(Vector3 v) {
        return new Vector2(v.y, v.z);
    }

    private GameObject meshObject;
}
