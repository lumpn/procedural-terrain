using UnityEngine;
using System.Collections.Generic;

public class PolygonGenerator : MonoBehaviour {

    // This first list contains every vertex of the mesh that we are going to render
    public List<Vector3> vertices = new List<Vector3>();

    // The triangles tell Unity how to build each section of the mesh joining
    // the vertices
    public List<int> triangles = new List<int>();

    // The UV list is unimportant right now but it tells Unity how the texture is
    // aligned on each polygon
    public List<Vector2> uvs = new List<Vector2>();


    // A mesh is made up of the vertices, triangles and UVs we are going to define,
    // after we make them up we'll save them as this mesh
    private Mesh mesh;

    // Use this for initialization
    void Start() {

        mesh = GetComponent<MeshFilter>().mesh;

        const int sizeX = 10;
        const int sizeZ = 10;

        // build height map
        float[,] heightMap = new float[sizeX + 1, sizeZ + 1];
        for (int x = 0; x < sizeX + 1; x++) {
            for (int z = 0; z < sizeZ + 1; z++) {
                heightMap[x, z] = Random.Range(0.0f, 1.0f);
            }
        }

        // build mesh
        ClearTiles();
        for (int x = 0; x < sizeX; x++) {
            for (int z = 0; z < sizeZ; z++) {
                AddTile(x, z, heightMap);
            }
        }
        UpdateMesh();
        ClearTiles();
    }

    void UpdateMesh() {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    void AddTile(int x, int z, float[,] heightMap) {
        int baseIndex = vertices.Count;

        vertices.Add(resolve(x, z, heightMap));
        vertices.Add(resolve(x + 1, z, heightMap));
        vertices.Add(resolve(x + 1, z + 1, heightMap));
        vertices.Add(resolve(x, z + 1, heightMap));

        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(1, 0));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(0, 1));

        triangles.Add(baseIndex + 3);
        triangles.Add(baseIndex + 1);
        triangles.Add(baseIndex + 0);

        triangles.Add(baseIndex + 3);
        triangles.Add(baseIndex + 2);
        triangles.Add(baseIndex + 1);
    }

    void ClearTiles() {
        vertices.Clear();
        uvs.Clear();
        triangles.Clear();
    }

    Vector3 resolve(int x, int z, float[,] heightMap) {
        return new Vector3(x, heightMap[x, z], z);
    }
}
