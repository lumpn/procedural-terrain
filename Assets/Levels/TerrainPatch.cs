using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class TerrainPatch {

    public TerrainPatch(GameObject meshObject) {
        this.meshObject = meshObject;
    }

    public IEnumerable Evaluate(IDensity density, float isoLevel, Vector3 offset, float size, int resolution, MarchingCubes evaluator) {

        // re-position
        meshObject.transform.position = offset;

        // evaluate density
        var range = new SamplingRange(-0.5f * size, 0.5f * size, size / resolution);
        IEnumerable<Vector3> vertices = evaluator.BuildSurface(density, isoLevel, range, range, range, offset);

        // build UVs and triangles from vertices
        var uvs = new List<Vector2>();
        var triangles = new List<int>();

        int i = 0;
        int idx = 0;
        Vector3[] face = new Vector3[3];
        foreach (var vertex in vertices) {
            yield return 0;

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
            Func<Vector3, Vector2> mapping = MapXZ;
            if (nx >= nz && nx > ny) {
                mapping = MapYZ;
            } else if (nz >= nx && nz > ny) {
                mapping = MapXY;
            }

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
        yield return 0;
        var mesh = meshObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.uv = uvs.ToArray();
        mesh.vertices = vertices.ToArray();
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
    }

    private static Vector2 MapXY(Vector3 v) {
        return new Vector2(v.x, v.y);
    }

    private static Vector2 MapXZ(Vector3 v) {
        return new Vector2(v.x, v.z);
    }

    private static Vector2 MapYZ(Vector3 v) {
        return new Vector2(v.y, v.z);
    }

    private GameObject meshObject;
}
