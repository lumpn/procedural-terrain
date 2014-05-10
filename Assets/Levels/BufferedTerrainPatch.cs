using UnityEngine;
using System.Collections;
using System;

public class BufferedTerrainPatch {

    public BufferedTerrainPatch(int x, int y, int z, TerrainPatch patch) {
        this.x = x;
        this.y = y;
        this.z = z;
        this.patch = patch;
    }

    public bool IsAt(int px, int py, int pz) {
        return (x == px) && (y == py) && (z == pz);
    }

    public TerrainPatch GetPatch() {
        return patch;
    }

    public void Evaluate(int x, int y, int z, IDensity density, float isoLevel, int resolution) {

        // check position
        if (this.x == x && this.y == y && this.z == y) {
            // we have already evaluated this patch
            return;
        }

        // new position
        this.x = x;
        this.y = y;
        this.z = z;

        // re-position
        var offset = new Vector3(x * size, y * size, z * size);
        meshObject.transform.position = offset;

        // evaluate
        var range = new SamplingRange(-0.5f * size, 0.5f * size, size / resolution);
        IEnumerable<Vector3> vertices = evaluator.BuildSurface(density, isoLevel, range, range, range, offset);

        // re-build mesh
        startCoroutine(BuildMesh(vertices));
    }

    private IEnumerable BuildMesh(IEnumerable<Vector3> vertices) {

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
            Func<Vector3, Vector2> mapping = (p => new Vector2(p.x, p.z));
            if (nx >= nz && nx > ny) {
                mapping = (p => new Vector2(p.y, p.z));
            } else if (nz >= nx && nz > ny) {
                mapping = (p => new Vector2(p.x, p.y));
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

    private int x, y, z;
    private TerrainPatch patch;
}
