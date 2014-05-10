using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenerateTerrain : MonoBehaviour {

    // Use this for initialization
    void Start() {

        var vertices = new List<Vector3>();
        vertices.Add(new Vector3(0, 0, 0));
        vertices.Add(new Vector3(1, 0, 0));
        vertices.Add(new Vector3(0, 0, 1));
        vertices.Add(new Vector3(1, 0, 1));
        vertices.Add(new Vector3(2, 0, 0));

        var uvs = new List<Vector2>();
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(2, 0));

        var triangles = new List<int>();
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(1);
        triangles.Add(2);
        triangles.Add(3);

        var mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        var patch = GameObject.Find("Terrain Patch");
        var meshFilter = patch.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        for (int i = 1; i < 10; i++) {
            var clone = GameObject.Instantiate(patch) as GameObject;
            clone.transform.position = new Vector3(0, i, 0);

        }

        triangles.Add(1);
        triangles.Add(3);
        triangles.Add(4);

        mesh.triangles = triangles.ToArray();

        Debug.Log("done generating");
    }

    private void CreateMesh(GameObject patch, Mesh mesh) {
        var meshFilter = patch.GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    // Update is called once per frame
    void Update() {
        var camPos = Camera.main.transform.position;
        var patchIdx = camPos / patchSize;
        var drawBounds = new Vector3(drawDistance, drawDistance, drawDistance);
        var boundsMin = (camPos - drawBounds) / patchSize;
        var boundsMax = (camPos + drawBounds) / patchSize;

        var buffer = new TerrainBuffer(numPatches);
        var evaluator = new MarchingCubes();

        int px = Mathf.FloorToInt(patchIdx.x);
        int py = Mathf.FloorToInt(patchIdx.y);
        int pz = Mathf.FloorToInt(patchIdx.z);
        for (int x = px - numPatches; x < px + numPatches; x++) {
            buffer.Evaluate(x, 0, 0);
        }
    }

    private GameObject[, ,] patches = new GameObject[bufferSize, bufferSize, bufferSize];

    private const float drawDistance = 70.0f;
    private const float patchSize = 10.0f;

    private const int numPatches = (int)(drawDistance / patchSize);
    private const int numBorderPatches = 2;
    private const int bufferSize = (numPatches * 2) + numBorderPatches;
}
